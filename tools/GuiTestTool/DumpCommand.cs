using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using GuiTestKit.Automation;
using GuiTestKit.Evidence;

namespace GuiTestTool
{
    /// <summary>
    /// 起動中の画面から全項目を吸い出し、Page クラスのひな形を作る。
    /// 250画面分の「項目ID・項目名・基本値」を人が書かずに済ませるためのもの。
    /// </summary>
    internal static class DumpCommand
    {
        public static int Run(Options options)
        {
            var processArg = options.Get("process");
            if (processArg == null) throw new ToolException("--process でプロセス名か PID を指定してください。");

            var process = FindProcess(processArg);
            using (var automation = new UIA3Automation())
            using (var app = Application.Attach(process))
            {
                var window = SelectWindow(app, automation, options);
                if (window == null) return 2;

                Console.WriteLine("対象画面: {0}（画面ID: {1}）", window.Title, window.AutomationId);
                var controls = ControlAccess.ReadAll(window, CustomControls.Handlers, window.BoundingRectangle.Location);
                PrintTable(controls);

                var className = options.Get("class") ?? SafeIdentifier((string.IsNullOrEmpty(window.AutomationId) ? "画面" : window.AutomationId) + "Page");
                var ns = options.Get("namespace") ?? "Product.GuiTests.Pages";
                var outPath = options.Get("out") ?? className + ".cs";
                var code = Generate(className, ns, window, process.ProcessName, controls);
                File.WriteAllText(outPath, code, new UTF8Encoding(true));
                Console.WriteLine();
                Console.WriteLine("Page クラスのひな形: " + Path.GetFullPath(outPath));
            }
            return 0;
        }

        private static Process FindProcess(string arg)
        {
            int pid;
            if (int.TryParse(arg, out pid)) return Process.GetProcessById(pid);

            var name = arg.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? arg.Substring(0, arg.Length - 4) : arg;
            var processes = Process.GetProcessesByName(name);
            if (processes.Length == 0) throw new ToolException("プロセスが見つかりません: " + arg + "（対象アプリを起動してから実行してください）");
            if (processes.Length > 1) throw new ToolException("同じ名前のプロセスが複数あります。--process に PID を指定してください: "
                + string.Join(", ", processes.Select(p => p.Id)));
            return processes[0];
        }

        private static Window SelectWindow(Application app, UIA3Automation automation, Options options)
        {
            var delay = options.Get("delay");
            if (delay != null)
            {
                Console.WriteLine("{0} 秒以内に、吸い出したい画面をクリックして前面にしてください…", delay);
                Thread.Sleep(int.Parse(delay) * 1000);
                var hWnd = NativeForeground.Get();
                var fg = automation.FromHandle(hWnd);
                if (fg.Properties.ProcessId.ValueOrDefault != app.ProcessId) throw new ToolException("前面の画面が対象アプリのものではありません。");
                return fg.AsWindow();
            }

            var windows = AllWindows(app, automation);
            var key = options.Get("window");
            if (key != null)
            {
                var match = windows.FirstOrDefault(w => w.AutomationId == key || w.Title == key);
                if (match == null) throw new ToolException("画面が見つかりません: " + key + Environment.NewLine + ListWindows(windows));
                return match;
            }
            if (windows.Count == 1) return windows[0];

            Console.Error.WriteLine("画面が複数開いています。--window で画面ID（またはタイトル）を指定してください。");
            Console.Error.WriteLine(ListWindows(windows));
            return null;
        }

        /// <summary>トップレベルの画面と、そこから開いている子画面（モーダル画面など）すべて。</summary>
        private static List<Window> AllWindows(Application app, UIA3Automation automation)
        {
            var result = new List<Window>();
            var queue = new Queue<AutomationElement>(app.GetAllTopLevelWindows(automation));
            while (queue.Count > 0)
            {
                var w = queue.Dequeue();
                result.Add(w.AsWindow());
                foreach (var child in w.FindAllChildren(cf => cf.ByControlType(ControlType.Window))) queue.Enqueue(child);
            }
            return result;
        }

        private static string ListWindows(IEnumerable<Window> windows)
        {
            return string.Join(Environment.NewLine, windows.Select(w => "  画面ID: " + w.AutomationId + "　タイトル: " + w.Title));
        }

        private static void PrintTable(List<ControlSnapshot> controls)
        {
            Console.WriteLine();
            Console.WriteLine("{0,-24} {1,-12} {2,-20} {3}", "項目ID", "種類", "項目名", "値");
            foreach (var c in controls)
            {
                Console.WriteLine("{0,-24} {1,-12} {2,-20} {3}{4}", c.Id, c.Type, c.Label, OneLine(c.Value), c.Enabled ? "" : "（無効）");
            }
        }

        // ------------------------------------------------------------------
        // コード生成
        // ------------------------------------------------------------------

        private static string Generate(string className, string ns, Window window, string processName, List<ControlSnapshot> controls)
        {
            var inputs = controls.Where(c => ControlAccess.IsInputType(c.Type) || c.Type == "Custom")
                                 .Where(c => c.Type != "RadioButton" || c.Value == ScreenValues.ON)
                                 .ToList();
            var others = controls.Except(inputs).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using FlaUI.Core.AutomationElements;");
            sb.AppendLine("using GuiTestKit.Automation;");
            sb.AppendLine();
            sb.AppendLine("namespace " + ns);
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// " + Escape(window.Title) + "");
            sb.AppendFormat("    /// GuiTestTool dump で自動生成（{0:yyyy/MM/dd HH:mm}、{1}）。基本値は吸い出した時点の画面の値。{2}", DateTime.Now, processName, Environment.NewLine);
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public class " + className + " : ScreenPageBase");
            sb.AppendLine("    {");
            sb.AppendLine("        public const string WindowId = \"" + Escape(window.AutomationId) + "\";");
            sb.AppendLine();
            sb.AppendLine("        public " + className + "(Window window) : base(window)");
            sb.AppendLine("        {");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>全項目を正しく入力した状態。テストケースではここからの差分だけを書く。</summary>");
            sb.AppendLine("        public override ScreenValues BaseValues { get; } = new ScreenValues");
            sb.AppendLine("        {");
            foreach (var c in inputs)
            {
                var comment = new List<string>();
                if (c.Type == "Custom") comment.Add("市販部品: 入力はセル単位で行う");
                if (!c.Enabled) comment.Add("吸い出し時は無効");
                var value = c.Type == "Custom" ? "null" : "\"" + Escape(c.Value) + "\"";
                sb.AppendFormat("            {{ \"{0}\", {1}, \"{2}\" }},{3}{4}",
                    Escape(c.Id), value, Escape(c.Label ?? ""), comment.Count > 0 ? "   // " + string.Join("、", comment) : "", Environment.NewLine);
            }
            sb.AppendLine("        };");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>実行するたびに変わる項目（日時・採番など）。VB/C# の比較で値も画像も比較しない。</summary>");
            sb.AppendLine("        public override IEnumerable<string> CompareExcludedIds");
            sb.AppendLine("        {");
            sb.AppendLine("            get { return new string[] { }; }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- 表示項目・ボタン（参考）。必要な操作だけメソッドにする。例:");
            sb.AppendLine("        //   public MessageBoxPage 登録() { return ClickAndWaitMessageBox(\"btnRegister\"); }");
            foreach (var c in others)
            {
                sb.AppendFormat("        // {0,-20} {1,-10} {2}{3}", c.Id, c.Type, OneLine(c.Value), Environment.NewLine);
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string Escape(string text)
        {
            return (text ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private static string OneLine(string text)
        {
            var line = (text ?? string.Empty).Replace("\r", "").Replace("\n", " ");
            return line.Length > 60 ? line.Substring(0, 60) + "…" : line;
        }

        private static string SafeIdentifier(string name)
        {
            var sb = new StringBuilder();
            foreach (var ch in name) sb.Append(char.IsLetterOrDigit(ch) || ch == '_' ? ch : '_');
            if (sb.Length == 0 || char.IsDigit(sb[0])) sb.Insert(0, '_');
            return sb.ToString();
        }
    }

    internal static class NativeForeground
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr Get();
    }
}
