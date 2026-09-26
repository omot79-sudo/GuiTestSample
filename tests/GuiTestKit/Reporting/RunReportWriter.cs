using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GuiTestKit.Evidence;

namespace GuiTestKit.Reporting
{
    /// <summary>
    /// 1回の実行分のエビデンス（record.json 群）から、人がレビューするための report.html を作る。
    /// テスト名・操作手順・入力値・確認結果・キャプチャを時系列で並べる。
    /// </summary>
    public static class RunReportWriter
    {
        /// <param name="runDir">Evidence\実行日時 のフォルダ</param>
        /// <returns>作成した report.html のパス</returns>
        public static string Write(string runDir)
        {
            var records = LoadRecords(runDir);
            var path = Path.Combine(runDir, "report.html");
            File.WriteAllText(path, Build(runDir, records), new UTF8Encoding(true));
            return path;
        }

        public static List<TestRecord> LoadRecords(string dir)
        {
            var records = new List<TestRecord>();
            foreach (var file in Directory.GetFiles(dir, "record.json", SearchOption.AllDirectories))
            {
                var record = JsonFile.Read<TestRecord>(file);
                record.Folder = Path.GetDirectoryName(file);
                records.Add(record);
            }
            return records
                .OrderBy(r => r.Target).ThenBy(r => r.Scenario).ThenBy(r => r.StartedAt)
                .ToList();
        }

        private static string Build(string runDir, List<TestRecord> records)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html lang=\"ja\"><head><meta charset=\"utf-8\">");
            sb.AppendLine("<title>GUIテスト結果 " + Html.E(Path.GetFileName(runDir)) + "</title>");
            sb.AppendLine("<style>" + Html.Style + "</style></head><body>");
            sb.AppendLine("<h1>GUIテスト結果</h1>");
            sb.AppendLine("<div class=\"meta\">実行: " + Html.E(Path.GetFileName(runDir)) + "　／　レポート作成: "
                + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "</div>");

            // ---- 集計 ----
            sb.AppendLine("<table><tr><th>対象</th><th>テスト数</th><th>合格</th><th>不合格</th></tr>");
            foreach (var g in records.GroupBy(r => r.Target))
            {
                var ng = g.Count(r => r.Outcome != "合格");
                sb.AppendFormat("<tr><td>{0}</td><td class=\"num\">{1}</td><td class=\"num\">{2}</td><td class=\"num\">{3}</td></tr>\n",
                    Html.E(g.Key), g.Count(), g.Count() - ng, ng == 0 ? "0" : "<span class=\"badge ng\">" + ng + "</span>");
            }
            sb.AppendLine("</table>");

            sb.AppendLine("<div class=\"toolbar\"><label><input type=\"checkbox\" id=\"filter\" onchange=\"applyFilter(this)\"> 不合格のみ表示</label></div>");

            // ---- 一覧 ----
            sb.AppendLine("<h2>テスト一覧</h2>");
            sb.AppendLine("<table><tr><th>No</th><th>対象</th><th>画面（テストクラス）</th><th>テスト</th><th>結果</th><th>キャプチャ</th></tr>");
            for (var i = 0; i < records.Count; i++)
            {
                var r = records[i];
                sb.AppendFormat("<tr data-status=\"{0}\"><td class=\"num\">{1}</td><td>{2}</td><td>{3}</td><td><a href=\"#t{1}\">{4}</a></td><td>{5}</td><td class=\"num\">{6}</td></tr>\n",
                    Status(r), i + 1, Html.E(r.Target), Html.E(r.Scenario), Html.E(r.DisplayName), Badge(r.Outcome),
                    r.Steps.Count(s => s.Kind == StepKind.Capture));
            }
            sb.AppendLine("</table>");

            // ---- 詳細 ----
            sb.AppendLine("<h2>テスト詳細</h2>");
            for (var i = 0; i < records.Count; i++)
            {
                AppendTest(sb, runDir, records[i], i + 1);
            }

            sb.AppendLine(Html.FilterScript);
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        private static void AppendTest(StringBuilder sb, string runDir, TestRecord r, int no)
        {
            sb.AppendFormat("<div class=\"test\" id=\"t{0}\" data-status=\"{1}\">\n", no, Status(r));
            sb.AppendFormat("<h3>{0}. {1}　{2}</h3>\n", no, Html.E(r.DisplayName), Badge(r.Outcome));
            sb.AppendFormat("<div class=\"meta\">対象: {0}　／　画面: {1}　／　{2} ～ {3}</div>\n",
                Html.E(r.Target), Html.E(r.Scenario), Html.E(r.StartedAt), Html.E(r.FinishedAt));

            sb.AppendLine("<ol class=\"steps\">");
            foreach (var s in r.Steps)
            {
                if (s.Kind == StepKind.Operation)
                {
                    sb.AppendLine("<li class=\"op\">" + Html.E(s.Text) + "</li>");
                }
                else if (s.Kind == StepKind.Check)
                {
                    var ok = s.Passed == true;
                    sb.AppendFormat("<li class=\"check\"><span class=\"badge {0}\">{1}</span> 確認: {2}　期待値「{3}」　実際「{4}」</li>\n",
                        ok ? "ok" : "ng", ok ? "OK" : "NG", Html.E(s.Text), Html.E(s.Expected), Html.E(s.Actual));
                }
                else if (s.Kind == StepKind.Capture)
                {
                    AppendCapture(sb, runDir, r, s);
                }
            }
            sb.AppendLine("</ol>");

            if (r.Outcome != "合格" && !r.Steps.Any(s => s.Passed == false))
            {
                sb.AppendLine("<div class=\"badge warn\">確認以外の箇所でエラーになっています。詳細はテスト結果(.trx)のエラーメッセージを参照してください。</div>");
            }
            sb.AppendLine("</div>");
        }

        private static void AppendCapture(StringBuilder sb, string runDir, TestRecord r, StepRecord s)
        {
            var image = Html.RelativeUrl(runDir, Path.Combine(r.Folder, s.EvidenceImage));
            sb.AppendLine("<li class=\"capture\">");
            sb.AppendFormat("<div class=\"caption\">{0:00}. {1}　<span class=\"meta\">（{2}）</span></div>\n",
                s.CaptureNo, Html.E(s.Text), Html.E(s.WindowTitle));
            sb.AppendFormat("<a href=\"{0}\" target=\"_blank\"><img class=\"shot\" src=\"{0}\" loading=\"lazy\" alt=\"{1}\"></a>\n", image, Html.E(s.Text));
            if (s.DesktopImage != null)
            {
                var desktop = Html.RelativeUrl(runDir, Path.Combine(r.Folder, s.DesktopImage));
                sb.AppendFormat("<div><a href=\"{0}\" target=\"_blank\">デスクトップ全体のキャプチャ</a></div>\n", desktop);
            }
            if (s.Controls != null && s.Controls.Count > 0)
            {
                var masked = new HashSet<string>((s.Masks ?? new List<MaskArea>()).Select(m => m.Id));
                sb.AppendFormat("<details><summary>画面の項目値（{0}項目）</summary>\n", s.Controls.Count);
                sb.AppendLine("<table><tr><th>項目ID</th><th>項目名</th><th>種類</th><th>値</th><th>状態</th></tr>");
                foreach (var c in s.Controls)
                {
                    sb.AppendFormat("<tr{0}><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>\n",
                        masked.Contains(c.Id) ? " class=\"masked\"" : "", Html.E(c.Id), Html.E(c.Label), Html.E(c.Type),
                        Html.E(c.Value).Replace("\n", "<br>") + (masked.Contains(c.Id) ? "（比較対象外）" : ""), c.Enabled ? "有効" : "無効");
                }
                sb.AppendLine("</table></details>");
            }
            sb.AppendLine("</li>");
        }

        private static string Status(TestRecord r)
        {
            return r.Outcome == "合格" ? "ok" : "ng";
        }

        private static string Badge(string outcome)
        {
            if (outcome == "合格") return "<span class=\"badge ok\">合格</span>";
            return "<span class=\"badge ng\">" + Html.E(outcome ?? "未完了") + "</span>";
        }
    }
}
