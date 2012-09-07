using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;


namespace GoodYeyGenerator
{
	public static class WriteableBitmapExtension
	{
		/// <summary>
		/// 画像をグレースケール化します。
		/// </summary>
		/// <param name="bitmap"></param>
		public static void ToGrayScale(this WriteableBitmap bitmap)
		{
			var pixel = new int[bitmap.Pixels.Length];
			pixel = bitmap.Pixels;

			for (var p = 0; p < pixel.Length; p++)
			{
				var color = pixel[p];
				var A = (byte)((color & 0xFF000000) >> 24);
				var R = (byte)((color & 0x00FF0000) >> 16);
				var G = (byte)((color & 0x0000FF00) >> 8);
				var B = (byte)((color & 0x000000FF));

				//var Gr = (byte)((0.3 * R + 0.59 * G + 0.11 * B) * (1 + ((float)B / 100)));
				var Gr = (byte)((R + G + B) / 3);

				bitmap.Pixels[p] = ((int)A << 24) | ((int)Gr << 16) | ((int)Gr << 8) | (int)Gr;
			}
		}

		/// <summary>
		/// BitmapImage へ変換します。
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		public static BitmapImage ToBitmapImage(this WriteableBitmap bitmap)
		{
			using (var st = new MemoryStream())
			{
				bitmap.SaveJpeg(st, bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);

				var bi = new BitmapImage();
				bi.SetSource(st);

				return bi;
			}
		}

		/// <summary>
		/// WriteableBitmap に UIElement をレンダーして保存します。
		/// </summary>
		/// <param name="bitmap"></param>
		/// <param name="elements"></param>
		/// <remarks>UIElement は最後に追加したのが上のレイヤーになります。</remarks>
		/// <returns></returns>
		public static Stream RenderAndSave(this WriteableBitmap bitmap, params UIElement[] elements)
		{
			var b = bitmap.Render(elements);

			var stream = new MemoryStream();
			b.SaveJpeg(stream, b.PixelWidth, b.PixelHeight, 0, 100);

			return stream;
		}

		/// <summary>
		/// WriteableBitmap に UIElement をレンダリングします。
		/// </summary>
		/// <param name="bitmap"></param>
		/// <param name="elements"></param>
		/// <remarks>UIElement は最後に追加したのが上のレイヤーになります。</remarks>
		/// <returns></returns>
		public static WriteableBitmap Render(this WriteableBitmap bitmap, params UIElement[] elements)
		{
			var b = bitmap.Clone();

			foreach (var item in elements)
			{
				b.Render(item, null);
			}

			b.Invalidate();

			return b;
		}
	}
}
