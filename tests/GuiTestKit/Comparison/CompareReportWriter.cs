using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GuiTestKit.Evidence;
using GuiTestKit.Reporting;

namespace GuiTestKit.Comparison
{
    /// <summary>
    /// VB断面・C#断面の比較結果を compare.html に出す。
    /// 既定では差異のあるものだけ表示し、キャプチャを「VB｜C#｜差分画像」の順で横に並べる。
    /// </summary>
    public static class CompareReportWriter
    {
        public static string Write(string outDir, string dirA, string dirB, CompareOptions options, List<TestComparison> results)
        {
            var nameA = SideName(dirA, results.Select(r => r.A));
            var nameB = SideName(dirB, results.Select(r => r.B));

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html lang=\"ja\"><head><meta charset=\"utf-8\">");
            sb.AppendLine("<title>キャプチャ比較 " + Html.E(nameA) + " / " + Html.E(nameB) + "</title>");
            sb.AppendLine("<style>" + Html.Style + "</style></head><body>");
            sb.AppendLine("<h1>キャプチャ・項目値の比較（" + Html.E(nameA) + " / " + Html.E(nameB) + "）</h1>");
            sb.AppendLine("<div class=\"meta\">");
            sb.AppendLine(Html.E(nameA) + ": " + Html.E(Path.GetFullPath(dirA)) + "<br>");
            sb.AppendLine(Html.E(nameB) + ": " + Html.E(Path.GetFullPath(dirB)) + "<br>");
            sb.AppendLine("色の許容差: " + options.Tolerance + "　／　作成: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            sb.AppendLine("</div>");

            // ---- 集計 ----
            sb.AppendLine("<table><tr><th>テスト数</th><th>一致</th><th>差異あり</th><th>片方のみ</th><th>キャプチャ数</th><th>差異のあるキャプチャ</th></tr>");
            sb.AppendFormat("<tr><td class=\"num\">{0}</td><td class=\"num\">{1}</td><td class=\"num\">{2}</td><td class=\"num\">{3}</td><td class=\"num\">{4}</td><td class=\"num\">{5}</td></tr></table>\n",
                results.Count,
                results.Count(r => r.Status == CompareStatus.Same),
                results.Count(r => r.Status == CompareStatus.Different),
                results.Count(r => r.Status == CompareStatus.OnlyOneSide),
                results.Sum(r => r.Steps.Count),
                results.Sum(r => r.Steps.Count(s => s.Status != CompareStatus.Same)));

            sb.AppendLine("<div class=\"meta\">差分画像の色: <span style=\"color:#cf222e\">■ 赤＝差異</span>　<span style=\"color:#f90\">■ オレンジ＝サイズ違いではみ出した部分</span>　<span style=\"color:#9db4d0\">■ 青い斜線＝比較対象外</span>　薄いグレー＝一致</div>");
            sb.AppendLine("<div class=\"toolbar\"><label><input type=\"checkbox\" id=\"filter\" checked onchange=\"applyFilter(this)\"> 差異のあるものだけ表示</label></div>");

            // ---- 一覧 ----
            sb.AppendLine("<h2>テスト一覧</h2>");
            sb.AppendFormat("<table><tr><th>No</th><th>テスト</th><th>判定</th><th>{0} 結果</th><th>{1} 結果</th><th>差異のあるキャプチャ</th></tr>\n", Html.E(nameA), Html.E(nameB));
            for (var i = 0; i < results.Count; i++)
            {
                var r = results[i];
                sb.AppendFormat("<tr data-status=\"{0}\"><td class=\"num\">{1}</td><td><a href=\"#c{1}\">{2}</a></td><td>{3}</td><td>{4}</td><td>{5}</td><td class=\"num\">{6} / {7}</td></tr>\n",
                    FilterStatus(r.Status), i + 1, Html.E(r.Key), Badge(r.Status),
                    Html.E(r.A == null ? "（なし）" : r.A.Outcome), Html.E(r.B == null ? "（なし）" : r.B.Outcome),
                    r.Steps.Count(s => s.Status != CompareStatus.Same), r.Steps.Count);
            }
            sb.AppendLine("</table>");

            // ---- 詳細 ----
            sb.AppendLine("<h2>詳細</h2>");
            for (var i = 0; i < results.Count; i++)
            {
                AppendTest(sb, outDir, results[i], i + 1, nameA, nameB);
            }

            sb.AppendLine(Html.FilterScript);
            sb.AppendLine("</body></html>");

            var path = Path.Combine(outDir, "compare.html");
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(true));
            return path;
        }

        private static void AppendTest(StringBuilder sb, string outDir, TestComparison r, int no, string nameA, string nameB)
        {
            sb.AppendFormat("<div class=\"test\" id=\"c{0}\" data-status=\"{1}\">\n", no, FilterStatus(r.Status));
            sb.AppendFormat("<h3>{0}. {1}　{2}</h3>\n", no, Html.E(r.Key), Badge(r.Status));
            if (r.A == null || r.B == null)
            {
                sb.AppendFormat("<div>{0} にしかありません。</div></div>\n", Html.E(r.A == null ? nameB : nameA));
                return;
            }
            foreach (var note in r.Notes) sb.AppendLine("<div class=\"badge warn\">" + Html.E(note) + "</div>");

            foreach (var s in r.Steps)
            {
                sb.AppendFormat("<div data-status=\"{0}\">\n", FilterStatus(s.Status));
                var label = s.A != null ? s.A.Text : s.B.Text;
                sb.AppendFormat("<div class=\"caption\" style=\"margin-top:16px\">{0:00}. {1}　{2}</div>\n", s.No, Html.E(label), Badge(s.Status));

                if (s.A == null || s.B == null)
                {
                    sb.AppendFormat("<div>このキャプチャは {0} にしかありません。</div></div>\n", Html.E(s.A == null ? nameB : nameA));
                    continue;
                }
                foreach (var note in s.Notes) sb.AppendLine("<div class=\"badge warn\">" + Html.E(note) + "</div>");

                var img = s.Image;
                var sizeText = img.SizeMatches
                    ? string.Format("{0}×{1}", img.SizeA.Width, img.SizeA.Height)
                    : string.Format("<span class=\"badge ng\">サイズ違い {0}×{1} / {2}×{3}</span>", img.SizeA.Width, img.SizeA.Height, img.SizeB.Width, img.SizeB.Height);
                sb.AppendFormat("<div class=\"meta\">画像: {0}　差異ピクセル: {1:N0}（{2:P3}）</div>\n", sizeText, img.DiffPixels, img.DiffRatio);

                sb.AppendLine("<div class=\"pair\">");
                AppendImage(sb, outDir, nameA, r.A, s.A);
                AppendImage(sb, outDir, nameB, r.B, s.B);
                var diffUrl = Html.RelativeUrl(outDir, s.DiffImage);
                sb.AppendFormat("<div><div class=\"meta\">差分</div><a href=\"{0}\" target=\"_blank\"><img class=\"shot\" src=\"{0}\" loading=\"lazy\"></a></div>\n", diffUrl);
                sb.AppendLine("</div>");

                if (s.Values.Count > 0)
                {
                    sb.AppendFormat("<table><tr><th>項目ID</th><th>項目名</th><th>差異</th><th>{0}</th><th>{1}</th></tr>\n", Html.E(nameA), Html.E(nameB));
                    foreach (var v in s.Values)
                    {
                        sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td class=\"diffcell\">{3}</td><td class=\"diffcell\">{4}</td></tr>\n",
                            Html.E(v.Id), Html.E(v.Label), Html.E(v.What), Html.E(v.A), Html.E(v.B));
                    }
                    sb.AppendLine("</table>");
                }
                sb.AppendLine("</div>");
            }
            sb.AppendLine("</div>");
        }

        private static void AppendImage(StringBuilder sb, string outDir, string name, TestRecord record, StepRecord step)
        {
            var compare = Html.RelativeUrl(outDir, Path.Combine(record.Folder, step.CompareImage));
            var evidence = Html.RelativeUrl(outDir, Path.Combine(record.Folder, step.EvidenceImage));
            sb.AppendFormat("<div><div class=\"meta\">{0}（<a href=\"{1}\" target=\"_blank\">タイトルバー込み</a>）</div><a href=\"{2}\" target=\"_blank\"><img class=\"shot\" src=\"{2}\" loading=\"lazy\"></a></div>\n",
                Html.E(name), evidence, compare);
        }

        /// <summary>表示名: 記録の Target（VB / CS）。取れなければフォルダ名。</summary>
        private static string SideName(string dir, IEnumerable<TestRecord> records)
        {
            var target = records.Where(r => r != null).Select(r => r.Target).FirstOrDefault();
            return target ?? Path.GetFileName(Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar));
        }

        private static string FilterStatus(string status)
        {
            return status == CompareStatus.Same ? "ok" : "ng";
        }

        private static string Badge(string status)
        {
            var css = status == CompareStatus.Same ? "ok" : status == CompareStatus.Different ? "ng" : "warn";
            return "<span class=\"badge " + css + "\">" + Html.E(status) + "</span>";
        }
    }
}
