using System.Collections.Generic;
using FlaUI.Core.AutomationElements;

namespace GuiTestKit.Automation
{
    /// <summary>
    /// 標準の部品として扱えないもの（Spread などの市販部品）の値の読み書きを差し替える口。
    /// CustomControls.Handlers に登録すると、Set / Get / 全項目の吸い出し がこちらを使う。
    /// </summary>
    public interface ICustomControlHandler
    {
        /// <summary>この部品を担当するかどうか（AutomationId の接頭辞などで判定する）。</summary>
        bool CanHandle(AutomationElement element);

        /// <summary>比較・レポート用に値を文字列で返す（表なら TSV など）。</summary>
        string Read(AutomationElement element);

        void Write(AutomationElement element, string value);
    }

    /// <summary>全画面共通で使う差し替え部品の登録先。製品のテスト基底クラスの静的コンストラクタ等で登録する。</summary>
    public static class CustomControls
    {
        public static readonly List<ICustomControlHandler> Handlers = new List<ICustomControlHandler>();
    }
}
