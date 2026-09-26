using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using GuiTestKit.Automation;
using GuiTestKit.Evidence;
using GuiTestKit.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GuiTestKit.Testing
{
    /// <summary>
    /// GUIテストの共通基底クラス。
    /// ・テストごとに対象EXEを起動・終了する（対象は runsettings の Target = VB / CS で切り替え）
    /// ・テストデータ初期化コマンドの実行
    /// ・操作／確認／キャプチャをエビデンスとして記録し、HTMLレポートを出す
    /// </summary>
    public abstract class GuiTestBase
    {
        // テスト実行1回分で共通のフォルダ名（例: Evidence\20260926_171500\...）
        private static readonly string RunStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        private static string _runDir;
        private static uint? _originalCaretBlink;

        static GuiTestBase()
        {
            NativeMethods.EnableDpiAwareness();
        }

        public TestContext TestContext { get; set; }

        protected Application App { get; private set; }
        protected UIA3Automation Automation { get; private set; }
        protected Window MainWindow { get; private set; }
        protected TestSession Session { get; private set; }

        /// <summary>テスト対象（runsettings の Target。例: VB / CS）。</summary>
        protected string Target { get; private set; }

        /// <summary>DataRow のケース番号など。エビデンスのフォルダ名に付く。キャプチャより前に設定する。</summary>
        protected string CaseId
        {
            get { return Session.Record.CaseId; }
            set { Session.SetCaseId(value); }
        }

        [TestInitialize]
        public void StartApplication()
        {
            Target = GetParameter("Target");
            var exePath = ResolvePath(GetParameter("TargetExe" + Target));
            Assert.IsTrue(File.Exists(exePath),
                "テスト対象の EXE が見つかりません: " + exePath + Environment.NewLine + "先にビルドするか、runsettings のパスを確認してください。");

            _runDir = _runDir ?? Path.Combine(ResolvePath(GetParameter("EvidenceDir")), RunStamp);
            var resetMessage = RunResetCommand();

            // 業務アプリは設定ファイルを作業フォルダから読むことが多いので、EXE のフォルダで起動する
            App = Application.Launch(new ProcessStartInfo(exePath) { WorkingDirectory = Path.GetDirectoryName(exePath) });
            Automation = new UIA3Automation();
            MainWindow = App.GetMainWindow(Automation, TimeSpan.FromSeconds(30));
            Assert.IsNotNull(MainWindow, "メイン画面が表示されませんでした。");
            MainWindow.SetForeground();

            DisableCaretBlink();

            Session = new TestSession(App, Automation, MainWindow, _runDir, new TestRecord
            {
                Target = Target,
                Scenario = GetType().Name,
                TestName = TestContext.TestName,
                StartedAt = Now(),
            });
            TestSession.Current = Session;
            if (resetMessage != null) Session.LogOperation(resetMessage);
        }

        [TestCleanup]
        public void StopApplication()
        {
            try
            {
                if (Session != null)
                {
                    var passed = TestContext.CurrentTestOutcome == UnitTestOutcome.Passed;
                    // 失敗したテストは、終了直前の画面とデスクトップ全体を必ず残す
                    if (!passed)
                    {
                        try { Session.Snap("失敗時の画面", includeDesktop: true); }
                        catch (Exception ex) { Session.LogOperation("失敗時の画面を撮れませんでした: " + ex.Message); }
                    }
                    Session.Record.Outcome = passed ? "合格" : "不合格";
                    Session.Record.FinishedAt = Now();
                    Session.Save();
                    TestContext.AddResultFile(Path.Combine(Session.TestDir, "record.json"));
                }
            }
            finally
            {
                TestSession.Current = null;
                RestoreCaretBlink();
                CloseApplication();
            }
        }

        /// <summary>
        /// 実行した全テストのHTMLレポートを出す。
        /// テストプロジェクトの [AssemblyCleanup] から呼ぶ（MSTest の仕様で、基盤側には置けないため）。
        /// </summary>
        public static void WriteRunReport()
        {
            if (_runDir != null && Directory.Exists(_runDir)) RunReportWriter.Write(_runDir);
        }

        // ------------------------------------------------------------------
        // テストケースから使う操作
        // ------------------------------------------------------------------

        /// <summary>前面の画面をキャプチャする（人が見る用・比較用・全項目の値）。</summary>
        protected void Snap(string label)
        {
            Session.Snap(label);
        }

        /// <summary>レポートにメモを残す。</summary>
        protected void Log(string text)
        {
            Session.LogOperation(text);
        }

        /// <summary>期待値と一致するか確認する（結果はレポートにも出る）。</summary>
        protected void Check(string item, string expected, string actual)
        {
            var passed = expected == actual;
            Session.LogCheck(item, expected, actual, passed);
            if (!passed) Assert.Fail("[" + item + "] 期待値: " + expected + " / 実際: " + actual);
        }

        protected void Check(string item, bool expected, bool actual)
        {
            Check(item, ToText(expected), ToText(actual));
        }

        /// <summary>画面の表示が変わるのを待ってから確認する（計算結果の反映待ちなど）。</summary>
        protected void CheckEventually(string item, string expected, Func<string> actual, TimeSpan? timeout = null)
        {
            string last = null;
            Retry.WhileFalse(() => (last = actual()) == expected, timeout ?? ScreenPageBase.DefaultTimeout);
            Check(item, expected, last);
        }

        // ------------------------------------------------------------------
        // 内部処理
        // ------------------------------------------------------------------

        /// <summary>runsettings の ResetCommand（テストデータ初期化）をテストごとに実行する。</summary>
        private string RunResetCommand()
        {
            var command = GetParameter("ResetCommand", required: false);
            if (string.IsNullOrWhiteSpace(command)) return null;

            var psi = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using (var process = Process.Start(psi))
            {
                var output = process.StandardOutput.ReadToEndAsync();
                var error = process.StandardError.ReadToEndAsync();
                if (!process.WaitForExit(5 * 60 * 1000))
                {
                    process.Kill();
                    Assert.Fail("テストデータ初期化コマンドが5分で終わりませんでした: " + command);
                }
                if (process.ExitCode != 0)
                {
                    Assert.Fail("テストデータ初期化コマンドが失敗しました（終了コード " + process.ExitCode + "）: " + command
                        + Environment.NewLine + output.Result + error.Result);
                }
            }
            return "テストデータを初期化: " + command;
        }

        private void CloseApplication()
        {
            if (App != null)
            {
                try
                {
                    // まず普通に閉じる（アプリの後始末を走らせる）。閉じなければ強制終了
                    App.Close(false);
                    using (var process = Process.GetProcessById(App.ProcessId))
                    {
                        if (!process.WaitForExit(3000)) process.Kill();
                    }
                }
                catch (ArgumentException)
                {
                    // すでに終了している
                }
                catch (InvalidOperationException)
                {
                    // すでに終了している
                }
                App.Dispose();
                App = null;
            }
            if (Automation != null)
            {
                Automation.Dispose();
                Automation = null;
            }
        }

        /// <summary>
        /// カーソル(キャレット)の点滅を止める（常に表示）。撮るタイミングで写ったり写らなかったりするのを防ぐ。
        /// Windows 全体の設定なので、テスト後に元に戻す。
        /// </summary>
        private static void DisableCaretBlink()
        {
            if (_originalCaretBlink == null) _originalCaretBlink = NativeMethods.GetCaretBlink();
            NativeMethods.SetCaretBlink(uint.MaxValue);
        }

        private static void RestoreCaretBlink()
        {
            if (_originalCaretBlink != null) NativeMethods.SetCaretBlink(_originalCaretBlink.Value);
        }

        private string GetParameter(string name, bool required = true)
        {
            object raw = null;
            try { raw = TestContext.Properties[name]; }
            catch (KeyNotFoundException) { }

            var value = raw as string;
            if (required && string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    "runsettings のパラメータ '" + name + "' がありません。gui.runsettings を指定して実行してください。");
            }
            return value;
        }

        /// <summary>相対パスはテストの出力フォルダ（bin\Debug\net462）基準の絶対パスにする。</summary>
        private static string ResolvePath(string value)
        {
            return Path.IsPathRooted(value)
                ? value
                : Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value));
        }

        private static string ToText(bool value)
        {
            return value ? "はい" : "いいえ";
        }

        private static string Now()
        {
            return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
