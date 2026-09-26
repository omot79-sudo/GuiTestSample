using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using GuiTestKit.Automation;
using GuiTestKit.Evidence;

namespace GuiTestKit.Testing
{
    /// <summary>
    /// 実行中のテスト1件分の状態（起動中のアプリ、エビデンスの記録）。
    /// GUIテストは並列実行しないので、実行中のテストは常に1件（Current）。
    /// Page クラスは Current を通して操作ログを記録する。
    /// </summary>
    public sealed class TestSession
    {
        /// <summary>マウス移動後、ボタンのハイライト等が消えるのを待つ時間。</summary>
        public static int CaptureSettleMilliseconds = 200;

        private readonly List<ScreenPageBase> _pages = new List<ScreenPageBase>();
        private readonly string _runDir;
        private int _captureNo;
        private string _testDir;

        internal TestSession(Application app, AutomationBase automation, Window mainWindow, string runDir, TestRecord record)
        {
            App = app;
            Automation = automation;
            MainWindow = mainWindow;
            Record = record;
            _runDir = runDir;
        }

        public static TestSession Current { get; internal set; }

        public Application App { get; private set; }
        public AutomationBase Automation { get; private set; }
        public Window MainWindow { get; private set; }
        public TestRecord Record { get; private set; }

        /// <summary>このテストのエビデンスフォルダ: 実行日時\対象\シナリオ\テスト名[_ケース番号]</summary>
        public string TestDir
        {
            get
            {
                if (_testDir == null)
                {
                    var folder = Record.CaseId == null ? Record.TestName : Record.TestName + "_" + Record.CaseId;
                    _testDir = Path.Combine(_runDir, SafeName(Record.Target), SafeName(Record.Scenario), SafeName(folder));
                }
                return _testDir;
            }
        }

        internal void SetCaseId(string caseId)
        {
            if (_testDir != null) throw new InvalidOperationException("CaseId はキャプチャを撮る前に設定してください。");
            Record.CaseId = caseId;
        }

        internal void RegisterPage(ScreenPageBase page)
        {
            _pages.Add(page);
        }

        // ------------------------------------------------------------------
        // 記録
        // ------------------------------------------------------------------

        public void LogOperation(string text)
        {
            Record.Steps.Add(new StepRecord { Kind = StepKind.Operation, Text = text });
        }

        public void LogCheck(string item, string expected, string actual, bool passed)
        {
            Record.Steps.Add(new StepRecord
            {
                Kind = StepKind.Check,
                Text = item,
                Expected = expected,
                Actual = actual,
                Passed = passed,
            });
        }

        /// <summary>
        /// 前面の画面をキャプチャする。1回で次の3つを保存する。
        /// ・人が見る用: 前面の画面＋その親画面（タイトルバー込み）
        /// ・比較用　　: 前面の画面の中身部分だけ（タイトルバー・枠なし）
        /// ・画面の全項目の値（record.json に記録）
        /// </summary>
        public StepRecord Snap(string label, bool includeDesktop = false)
        {
            var front = FindFrontWindow();
            var hWnd = front.Properties.NativeWindowHandle.Value;

            var evidenceBounds = NativeMethods.GetVisibleWindowBounds(hWnd);
            for (var owner = NativeMethods.GetOwner(hWnd); owner != IntPtr.Zero; owner = NativeMethods.GetOwner(owner))
            {
                evidenceBounds = Rectangle.Union(evidenceBounds, NativeMethods.GetVisibleWindowBounds(owner));
            }

            PrepareForCapture(front, evidenceBounds);
            var clientBounds = NativeMethods.GetClientBoundsOnScreen(hWnd);

            _captureNo++;
            var fileName = string.Format("{0:00}_{1}.png", _captureNo, SafeName(label));
            Directory.CreateDirectory(Path.Combine(TestDir, "compare"));

            var step = new StepRecord
            {
                Kind = StepKind.Capture,
                Text = label,
                CaptureNo = _captureNo,
                WindowId = front.Properties.AutomationId.ValueOrDefault,
                WindowTitle = front.Title,
                EvidenceImage = fileName,
                CompareImage = "compare/" + fileName,
            };
            Capture.Rectangle(evidenceBounds).ToFile(Path.Combine(TestDir, fileName));
            Capture.Rectangle(clientBounds).ToFile(Path.Combine(TestDir, "compare", fileName));

            if (includeDesktop)
            {
                step.DesktopImage = string.Format("{0:00}_{1}_デスクトップ.png", _captureNo, SafeName(label));
                Capture.Rectangle(NativeMethods.GetVirtualScreen()).ToFile(Path.Combine(TestDir, step.DesktopImage));
            }

            var page = _pages.LastOrDefault(p => SameWindow(p.Window, hWnd));
            var handlers = page != null ? page.CustomHandlers : CustomControls.Handlers;
            step.Controls = ControlAccess.ReadAll(front, handlers, clientBounds.Location);
            if (page != null) step.Masks = ResolveMasks(page, clientBounds.Location);

            Record.Steps.Add(step);
            Save();
            return step;
        }

        public void Save()
        {
            Directory.CreateDirectory(TestDir);
            JsonFile.Write(Path.Combine(TestDir, "record.json"), Record);
        }

        // ------------------------------------------------------------------
        // キャプチャの準備
        // ------------------------------------------------------------------

        /// <summary>
        /// 前面の画面を決める。基本は「アクティブなウィンドウ」。
        /// 別アプリが前面にある場合は、メイン画面から開いているモーダル画面を辿った一番上。
        /// </summary>
        private Window FindFrontWindow()
        {
            var hWnd = NativeMethods.GetForegroundWindowOf(App.ProcessId);
            if (hWnd != IntPtr.Zero)
            {
                var element = Automation.FromHandle(hWnd);
                if (element.ControlType == ControlType.Window) return element.AsWindow();
            }

            var window = MainWindow;
            while (true)
            {
                var modal = window.ModalWindows.FirstOrDefault();
                if (modal == null) return window;
                window = modal;
            }
        }

        /// <summary>
        /// 比較のノイズを減らす: 対象を最前面に出し、マウスを画面の外へ逃がす。
        /// （フォーカスは動かさない。フォーカス移動で入力チェック等のイベントが走る画面があるため）
        /// </summary>
        private static void PrepareForCapture(Window front, Rectangle bounds)
        {
            try { front.SetForeground(); } catch (Exception) { }

            var screen = NativeMethods.GetVirtualScreen();
            var right = new Point(bounds.Right + 40, bounds.Top + 10);
            var left = new Point(bounds.Left - 40, bounds.Top + 10);
            Mouse.Position = screen.Contains(right) ? right : screen.Contains(left) ? left : new Point(screen.Left + 1, screen.Top + 1);

            Wait.UntilInputIsProcessed();
            Thread.Sleep(CaptureSettleMilliseconds);
        }

        private static List<MaskArea> ResolveMasks(ScreenPageBase page, Point origin)
        {
            var masks = new List<MaskArea>();
            foreach (var id in page.CompareExcludedIds)
            {
                var element = page.FindOrNull(id);
                var bounds = element == null ? Rectangle.Empty : element.BoundingRectangle;
                masks.Add(new MaskArea
                {
                    Id = id,
                    X = bounds.X - origin.X,
                    Y = bounds.Y - origin.Y,
                    Width = bounds.Width,
                    Height = bounds.Height,
                });
            }
            return masks;
        }

        private static bool SameWindow(Window window, IntPtr hWnd)
        {
            try { return window.Properties.NativeWindowHandle.ValueOrDefault == hWnd; }
            catch (Exception) { return false; }
        }

        internal static string SafeName(string name)
        {
            var chars = name.ToCharArray();
            var invalid = Path.GetInvalidFileNameChars();
            for (var i = 0; i < chars.Length; i++)
            {
                if (Array.IndexOf(invalid, chars[i]) >= 0 || chars[i] == '#' || chars[i] == '%') chars[i] = '_';
            }
            return new string(chars);
        }
    }
}
