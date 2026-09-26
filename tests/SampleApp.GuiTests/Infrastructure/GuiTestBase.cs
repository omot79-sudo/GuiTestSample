using System;
using System.Collections.Generic;
using System.IO;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Capturing;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SampleApp.GuiTests.Pages;

namespace SampleApp.GuiTests.Infrastructure
{
    /// <summary>
    /// GUIテストの共通基底クラス。
    /// 各テストの前に EXE を起動し、後で終了する。キャプチャ保存の仕組みもここに置く。
    /// </summary>
    public abstract class GuiTestBase
    {
        // テスト実行1回分で共通のフォルダ名（例: Evidence\20260926_171500\...）
        private static readonly string RunStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        private int _step;

        public TestContext TestContext { get; set; }

        protected Application App { get; private set; }
        protected UIA3Automation Automation { get; private set; }
        protected MainWindowPage Page { get; private set; }

        /// <summary>DataRow のケース番号など、エビデンスのフォルダ名に付ける識別子。</summary>
        protected string CaseId { get; set; }

        /// <summary>runsettings の中で、テスト対象EXEのパスを持つパラメータ名。</summary>
        protected abstract string TargetExeParameter { get; }

        [TestInitialize]
        public void LaunchApp()
        {
            var exePath = ResolvePath(TargetExeParameter);
            Assert.IsTrue(File.Exists(exePath),
                "テスト対象の EXE が見つかりません: " + exePath + Environment.NewLine + "先にソリューションをビルドしてください。");

            App = Application.Launch(exePath);
            Automation = new UIA3Automation();

            var window = App.GetMainWindow(Automation, TimeSpan.FromSeconds(10));
            Assert.IsNotNull(window, "メイン画面が表示されませんでした。");
            window.Focus();

            Page = new MainWindowPage(window);
            _step = 0;
        }

        [TestCleanup]
        public void CloseApp()
        {
            try
            {
                // 失敗したテストは、終了直前の画面を必ず残す
                if (TestContext.CurrentTestOutcome != UnitTestOutcome.Passed && Page != null)
                {
                    Snap("失敗時の画面");
                }
            }
            finally
            {
                if (App != null)
                {
                    App.Close();
                    if (!App.HasExited) App.Kill();
                    App.Dispose();
                }
                if (Automation != null) Automation.Dispose();
            }
        }

        /// <summary>
        /// 画面キャプチャを保存し、テスト結果(.trx)にも添付する。
        /// element を省略するとメイン画面全体を撮る。
        /// 保存先: Evidence\実行日時\テストクラス名\テスト名[_ケース番号]\01_ラベル.png
        /// </summary>
        protected string Snap(string label, AutomationElement element = null)
        {
            _step++;
            var testFolder = CaseId == null ? TestContext.TestName : TestContext.TestName + "_" + CaseId;
            var dir = Path.Combine(ResolvePath("EvidenceDir"), RunStamp, GetType().Name, testFolder);
            Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, string.Format("{0:00}_{1}.png", _step, label));
            Capture.Element(element ?? Page.Window).ToFile(path);
            TestContext.AddResultFile(path);
            return path;
        }

        /// <summary>runsettings のパラメータを読み、相対パスならテスト出力フォルダ基準の絶対パスにする。</summary>
        private string ResolvePath(string parameterName)
        {
            object raw = null;
            try { raw = TestContext.Properties[parameterName]; }
            catch (KeyNotFoundException) { }

            var value = raw as string;
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    "runsettings のパラメータ '" + parameterName + "' がありません。gui.runsettings を指定して実行してください。");
            }

            return Path.IsPathRooted(value)
                ? value
                : Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, value));
        }
    }
}
