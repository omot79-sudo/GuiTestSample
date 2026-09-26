using System;
using System.Collections.Generic;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Exceptions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using GuiTestKit.Evidence;
using GuiTestKit.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GuiTestKit.Automation
{
    /// <summary>
    /// 全画面共通の Page 基底クラス。
    /// 部品は変数として定義せず、項目ID（AutomationId = WinForms の Name）の文字列で指定する。
    /// 画面ごとのクラスには「基本値」「比較対象外の項目」「その画面特有の操作」だけを書く。
    /// </summary>
    public abstract class ScreenPageBase
    {
        /// <summary>部品や画面が出てくるまで待つ最大時間。</summary>
        public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        private Dictionary<string, AutomationElement> _index;

        protected ScreenPageBase(Window window)
        {
            if (window == null) throw new ArgumentNullException("window");
            Window = window;
            var session = TestSession.Current;
            if (session != null) session.RegisterPage(this);
        }

        public Window Window { get; private set; }

        /// <summary>この画面の全項目の標準的な入力値。GuiTestTool dump で画面から吸い出して作る。</summary>
        public virtual ScreenValues BaseValues { get { return new ScreenValues(); } }

        /// <summary>実行するたびに変わる項目（日時・採番など）。VB/C# の比較で値も画像も比較しない。</summary>
        public virtual IEnumerable<string> CompareExcludedIds { get { return Enumerable.Empty<string>(); } }

        /// <summary>テキストボックスへの入力方法。キー入力イベントで動く画面は Keyboard にする。</summary>
        protected virtual InputMode TextInputMode { get { return InputMode.SetValue; } }

        /// <summary>この画面で使う、市販部品などの読み書きの差し替え。既定は全画面共通の登録内容。</summary>
        public virtual IEnumerable<ICustomControlHandler> CustomHandlers { get { return CustomControls.Handlers; } }

        // ------------------------------------------------------------------
        // 値の入力・取得
        // ------------------------------------------------------------------

        /// <summary>
        /// 基本値を全部入力する。changes を渡すと、その項目だけ基本値から変えて入力する。
        /// レポートには「基本値を入力」と「基本値から変えた項目」だけが出る。
        /// </summary>
        public void EnterBaseValues(ScreenValues changes = null)
        {
            var values = BaseValues.With(changes);
            Log("基本値を入力（" + values.Count + "項目）");
            if (changes != null)
            {
                foreach (var c in changes)
                {
                    var baseValue = BaseValues[c.Id];
                    Log("　変更: " + Describe(c.Id) + " = " + Show(c.Value) + (BaseValues.Contains(c.Id) ? "（基本値: " + Show(baseValue) + "）" : ""));
                }
            }
            foreach (var v in values) SetCore(v.Id, v.Value);
        }

        /// <summary>複数の項目を、書いた順に入力する（基本値を使わない場合）。</summary>
        public void SetValues(ScreenValues values)
        {
            foreach (var v in values)
            {
                Log("入力: " + Describe(v.Id) + " = " + Show(v.Value));
                SetCore(v.Id, v.Value);
            }
        }

        /// <summary>1項目を入力する。テキスト・コンボは表示文字列、チェック・ラジオは ON/OFF。</summary>
        public void Set(string id, string value)
        {
            Log("入力: " + Describe(id) + " = " + Show(value));
            SetCore(id, value);
        }

        /// <summary>1項目を実際にキーを打鍵して入力する。</summary>
        public void Type(string id, string value)
        {
            Log("打鍵入力: " + Describe(id) + " = " + Show(value));
            ControlAccess.Write(Find(id), value, InputMode.Keyboard, CustomHandlers);
        }

        public string Get(string id)
        {
            return ControlAccess.Read(Find(id), CustomHandlers);
        }

        public bool IsEnabled(string id)
        {
            return Find(id).IsEnabled;
        }

        /// <summary>画面の全項目の状態（値・有効/無効）を読み取る。</summary>
        public List<ControlSnapshot> ReadAll()
        {
            return ControlAccess.ReadAll(Window, CustomHandlers, Window.BoundingRectangle.Location);
        }

        private void SetCore(string id, string value)
        {
            if (value == null) return;   // null は「触らない」

            var element = Find(id);
            if (!element.IsEnabled)
            {
                // 無効な項目：すでにその値なら何もしない（区分によって無効になる項目など）
                var current = ControlAccess.Read(element, CustomHandlers);
                if (current == value || (value.Length == 0 && string.IsNullOrEmpty(current))) return;
                throw new AssertFailedException(Describe(id) + " は無効のため「" + value + "」を入力できません（現在値: " + current + "）。入力順か基本値を見直してください。");
            }

            try
            {
                ControlAccess.Write(element, value, TextInputMode, CustomHandlers);
            }
            catch (Exception ex)
            {
                throw new AssertFailedException(Describe(id) + " に「" + value + "」を入力できませんでした: " + ex.Message, ex);
            }
        }

        // ------------------------------------------------------------------
        // ボタン操作・画面遷移
        // ------------------------------------------------------------------

        /// <summary>
        /// ボタンを押す（UI Automation の Invoke。マウスは動かない）。
        /// 押すとダイアログや別画面が開くボタンには Click を使うこと（Invoke だと戻ってこない場合がある）。
        /// </summary>
        public void Press(string id)
        {
            Log("押下: " + Describe(id));
            Find(id).AsButton().Invoke();
            Wait.UntilInputIsProcessed();
        }

        /// <summary>マウスでクリックする。</summary>
        public void Click(string id)
        {
            Log("クリック: " + Describe(id));
            ClickCore(id);
        }

        /// <summary>ボタンをクリックし、表示されたメッセージボックスを返す。</summary>
        public MessageBoxPage ClickAndWaitMessageBox(string id)
        {
            Click(id);
            return WaitForMessageBox();
        }

        /// <summary>ボタンをクリックして別画面を開き、その画面の Page を返す。</summary>
        protected T OpenScreen<T>(string buttonId, string windowId, Func<Window, T> createPage) where T : ScreenPageBase
        {
            Click(buttonId);
            return createPage(WaitForWindow(windowId));
        }

        /// <summary>この画面から開いたメッセージボックス（標準ダイアログ）を待って返す。</summary>
        public MessageBoxPage WaitForMessageBox(TimeSpan? timeout = null)
        {
            var dialog = Retry.WhileNull(() => FindMessageBox(), timeout ?? DefaultTimeout).Result;
            if (dialog == null) throw new AssertFailedException("メッセージボックスが表示されませんでした。");
            return new MessageBoxPage(dialog);
        }

        /// <summary>画面（フォーム）を AutomationId で探して待つ。WinForms ではフォームの Name。</summary>
        protected Window WaitForWindow(string windowId, TimeSpan? timeout = null)
        {
            var window = Retry.WhileNull(() => FindWindow(windowId), timeout ?? DefaultTimeout).Result;
            if (window == null) throw new AssertFailedException("画面が表示されませんでした: " + windowId);
            window.SetForeground();
            return window;
        }

        /// <summary>画面を閉じる（タイトルバーの×と同じ）。</summary>
        public void CloseWindow()
        {
            Log("画面を閉じる: " + Window.Title);
            Window.Close();
        }

        /// <summary>この画面が閉じるまで待つ。</summary>
        public bool WaitUntilClosed(TimeSpan? timeout = null)
        {
            return Retry.WhileTrue(() => IsOpen(), timeout ?? DefaultTimeout).Result;
        }

        /// <summary>条件が true になるまで待つ（固定の Sleep を使わないため）。</summary>
        public bool WaitUntil(Func<bool> condition, TimeSpan? timeout = null)
        {
            return Retry.WhileFalse(condition, timeout ?? DefaultTimeout).Result;
        }

        // ------------------------------------------------------------------
        // 部品の検索
        // ------------------------------------------------------------------

        /// <summary>
        /// 項目IDで部品を探す（出てくるまで待つ）。
        /// "親ID/子ID" で入れ物の中を指定、"ID#2" で同じIDの2つ目を指定できる。
        /// </summary>
        public AutomationElement Find(string id)
        {
            var element = Retry.WhileNull(() => FindOrNull(id), DefaultTimeout).Result;
            if (element == null) throw new AssertFailedException("部品が見つかりません: " + id + "（画面: " + Window.Title + "）");
            return element;
        }

        /// <summary>項目IDで部品を探す（待たない。無ければ null）。</summary>
        public AutomationElement FindOrNull(string id)
        {
            AutomationElement cached;
            if (_index != null && _index.TryGetValue(id, out cached) && IsAlive(cached)) return cached;

            // 画面全体を1回だけ走査して索引を作る（項目が数百ある画面でも速く探せるように）
            _index = BuildIndex();
            if (_index.TryGetValue(id, out cached)) return cached;

            return id.Contains("/") ? FindByPath(id) : null;
        }

        private Dictionary<string, AutomationElement> BuildIndex()
        {
            var index = new Dictionary<string, AutomationElement>();
            var counts = new Dictionary<string, int>();
            foreach (var e in Window.FindAllDescendants())
            {
                string id;
                try { id = e.Properties.AutomationId.ValueOrDefault; }
                catch (ElementNotAvailableException) { continue; }
                if (string.IsNullOrEmpty(id)) continue;

                int count;
                counts.TryGetValue(id, out count);
                counts[id] = ++count;
                index[count == 1 ? id : id + "#" + count] = e;
            }
            return index;
        }

        private AutomationElement FindByPath(string path)
        {
            AutomationElement scope = Window;
            foreach (var part in path.Split('/'))
            {
                var id = part;
                var nth = 1;
                var hash = part.LastIndexOf('#');
                if (hash > 0 && int.TryParse(part.Substring(hash + 1), out nth)) id = part.Substring(0, hash);
                else nth = 1;

                var matches = scope.FindAllDescendants(cf => cf.ByAutomationId(id));
                if (matches.Length < nth) return null;
                scope = matches[nth - 1];
            }
            return scope;
        }

        private static bool IsAlive(AutomationElement element)
        {
            try
            {
                // 閉じた画面の部品は、プロパティを読むと例外になる
                var unused = element.BoundingRectangle;
                return true;
            }
            catch (ElementNotAvailableException) { return false; }
            catch (System.Runtime.InteropServices.COMException) { return false; }
        }

        private bool IsOpen()
        {
            try { return Window.IsAvailable && !Window.IsOffscreen; }
            catch (Exception) { return false; }
        }

        private void ClickCore(string id)
        {
            var element = Find(id);
            Window.SetForeground();
            element.Click();
            Wait.UntilInputIsProcessed();
        }

        private Window FindMessageBox()
        {
            var modal = Window.ModalWindows.FirstOrDefault();
            if (modal != null) return modal;

            // 所有関係が取れない場合：同じプロセスの標準ダイアログ(#32770)を探す
            var session = TestSession.Current;
            if (session == null) return null;
            return session.App.GetAllTopLevelWindows(session.Automation)
                .FirstOrDefault(w => w.ClassName == "#32770");
        }

        private Window FindWindow(string windowId)
        {
            var cf = Window.ConditionFactory;
            var condition = cf.ByControlType(ControlType.Window).And(cf.ByAutomationId(windowId));

            var child = Window.FindFirstChild(condition);
            if (child != null) return child.AsWindow();

            var session = TestSession.Current;
            if (session == null) return null;
            foreach (var top in session.App.GetAllTopLevelWindows(session.Automation))
            {
                if (top.AutomationId == windowId) return top;
                var nested = top.FindFirstChild(condition);
                if (nested != null) return nested.AsWindow();
            }
            return null;
        }

        // ------------------------------------------------------------------
        // レポート用の記録
        // ------------------------------------------------------------------

        /// <summary>操作をエビデンス（レポート）に記録する。</summary>
        protected void Log(string text)
        {
            var session = TestSession.Current;
            if (session != null) session.LogOperation(text);
        }

        /// <summary>レポート用の項目表示：「顧客コード(txtCode)」。項目名は基本値の定義から取る。</summary>
        protected string Describe(string id)
        {
            var label = BaseValues.LabelOf(id);
            return label == null ? id : label + "(" + id + ")";
        }

        private static string Show(string value)
        {
            if (value == null) return "（変更しない）";
            return value.Length == 0 ? "（空）" : "「" + value + "」";
        }
    }
}
