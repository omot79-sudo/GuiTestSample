using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using GuiTestKit.Evidence;

namespace GuiTestKit.Automation
{
    /// <summary>
    /// 部品の種類ごとの値の読み書きと、画面の全項目の吸い出し。
    /// ScreenPageBase・キャプチャ・吸い出しツール（GuiTestTool dump）で共通に使う。
    /// </summary>
    public static class ControlAccess
    {
        /// <summary>中を辿らずに、それ自体を1項目として扱う部品。</summary>
        private static readonly HashSet<ControlType> LeafTypes = new HashSet<ControlType>
        {
            ControlType.Edit, ControlType.ComboBox, ControlType.CheckBox, ControlType.RadioButton,
            ControlType.Text, ControlType.Button, ControlType.SplitButton, ControlType.Hyperlink,
            ControlType.Spinner, ControlType.Slider, ControlType.ProgressBar,
            ControlType.Table, ControlType.DataGrid, ControlType.List, ControlType.Tree, ControlType.Document,
        };

        /// <summary>値を入力できる部品（基本値の対象）。</summary>
        public static bool IsInputType(string type)
        {
            return type == "Edit" || type == "ComboBox" || type == "CheckBox" || type == "RadioButton" || type == "Spinner";
        }

        // ------------------------------------------------------------------
        // 読み取り
        // ------------------------------------------------------------------

        public static string Read(AutomationElement element, IEnumerable<ICustomControlHandler> handlers)
        {
            var handler = FindHandler(element, handlers);
            if (handler != null) return handler.Read(element);

            try
            {
                switch (element.ControlType)
                {
                    case ControlType.Edit:
                        if (element.Properties.IsPassword.ValueOrDefault) return "(パスワード)";
                        return ReadValuePattern(element) ?? element.Name;

                    case ControlType.ComboBox:
                        var combo = element.AsComboBox();
                        var value = ReadValuePattern(element);
                        if (!string.IsNullOrEmpty(value)) return value;
                        var selected = combo.SelectedItem;
                        return selected == null ? string.Empty : selected.Text;

                    case ControlType.CheckBox:
                        var state = element.AsCheckBox().ToggleState;
                        return state == ToggleState.On ? ScreenValues.ON : state == ToggleState.Off ? ScreenValues.OFF : "不定";

                    case ControlType.RadioButton:
                        return element.AsRadioButton().IsChecked ? ScreenValues.ON : ScreenValues.OFF;

                    case ControlType.Text:
                    case ControlType.Button:
                    case ControlType.SplitButton:
                    case ControlType.Hyperlink:
                        return element.Name;

                    default:
                        var generic = ReadValuePattern(element);
                        if (generic != null) return generic;
                        var range = element.Patterns.RangeValue.PatternOrDefault;
                        if (range != null) return range.Value.ValueOrDefault.ToString();
                        return "(未対応: " + element.ControlType + ")";
                }
            }
            catch (Exception ex)
            {
                return "(取得エラー: " + ex.GetType().Name + ")";
            }
        }

        private static string ReadValuePattern(AutomationElement element)
        {
            var pattern = element.Patterns.Value.PatternOrDefault;
            return pattern == null ? null : pattern.Value.ValueOrDefault;
        }

        // ------------------------------------------------------------------
        // 書き込み
        // ------------------------------------------------------------------

        public static void Write(AutomationElement element, string value, InputMode mode, IEnumerable<ICustomControlHandler> handlers)
        {
            var handler = FindHandler(element, handlers);
            if (handler != null)
            {
                handler.Write(element, value);
                return;
            }

            switch (element.ControlType)
            {
                case ControlType.Edit:
                    if (mode == InputMode.Keyboard) TypeByKeyboard(element, value);
                    else element.AsTextBox().Text = value;
                    break;

                case ControlType.ComboBox:
                    var combo = element.AsComboBox();
                    if (combo.IsEditable)
                    {
                        combo.EditableText = value;
                    }
                    else
                    {
                        var item = combo.Select(value);
                        combo.Collapse();
                        if (item == null) throw new InvalidOperationException("コンボボックスに「" + value + "」がありません。");
                    }
                    break;

                case ControlType.CheckBox:
                    element.AsCheckBox().IsChecked = ToBool(value);
                    break;

                case ControlType.RadioButton:
                    // OFF はグループ内の別のラジオボタンを ON にすることで実現するため、何もしない
                    if (!ToBool(value)) break;
                    var radio = element.AsRadioButton();
                    if (radio.Patterns.SelectionItem.IsSupported) radio.IsChecked = true;
                    else radio.Click();
                    break;

                default:
                    var pattern = element.Patterns.Value.PatternOrDefault;
                    if (pattern == null)
                    {
                        throw new NotSupportedException(element.ControlType + " には値を入力できません。ICustomControlHandler で対応してください。");
                    }
                    pattern.SetValue(value);
                    break;
            }
        }

        /// <summary>実際にキーを打鍵して入力する（キー入力・フォーカス移動のイベントで動く画面用）。</summary>
        private static void TypeByKeyboard(AutomationElement element, string value)
        {
            element.Focus();
            // 単一行の TextBox は Ctrl+A で全選択できないことがあるので Home → Shift+End で選択して消す
            Keyboard.Type(VirtualKeyShort.HOME);
            Keyboard.TypeSimultaneously(VirtualKeyShort.SHIFT, VirtualKeyShort.END);
            Keyboard.Type(VirtualKeyShort.DELETE);
            if (value.Length > 0) Keyboard.Type(value);
            Wait.UntilInputIsProcessed();
        }

        private static bool ToBool(string value)
        {
            if (value == ScreenValues.ON) return true;
            if (value == ScreenValues.OFF) return false;
            throw new ArgumentException("チェックボックス・ラジオボタンの値は ON / OFF で指定してください: " + value);
        }

        private static ICustomControlHandler FindHandler(AutomationElement element, IEnumerable<ICustomControlHandler> handlers)
        {
            return handlers == null ? null : handlers.FirstOrDefault(h => h.CanHandle(element));
        }

        // ------------------------------------------------------------------
        // 全項目の吸い出し
        // ------------------------------------------------------------------

        /// <summary>
        /// 画面（ウィンドウ）内の、AutomationId を持つ全項目の状態を読み取る。
        /// 位置は origin（比較用画像の左上）からの相対座標で記録する。
        /// 同じ AutomationId が複数あるときは2つ目以降を "ID#2", "ID#3" … とする。
        /// </summary>
        public static List<ControlSnapshot> ReadAll(AutomationElement root, IEnumerable<ICustomControlHandler> handlers, Point origin)
        {
            var handlerList = handlers == null ? new List<ICustomControlHandler>() : handlers.ToList();
            var found = new List<Found>();
            var texts = new List<Found>();
            Walk(root, null, handlerList, found, texts);

            var counts = new Dictionary<string, int>();
            var result = new List<ControlSnapshot>();
            foreach (var f in found)
            {
                int count;
                counts.TryGetValue(f.Id, out count);
                counts[f.Id] = ++count;

                var type = f.Element.ControlType.ToString();
                result.Add(new ControlSnapshot
                {
                    Id = count == 1 ? f.Id : f.Id + "#" + count,
                    Type = FindHandler(f.Element, handlerList) != null ? "Custom" : type,
                    Label = GuessLabel(f, texts),
                    Value = Read(f.Element, handlerList),
                    Enabled = SafeIsEnabled(f.Element),
                    X = f.Bounds.X - origin.X,
                    Y = f.Bounds.Y - origin.Y,
                    Width = f.Bounds.Width,
                    Height = f.Bounds.Height,
                });
            }
            return result;
        }

        private class Found
        {
            public AutomationElement Element;
            public string Id;
            public string Group;
            public Rectangle Bounds;
        }

        private static void Walk(AutomationElement parent, string group, List<ICustomControlHandler> handlers, List<Found> found, List<Found> texts)
        {
            AutomationElement[] children;
            try { children = parent.FindAllChildren(); }
            catch (Exception) { return; }

            foreach (var child in children)
            {
                ControlType type;
                string id;
                try
                {
                    type = child.ControlType;
                    id = child.Properties.AutomationId.ValueOrDefault;
                }
                catch (Exception)
                {
                    continue;
                }

                // 子画面(ダイアログ)は別画面として扱う
                if (type == ControlType.Window) continue;

                var isHandled = handlers.Any(h => h.CanHandle(child));
                var hasId = !string.IsNullOrEmpty(id);
                var item = new Found { Element = child, Id = id, Group = group, Bounds = SafeBounds(child) };

                if (type == ControlType.Text) texts.Add(item);

                if (isHandled || LeafTypes.Contains(type))
                {
                    if (hasId || isHandled) found.Add(item);
                    continue;
                }

                // GroupBox 等の入れ物：中の部品の項目名に枠の名前を付けるため、名前を引き継ぐ
                var childGroup = type == ControlType.Group && !string.IsNullOrEmpty(child.Name) ? child.Name : group;
                Walk(child, childGroup, handlers, found, texts);
            }
        }

        /// <summary>項目名の推定：チェックボックス等は自分の表示名、入力欄は左隣のラベル。</summary>
        private static string GuessLabel(Found target, List<Found> texts)
        {
            var type = target.Element.ControlType;
            string label = null;
            if (type == ControlType.CheckBox || type == ControlType.RadioButton || type == ControlType.Button)
            {
                label = target.Element.Name;
            }
            else if (type != ControlType.Text)
            {
                var b = target.Bounds;
                var centerY = b.Top + b.Height / 2;
                var nearest = texts
                    .Where(t => t.Bounds.Right <= b.Left + 4 && b.Left - t.Bounds.Right < 300
                             && t.Bounds.Top - 4 <= centerY && centerY <= t.Bounds.Bottom + 4)
                    .OrderBy(t => b.Left - t.Bounds.Right)
                    .FirstOrDefault();
                if (nearest != null) label = nearest.Element.Name;
            }

            if (string.IsNullOrEmpty(label)) return null;
            return target.Group == null ? label : target.Group + "/" + label;
        }

        private static Rectangle SafeBounds(AutomationElement element)
        {
            try { return element.BoundingRectangle; }
            catch (Exception) { return Rectangle.Empty; }
        }

        private static bool SafeIsEnabled(AutomationElement element)
        {
            try { return element.IsEnabled; }
            catch (Exception) { return false; }
        }
    }

    /// <summary>テキストボックスへの入力方法。</summary>
    public enum InputMode
    {
        /// <summary>値を直接設定する（速い。IMEやキー入力イベントに左右されない）。</summary>
        SetValue,

        /// <summary>実際にキーを打鍵する（KeyPress・Validating 等のイベントで動く画面用）。</summary>
        Keyboard,
    }
}
