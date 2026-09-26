using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using GuiTestKit.Evidence;
using GuiTestKit.Reporting;

namespace GuiTestKit.Comparison
{
    /// <summary>比較の判定。</summary>
    public static class CompareStatus
    {
        public const string Same = "一致";
        public const string Different = "差異あり";
        public const string OnlyOneSide = "片方のみ";
    }

    public class CompareOptions
    {
        /// <summary>RGB各色の差がこの値以下なら同じとみなす（0 = 完全一致）。</summary>
        public int Tolerance { get; set; }
    }

    /// <summary>テスト1件の比較結果。</summary>
    public class TestComparison
    {
        /// <summary>シナリオ\テスト名[_ケース番号]（VB・C#でテストを対応付けるキー）。</summary>
        public string Key { get; set; }
        public TestRecord A { get; set; }
        public TestRecord B { get; set; }
        public List<StepComparison> Steps { get; set; }
        public List<string> Notes { get; set; }

        public string Status
        {
            get
            {
                if (A == null || B == null) return CompareStatus.OnlyOneSide;
                if (Notes.Count > 0 || Steps.Any(s => s.Status != CompareStatus.Same)) return CompareStatus.Different;
                return CompareStatus.Same;
            }
        }

        public TestComparison()
        {
            Steps = new List<StepComparison>();
            Notes = new List<string>();
        }
    }

    /// <summary>キャプチャ1枚分の比較結果。</summary>
    public class StepComparison
    {
        public int No { get; set; }
        public StepRecord A { get; set; }
        public StepRecord B { get; set; }
        public ImageDiffResult Image { get; set; }
        public string DiffImage { get; set; }
        public List<ValueDifference> Values { get; set; }
        public List<string> Notes { get; set; }

        public string Status
        {
            get
            {
                if (A == null || B == null) return CompareStatus.OnlyOneSide;
                var imageDiff = Image != null && (Image.DiffPixels > 0 || !Image.SizeMatches);
                return imageDiff || Values.Count > 0 || Notes.Count > 0 ? CompareStatus.Different : CompareStatus.Same;
            }
        }

        public StepComparison()
        {
            Values = new List<ValueDifference>();
            Notes = new List<string>();
        }
    }

    /// <summary>項目値の差異。</summary>
    public class ValueDifference
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string What { get; set; }
        public string A { get; set; }
        public string B { get; set; }
    }

    /// <summary>
    /// 2つのエビデンスフォルダ（例: VB断面の実行結果 と C#断面の実行結果）を突き合わせる。
    /// テストは「シナリオ\テスト名」、キャプチャは撮った順番で対応付ける。
    /// </summary>
    public static class EvidenceComparer
    {
        /// <param name="dirA">比較元（例: Evidence\20260926_100000\VB）</param>
        /// <param name="dirB">比較先（例: Evidence\20260926_110000\CS）</param>
        /// <param name="outDir">差分画像と compare.html の出力先</param>
        public static List<TestComparison> Compare(string dirA, string dirB, string outDir, CompareOptions options)
        {
            var recordsA = RunReportWriter.LoadRecords(dirA).ToDictionary(r => KeyOf(dirA, r));
            var recordsB = RunReportWriter.LoadRecords(dirB).ToDictionary(r => KeyOf(dirB, r));
            var diffDir = Path.Combine(outDir, "diff");
            Directory.CreateDirectory(diffDir);

            var results = new List<TestComparison>();
            var keys = recordsA.Keys.Concat(recordsB.Keys.Where(k => !recordsA.ContainsKey(k))).ToList();
            for (var t = 0; t < keys.Count; t++)
            {
                TestRecord a, b;
                recordsA.TryGetValue(keys[t], out a);
                recordsB.TryGetValue(keys[t], out b);
                var test = new TestComparison { Key = keys[t], A = a, B = b };
                results.Add(test);
                if (a == null || b == null) continue;

                if (a.Outcome != b.Outcome) test.Notes.Add("テスト結果が違います（" + a.Outcome + " / " + b.Outcome + "）");

                var capturesA = a.Steps.Where(s => s.Kind == StepKind.Capture).ToList();
                var capturesB = b.Steps.Where(s => s.Kind == StepKind.Capture).ToList();
                for (var i = 0; i < Math.Max(capturesA.Count, capturesB.Count); i++)
                {
                    var step = new StepComparison
                    {
                        No = i + 1,
                        A = i < capturesA.Count ? capturesA[i] : null,
                        B = i < capturesB.Count ? capturesB[i] : null,
                    };
                    test.Steps.Add(step);
                    if (step.A == null || step.B == null) continue;

                    CompareStep(step, a, b, Path.Combine(diffDir, string.Format("{0:0000}_{1:00}.png", t + 1, i + 1)), options);
                }
            }
            return results;
        }

        private static void CompareStep(StepComparison step, TestRecord a, TestRecord b, string diffPath, CompareOptions options)
        {
            if (step.A.Text != step.B.Text)
            {
                step.Notes.Add("キャプチャの名前が違います（" + step.A.Text + " / " + step.B.Text + "）。手順がずれている可能性があります。");
            }

            // 比較対象外: 両方の指定を合わせる
            var maskAreas = (step.A.Masks ?? new List<MaskArea>()).Concat(step.B.Masks ?? new List<MaskArea>()).ToList();
            var maskIds = new HashSet<string>(maskAreas.Select(m => m.Id));
            var maskRects = maskAreas.Select(m => new Rectangle(m.X, m.Y, m.Width, m.Height)).ToList();

            step.Image = ImageComparer.Compare(
                Path.Combine(a.Folder, step.A.CompareImage), Path.Combine(b.Folder, step.B.CompareImage),
                maskRects, options.Tolerance, diffPath);
            step.DiffImage = diffPath;

            step.Values = CompareValues(step.A.Controls, step.B.Controls, maskIds);
        }

        public static List<ValueDifference> CompareValues(List<ControlSnapshot> a, List<ControlSnapshot> b, ICollection<string> excludedIds)
        {
            var diffs = new List<ValueDifference>();
            var mapA = (a ?? new List<ControlSnapshot>()).ToDictionary(c => c.Id);
            var mapB = (b ?? new List<ControlSnapshot>()).ToDictionary(c => c.Id);
            var ids = mapA.Keys.Concat(mapB.Keys.Where(k => !mapA.ContainsKey(k)));

            foreach (var id in ids)
            {
                if (excludedIds.Contains(id)) continue;
                ControlSnapshot ca, cb;
                mapA.TryGetValue(id, out ca);
                mapB.TryGetValue(id, out cb);
                var label = (ca ?? cb).Label;

                if (ca == null || cb == null)
                {
                    diffs.Add(new ValueDifference { Id = id, Label = label, What = "項目の有無", A = ca == null ? "（なし）" : "あり", B = cb == null ? "（なし）" : "あり" });
                    continue;
                }
                if (ca.Value != cb.Value) diffs.Add(new ValueDifference { Id = id, Label = label, What = "値", A = ca.Value, B = cb.Value });
                if (ca.Enabled != cb.Enabled) diffs.Add(new ValueDifference { Id = id, Label = label, What = "有効/無効", A = ca.Enabled ? "有効" : "無効", B = cb.Enabled ? "有効" : "無効" });
                if (ca.Type != cb.Type) diffs.Add(new ValueDifference { Id = id, Label = label, What = "部品の種類", A = ca.Type, B = cb.Type });
                if (ca.X != cb.X || ca.Y != cb.Y || ca.Width != cb.Width || ca.Height != cb.Height)
                {
                    diffs.Add(new ValueDifference { Id = id, Label = label, What = "位置・サイズ", A = Bounds(ca), B = Bounds(cb) });
                }
            }
            return diffs;
        }

        private static string Bounds(ControlSnapshot c)
        {
            return string.Format("({0},{1}) {2}×{3}", c.X, c.Y, c.Width, c.Height);
        }

        private static string KeyOf(string dir, TestRecord record)
        {
            var full = Path.GetFullPath(dir).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var folder = Path.GetFullPath(record.Folder);
            return folder.StartsWith(full, StringComparison.OrdinalIgnoreCase) ? folder.Substring(full.Length) : folder;
        }
    }
}
