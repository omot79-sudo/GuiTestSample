using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GuiTestKit.Comparison;
using GuiTestKit.Reporting;

namespace GuiTestTool
{
    internal static class Program
    {
        private const string Usage = @"GuiTestTool — GUIテスト用ツール

使い方:
  GuiTestTool compare <比較元フォルダ> <比較先フォルダ> [--out <出力フォルダ>] [--tolerance <0-255>]
      2回分のエビデンスを突き合わせ、差分画像と compare.html を出す。
      例) GuiTestTool compare Evidence\20260926_100000\VB Evidence\20260926_110000\CS
      終了コード: 0 = すべて一致, 1 = 差異あり, 2 = エラー

  GuiTestTool report <実行フォルダ>
      エビデンス（record.json）から report.html を作り直す。
      例) GuiTestTool report Evidence\20260926_100000

  GuiTestTool dump --process <プロセス名|PID> [--window <画面ID|タイトル>] [--delay <秒>]
                   [--class <クラス名>] [--namespace <名前空間>] [--out <出力ファイル>]
      起動中の画面から全項目を吸い出し、基本値つきの Page クラスのひな形を作る。
      画面に基本値を手で入力してから実行すると、その値が基本値になる。
      --delay を付けると、その秒数のあいだに前面にした画面を対象にする。
      例) GuiTestTool dump --process SampleAppVB --window CustomerForm --class 顧客登録Page
";

        private static int Main(string[] args)
        {
            Console.OutputEncoding = new UTF8Encoding(false);
            try
            {
                if (args.Length == 0) return ShowUsage();
                var options = ParseOptions(args.Skip(1).ToArray());
                switch (args[0].ToLowerInvariant())
                {
                    case "compare": return Compare(options);
                    case "report": return Report(options);
                    case "dump": return DumpCommand.Run(options);
                    default: return ShowUsage();
                }
            }
            catch (ToolException ex)
            {
                Console.Error.WriteLine("エラー: " + ex.Message);
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("エラー: " + ex);
                return 2;
            }
        }

        private static int Compare(Options options)
        {
            if (options.Positional.Count != 2) throw new ToolException("比較元フォルダと比較先フォルダを指定してください。");
            var dirA = RequireDirectory(options.Positional[0]);
            var dirB = RequireDirectory(options.Positional[1]);

            // 既定の出力先: Evidence\Compare_日時（比較先フォルダの2つ上 = Evidence）
            var outDir = options.Get("out") ?? Path.Combine(
                Path.GetDirectoryName(Path.GetDirectoryName(Path.GetFullPath(dirB).TrimEnd(Path.DirectorySeparatorChar))),
                "Compare_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(outDir);

            var compareOptions = new CompareOptions { Tolerance = int.Parse(options.Get("tolerance") ?? "0") };
            var results = EvidenceComparer.Compare(dirA, dirB, outDir, compareOptions);
            var report = CompareReportWriter.Write(outDir, dirA, dirB, compareOptions, results);

            var different = results.Count(r => r.Status != CompareStatus.Same);
            Console.WriteLine("テスト {0} 件: 一致 {1} 件 / 差異あり・片方のみ {2} 件", results.Count, results.Count - different, different);
            Console.WriteLine("比較結果: " + report);
            return different == 0 ? 0 : 1;
        }

        private static int Report(Options options)
        {
            if (options.Positional.Count != 1) throw new ToolException("実行フォルダ（Evidence\\実行日時）を指定してください。");
            var path = RunReportWriter.Write(RequireDirectory(options.Positional[0]));
            Console.WriteLine("レポート: " + path);
            return 0;
        }

        private static string RequireDirectory(string path)
        {
            if (!Directory.Exists(path)) throw new ToolException("フォルダがありません: " + path);
            return path;
        }

        private static int ShowUsage()
        {
            Console.WriteLine(Usage);
            return 2;
        }

        private static Options ParseOptions(string[] args)
        {
            var options = new Options();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    if (i + 1 >= args.Length) throw new ToolException(args[i] + " の値がありません。");
                    options.Named[args[i].Substring(2).ToLowerInvariant()] = args[++i];
                }
                else
                {
                    options.Positional.Add(args[i]);
                }
            }
            return options;
        }
    }

    internal class Options
    {
        public readonly List<string> Positional = new List<string>();
        public readonly Dictionary<string, string> Named = new Dictionary<string, string>();

        public string Get(string name)
        {
            string value;
            return Named.TryGetValue(name, out value) ? value : null;
        }
    }

    internal class ToolException : Exception
    {
        public ToolException(string message) : base(message)
        {
        }
    }
}
