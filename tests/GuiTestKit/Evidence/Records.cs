using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GuiTestKit.Evidence
{
    // ------------------------------------------------------------------
    // エビデンスの記録内容。テスト1件ごとに record.json として保存し、
    // HTMLレポートとVB/C#比較ツールの両方がこれを読む。
    // ------------------------------------------------------------------

    /// <summary>テスト1件分の記録。</summary>
    [DataContract]
    public class TestRecord
    {
        [DataMember(Order = 1)] public string Target { get; set; }
        [DataMember(Order = 2)] public string Scenario { get; set; }
        [DataMember(Order = 3)] public string TestName { get; set; }
        [DataMember(Order = 4, EmitDefaultValue = false)] public string CaseId { get; set; }
        [DataMember(Order = 5)] public string Outcome { get; set; }
        [DataMember(Order = 6)] public string StartedAt { get; set; }
        [DataMember(Order = 7)] public string FinishedAt { get; set; }
        [DataMember(Order = 8)] public List<StepRecord> Steps { get; set; }

        /// <summary>record.json があるフォルダ（読み込み時に設定。保存はしない）。</summary>
        public string Folder { get; set; }

        public string DisplayName
        {
            get { return CaseId == null ? TestName : TestName + " [" + CaseId + "]"; }
        }

        public TestRecord()
        {
            Steps = new List<StepRecord>();
        }
    }

    /// <summary>記録の種類。テスト中に起きた順に並べる。</summary>
    public static class StepKind
    {
        public const string Operation = "操作";
        public const string Check = "確認";
        public const string Capture = "キャプチャ";
    }

    /// <summary>テスト中の1件の出来事（操作・確認・キャプチャ）。</summary>
    [DataContract]
    public class StepRecord
    {
        [DataMember(Order = 1)] public string Kind { get; set; }
        [DataMember(Order = 2)] public string Text { get; set; }

        // ---- 確認（Kind = 確認） ----
        [DataMember(Order = 10, EmitDefaultValue = false)] public string Expected { get; set; }
        [DataMember(Order = 11, EmitDefaultValue = false)] public string Actual { get; set; }
        [DataMember(Order = 12, EmitDefaultValue = false)] public bool? Passed { get; set; }

        // ---- キャプチャ（Kind = キャプチャ） ----
        [DataMember(Order = 20, EmitDefaultValue = false)] public int CaptureNo { get; set; }
        [DataMember(Order = 21, EmitDefaultValue = false)] public string WindowId { get; set; }
        [DataMember(Order = 22, EmitDefaultValue = false)] public string WindowTitle { get; set; }

        /// <summary>人が見る用（タイトルバー込み、親画面も含む）。record.json からの相対パス。</summary>
        [DataMember(Order = 23, EmitDefaultValue = false)] public string EvidenceImage { get; set; }

        /// <summary>比較用（前面ウィンドウの中身部分だけ）。record.json からの相対パス。</summary>
        [DataMember(Order = 24, EmitDefaultValue = false)] public string CompareImage { get; set; }

        /// <summary>失敗時のみ：デスクトップ全体。record.json からの相対パス。</summary>
        [DataMember(Order = 25, EmitDefaultValue = false)] public string DesktopImage { get; set; }

        /// <summary>撮影時点の画面の全項目の値。</summary>
        [DataMember(Order = 26, EmitDefaultValue = false)] public List<ControlSnapshot> Controls { get; set; }

        /// <summary>比較対象外の項目（値の比較からも、画像の比較からも外す）。</summary>
        [DataMember(Order = 27, EmitDefaultValue = false)] public List<MaskArea> Masks { get; set; }
    }

    /// <summary>画面の1項目の状態。</summary>
    [DataContract]
    public class ControlSnapshot
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public string Type { get; set; }
        [DataMember(Order = 3, EmitDefaultValue = false)] public string Label { get; set; }
        [DataMember(Order = 4)] public string Value { get; set; }
        [DataMember(Order = 5)] public bool Enabled { get; set; }

        /// <summary>比較用画像の左上を (0,0) とした位置。</summary>
        [DataMember(Order = 6)] public int X { get; set; }
        [DataMember(Order = 7)] public int Y { get; set; }
        [DataMember(Order = 8)] public int Width { get; set; }
        [DataMember(Order = 9)] public int Height { get; set; }
    }

    /// <summary>比較対象外の項目と、比較用画像上の位置。</summary>
    [DataContract]
    public class MaskArea
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public int X { get; set; }
        [DataMember(Order = 3)] public int Y { get; set; }
        [DataMember(Order = 4)] public int Width { get; set; }
        [DataMember(Order = 5)] public int Height { get; set; }
    }
}
