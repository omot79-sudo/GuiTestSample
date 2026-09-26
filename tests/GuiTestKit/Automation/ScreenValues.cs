using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GuiTestKit.Automation
{
    /// <summary>
    /// 画面の項目ID → 値 の一覧。入力する順番を保つ（区分を変えると他の項目が無効になる、等の画面があるため）。
    /// コレクション初期化子で書ける:
    /// <code>
    /// new ScreenValues
    /// {
    ///     { "txtCode", "000001", "顧客コード" },   // 項目ID, 値, 項目名（項目名は省略可。レポートの表示に使う）
    ///     { "chkActive", ScreenValues.ON },
    /// }
    /// </code>
    /// 値の書き方: テキスト・コンボボックスは表示文字列、チェックボックス・ラジオボタンは "ON"/"OFF"、
    /// 空文字 "" は「空にする」、null は「触らない」。
    /// </summary>
    public class ScreenValues : IEnumerable<ScreenValue>
    {
        public const string ON = "ON";
        public const string OFF = "OFF";

        private readonly List<ScreenValue> _items = new List<ScreenValue>();

        public void Add(string id, string value)
        {
            Add(id, value, null);
        }

        public void Add(string id, string value, string label)
        {
            var index = _items.FindIndex(x => x.Id == id);
            var item = new ScreenValue(id, value, label);
            if (index >= 0) _items[index] = item; else _items.Add(item);
        }

        public int Count { get { return _items.Count; } }

        public bool Contains(string id)
        {
            return _items.Any(x => x.Id == id);
        }

        public string this[string id]
        {
            get
            {
                var item = _items.FirstOrDefault(x => x.Id == id);
                return item == null ? null : item.Value;
            }
        }

        public string LabelOf(string id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            return item == null ? null : item.Label;
        }

        /// <summary>
        /// この一覧（基本値）に changes を上書きした新しい一覧を返す。
        /// 並び順は基本値の順。基本値にない項目は末尾に追加する。
        /// </summary>
        public ScreenValues With(ScreenValues changes)
        {
            var result = new ScreenValues();
            foreach (var item in _items)
            {
                var changed = changes != null && changes.Contains(item.Id);
                result.Add(item.Id, changed ? changes[item.Id] : item.Value, item.Label ?? (changed ? changes.LabelOf(item.Id) : null));
            }
            if (changes != null)
            {
                foreach (var item in changes.Where(x => !Contains(x.Id)))
                {
                    result.Add(item.Id, item.Value, item.Label);
                }
            }
            return result;
        }

        public IEnumerator<ScreenValue> GetEnumerator() { return _items.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

    public class ScreenValue
    {
        public ScreenValue(string id, string value, string label)
        {
            Id = id;
            Value = value;
            Label = label;
        }

        public string Id { get; private set; }
        public string Value { get; private set; }
        public string Label { get; private set; }
    }
}
