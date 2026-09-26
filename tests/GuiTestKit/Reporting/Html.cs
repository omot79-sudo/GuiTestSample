using System;
using System.IO;
using System.Linq;
using System.Net;

namespace GuiTestKit.Reporting
{
    /// <summary>HTMLレポート共通の部品（エスケープ・相対リンク・スタイル）。</summary>
    internal static class Html
    {
        public static string E(string text)
        {
            return WebUtility.HtmlEncode(text ?? string.Empty);
        }

        /// <summary>fromDir から見た target への相対URL（日本語・記号はエンコード）。</summary>
        public static string RelativeUrl(string fromDir, string target)
        {
            var from = new Uri(Path.GetFullPath(fromDir).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            var to = new Uri(Path.GetFullPath(target));
            if (from.Scheme != to.Scheme || !string.Equals(from.Host, to.Host, StringComparison.OrdinalIgnoreCase)
                || Path.GetPathRoot(from.LocalPath) != Path.GetPathRoot(to.LocalPath))
            {
                return to.AbsoluteUri;
            }
            var relative = Uri.UnescapeDataString(from.MakeRelativeUri(to).ToString());
            return string.Join("/", relative.Split('/').Select(s => s == ".." ? s : Uri.EscapeDataString(s)));
        }

        public const string Style = @"
:root { --fg:#1f2328; --muted:#656d76; --line:#d0d7de; --bg:#ffffff; --sub:#f6f8fa;
        --ok:#1a7f37; --okbg:#dafbe1; --ng:#cf222e; --ngbg:#ffebe9; --warn:#9a6700; --warnbg:#fff8c5; }
* { box-sizing: border-box; }
body { margin: 0; padding: 16px 24px 48px; font-family: 'Yu Gothic UI','Meiryo UI',Meiryo,sans-serif;
       font-size: 14px; color: var(--fg); background: var(--bg); }
h1 { font-size: 20px; margin: 0 0 4px; }
h2 { font-size: 16px; margin: 32px 0 8px; padding-bottom: 4px; border-bottom: 2px solid var(--line); }
h3 { font-size: 14px; margin: 16px 0 6px; }
.meta { color: var(--muted); margin-bottom: 16px; }
table { border-collapse: collapse; margin: 8px 0; }
th, td { border: 1px solid var(--line); padding: 4px 8px; text-align: left; vertical-align: top; }
th { background: var(--sub); font-weight: 600; white-space: nowrap; }
td.num { text-align: right; }
.badge { display: inline-block; padding: 1px 8px; border-radius: 10px; font-size: 12px; font-weight: 600; }
.ok { color: var(--ok); background: var(--okbg); }
.ng { color: var(--ng); background: var(--ngbg); }
.warn { color: var(--warn); background: var(--warnbg); }
.toolbar { position: sticky; top: 0; background: var(--bg); padding: 8px 0; border-bottom: 1px solid var(--line); z-index: 1; }
.test { border: 1px solid var(--line); border-radius: 6px; padding: 8px 16px 16px; margin: 16px 0; }
.test > h3 { margin-top: 8px; }
ol.steps { padding-left: 20px; }
ol.steps li { margin: 4px 0; }
li.op { color: var(--fg); }
li.check { list-style: none; margin-left: -20px; }
li.capture { list-style: none; margin: 12px 0 12px -20px; }
.caption { font-weight: 600; margin-bottom: 4px; }
img.shot { max-width: 100%; border: 1px solid var(--line); cursor: zoom-in; }
.pair { display: flex; gap: 12px; flex-wrap: wrap; align-items: flex-start; }
.pair > div { flex: 1 1 300px; min-width: 0; }
details { margin-top: 4px; }
summary { cursor: pointer; color: var(--muted); }
.diffcell { background: var(--ngbg); }
.masked { color: var(--muted); }
";

        /// <summary>チェックボックスで「問題のあるものだけ表示」を切り替えるスクリプト。</summary>
        public const string FilterScript = @"
<script>
function applyFilter(cb){document.querySelectorAll('[data-status]').forEach(function(el){
 el.style.display=(cb.checked&&el.getAttribute('data-status')==='ok')?'none':'';});}
window.addEventListener('load',function(){var cb=document.getElementById('filter');if(cb)applyFilter(cb);});
</script>";
    }
}
