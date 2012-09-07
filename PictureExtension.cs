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
using System.IO;

namespace GoodYeyGenerator
{
	public static class PictureExtension
	{
		/// <summary>
		/// 画像のオリジナル サイズを取得します。
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static Size GetOriginalSize(Stream stream)
		{
			if (stream.CanSeek)
				stream.Seek(0, SeekOrigin.Begin);

			var buf = new byte[8];
			while (stream.Read(buf, 0, 2) == 2 && buf[0] == 0xff)
			{
				if (buf[1] == 0xc0 && stream.Read(buf, 0, 7) == 7)
					return new Size(buf[5] * 256 + buf[6], buf[3] * 256 + buf[4]);
				else if (buf[1] != 0xd8)
				{
					if (stream.Read(buf, 0, 2) == 2)
						stream.Position += buf[0] * 256 + buf[1] - 2;
					else
						break;
				}
			}

			return Size.Empty;
		}

		/// <summary>
		/// 画像一覧などから取得したPhotoStreamを、MemoryStreamにして書き込み可能にします。
		/// </summary>
		/// <param name="photoStream"></param>
		/// <returns></returns>
		public static Stream PhotoStreamToMemoryStream(Stream photoStream)
		{
			var ms = new MemoryStream();
			var readByte = 1000;

			photoStream.Seek(0, SeekOrigin.Begin);
			while (readByte > 0)
			{
				var bytes = new byte[4096];
				readByte = photoStream.Read(bytes, 0, bytes.Length);
				if (readByte > 0)
					ms.Write(bytes, 0, bytes.Length);
			}

			return ms;
		}
	}
}
