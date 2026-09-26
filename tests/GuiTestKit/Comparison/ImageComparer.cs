using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace GuiTestKit.Comparison
{
    /// <summary>画像比較の結果。</summary>
    public class ImageDiffResult
    {
        public Size SizeA { get; set; }
        public Size SizeB { get; set; }
        public bool SizeMatches { get { return SizeA == SizeB; } }

        /// <summary>差異のあったピクセル数（サイズ違いではみ出した部分を含む）。</summary>
        public int DiffPixels { get; set; }

        /// <summary>比較したピクセル数（比較対象外の領域を除く）。</summary>
        public int ComparedPixels { get; set; }

        /// <summary>差異のあった範囲（差異がなければ Empty）。</summary>
        public Rectangle DiffBounds { get; set; }

        public double DiffRatio { get { return ComparedPixels == 0 ? 0 : (double)DiffPixels / ComparedPixels; } }
    }

    /// <summary>
    /// 2枚の画像をピクセル単位で比較し、差異を赤く塗った差分画像を作る。
    /// 差分画像の色: 赤 = 差異、オレンジ = サイズ違いではみ出した部分、青い斜線 = 比較対象外、薄いグレー = 一致。
    /// </summary>
    public static class ImageComparer
    {
        private const int Red = unchecked((int)0xFFFF0000);
        private const int Orange = unchecked((int)0xFFFF9900);
        private const int MaskDark = unchecked((int)0xFF9DB4D0);
        private const int MaskLight = unchecked((int)0xFFDCE5F0);

        /// <param name="tolerance">RGB各色の差がこの値以下なら同じとみなす（0 = 完全一致）。</param>
        /// <param name="diffImagePath">差分画像の保存先（null なら保存しない）。</param>
        public static ImageDiffResult Compare(string pathA, string pathB, IList<Rectangle> masks, int tolerance, string diffImagePath)
        {
            int widthA, heightA, widthB, heightB;
            var a = LoadPixels(pathA, out widthA, out heightA);
            var b = LoadPixels(pathB, out widthB, out heightB);

            var width = Math.Max(widthA, widthB);
            var height = Math.Max(heightA, heightB);
            var mask = BuildMask(width, height, masks);
            var output = new int[width * height];

            int diff = 0, compared = 0;
            int left = int.MaxValue, top = int.MaxValue, right = -1, bottom = -1;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var i = y * width + x;
                    if (mask[i])
                    {
                        output[i] = ((x + y) / 4) % 2 == 0 ? MaskDark : MaskLight;
                        continue;
                    }

                    compared++;
                    var inA = x < widthA && y < heightA;
                    var inB = x < widthB && y < heightB;
                    bool different;
                    if (!inA || !inB)
                    {
                        output[i] = Orange;
                        different = true;
                    }
                    else
                    {
                        var ca = a[y * widthA + x];
                        var cb = b[y * widthB + x];
                        different = MaxChannelDiff(ca, cb) > tolerance;
                        output[i] = different ? Red : Faded(cb);
                    }

                    if (different)
                    {
                        diff++;
                        if (x < left) left = x;
                        if (y < top) top = y;
                        if (x > right) right = x;
                        if (y > bottom) bottom = y;
                    }
                }
            }

            var result = new ImageDiffResult
            {
                SizeA = new Size(widthA, heightA),
                SizeB = new Size(widthB, heightB),
                DiffPixels = diff,
                ComparedPixels = compared,
                DiffBounds = diff == 0 ? Rectangle.Empty : Rectangle.FromLTRB(left, top, right + 1, bottom + 1),
            };

            if (diffImagePath != null) SaveDiffImage(diffImagePath, output, width, height, result.DiffBounds);
            return result;
        }

        private static int[] LoadPixels(string path, out int width, out int height)
        {
            // new Bitmap(path) はファイルをロックし続けるので、読み込んだらすぐ閉じる
            using (var original = new Bitmap(path))
            using (var bitmap = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb))
            {
                using (var g = Graphics.FromImage(bitmap)) g.DrawImage(original, 0, 0, original.Width, original.Height);
                width = bitmap.Width;
                height = bitmap.Height;
                var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    var pixels = new int[width * height];
                    for (var y = 0; y < height; y++)
                    {
                        Marshal.Copy(IntPtr.Add(data.Scan0, y * data.Stride), pixels, y * width, width);
                    }
                    return pixels;
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }
        }

        private static bool[] BuildMask(int width, int height, IList<Rectangle> masks)
        {
            var mask = new bool[width * height];
            if (masks == null) return mask;
            foreach (var m in masks)
            {
                var r = Rectangle.Intersect(m, new Rectangle(0, 0, width, height));
                for (var y = r.Top; y < r.Bottom; y++)
                {
                    for (var x = r.Left; x < r.Right; x++) mask[y * width + x] = true;
                }
            }
            return mask;
        }

        private static int MaxChannelDiff(int a, int b)
        {
            var dr = Math.Abs(((a >> 16) & 0xFF) - ((b >> 16) & 0xFF));
            var dg = Math.Abs(((a >> 8) & 0xFF) - ((b >> 8) & 0xFF));
            var db = Math.Abs((a & 0xFF) - (b & 0xFF));
            return Math.Max(dr, Math.Max(dg, db));
        }

        /// <summary>一致した部分は、差異の赤が目立つように薄いグレーにする。</summary>
        private static int Faded(int color)
        {
            var gray = (((color >> 16) & 0xFF) * 3 + ((color >> 8) & 0xFF) * 6 + (color & 0xFF)) / 10;
            var light = 255 - (255 - gray) / 3;
            return unchecked((int)0xFF000000) | (light << 16) | (light << 8) | light;
        }

        private static void SaveDiffImage(string path, int[] pixels, int width, int height, Rectangle diffBounds)
        {
            using (var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    for (var y = 0; y < height; y++)
                    {
                        Marshal.Copy(pixels, y * width, IntPtr.Add(data.Scan0, y * data.Stride), width);
                    }
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                if (!diffBounds.IsEmpty)
                {
                    // 小さな差異も見落とさないよう、差異の範囲を枠で囲む
                    using (var g = Graphics.FromImage(bitmap))
                    using (var pen = new Pen(Color.Red, 2))
                    {
                        var frame = Rectangle.Inflate(diffBounds, 3, 3);
                        frame.Intersect(new Rectangle(0, 0, width - 1, height - 1));
                        g.DrawRectangle(pen, frame);
                    }
                }
                bitmap.Save(path, ImageFormat.Png);
            }
        }
    }
}
