using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;

namespace GoodYeyGenerator
{
	public partial class MainPage : PhoneApplicationPage
	{
		WriteableBitmap originBitmap;
		WriteableBitmap editedBitmap;

		ApplicationBarIconButton fileChooseButton;
		ApplicationBarIconButton fileSaveButton;
		ApplicationBarMenuItem aboutItem;

		// コンストラクター
		public MainPage()
		{
			InitializeComponent();

			// ApplicationBarItemを追加
			this.fileChooseButton = new ApplicationBarIconButton
			{
				IconUri = new Uri("/icons/appbar.folder.rest.png", UriKind.Relative),
				Text = "読み込み"
			};
			this.fileSaveButton = new ApplicationBarIconButton
			{
				IconUri = new Uri("/icons/appbar.save.rest.png", UriKind.Relative),
				Text = "保存",
				IsEnabled = false
			};
			this.aboutItem = new ApplicationBarMenuItem
			{
				Text = "バージョン情報"
			};

			this.ApplicationBar.Buttons.Add(this.fileChooseButton);
			this.ApplicationBar.Buttons.Add(this.fileSaveButton);
			this.ApplicationBar.MenuItems.Add(this.aboutItem);

			this.fileChooseButton.Click += new EventHandler(fileChooseButton_Click);
			this.fileSaveButton.Click += new EventHandler(fileSaveButton_Click);
			this.aboutItem.Click += new EventHandler(aboutItem_Click);
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.pictureImage.Source == null)
			{
				this.startMessageShow.Begin();
			}
			else
			{
				this.startMessageGrid.Visibility = System.Windows.Visibility.Collapsed;
			}
		}

		private void fileChooseButton_Click(object sender, EventArgs e)
		{
			// 画像を選ぶ
			var task = new PhotoChooserTask
			{
				PixelHeight = 800,
				PixelWidth = 600,
				ShowCamera = true
			};

			task.Completed += (ps, pe) =>
				{
					if (pe.TaskResult == TaskResult.OK && pe.Error == null)
					{
						this.originBitmap = ConvertToWriteableBitmap(pe.ChosenPhoto);

						this.editedBitmap = ApplyYey(Grayscare(this.originBitmap));
						this.pictureImage.Source = editedBitmap.ToBitmapImage();

						this.fileSaveButton.IsEnabled = true;
						this.startMessageGrid.Visibility = System.Windows.Visibility.Collapsed;
					}
					else if (pe.TaskResult == TaskResult.Cancel && pe.Error != null)
					{
						MessageBox.Show(
							"PCでZune Softwareと同期中の可能性があります。\n" +
							"Zune Softwareを終了するか、端末をPCから切断してもう一度実行して下さい。",
							"エラー",
							MessageBoxButton.OK);
					}
				};

			task.Show();
		}

		private void fileSaveButton_Click(object sender, EventArgs e)
		{
			this.editedBitmap.SaveToMediaLibrary(DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg");

			MessageBox.Show("保存しました。");
		}

		void aboutItem_Click(object sender, EventArgs e)
		{
			this.NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
		}

		/// <summary>
		/// ストリームを WriteableBitmap に変換します。
		/// </summary>
		/// <param name="st"></param>
		/// <returns></returns>
		private WriteableBitmap ConvertToWriteableBitmap(Stream st)
		{
			using (var s = PictureExtension.PhotoStreamToMemoryStream(st))
			{
				try
				{
					var size = PictureExtension.GetOriginalSize(s);
					WriteableBitmap bitmap = null;

					bitmap = new WriteableBitmap((int)size.Width, (int)size.Height);
					// WriteableBitmapに画像を読み込ませる
					s.Seek(0, SeekOrigin.Begin);
					bitmap.LoadJpeg(s);

					return bitmap;
				}
				catch
				{
					return null;
				}
			}
		}

		/// <summary>
		/// 画像をグレースケールにします。
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		private WriteableBitmap Grayscare(WriteableBitmap bitmap)
		{
			var edit = bitmap.Clone();

			edit.ToGrayScale();

			return edit;

			//using (var s = PictureExtension.PhotoStreamToMemoryStream(bitmap))
			//{
			//    var size = PictureExtension.GetOriginalSize(s);
			//    WriteableBitmap edit = null;
				
			//    this.originBitmap = new WriteableBitmap((int)size.Width, (int)size.Height);
			//    edit = new WriteableBitmap((int)size.Width, (int)size.Height);

			//    // WriteableBitmapに画像を読み込ませる
			//    s.Seek(0, SeekOrigin.Begin);
			//    this.originBitmap.LoadJpeg(s);
			//    // editableの方にも同じのをコピー
			//    edit = this.originBitmap.Clone();

			//    // グレスケ化
			//    edit = this.originBitmap.ToGrayScale();

			//    return edit;
			//}
		}

		/// <summary>
		/// 画像に遺影を重ねます。
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		private WriteableBitmap ApplyYey(WriteableBitmap bitmap)
		{
			var wb = bitmap.Clone();
			var file = Application.GetResourceStream(new Uri("photoframe.png", UriKind.Relative));

			var yey = new WriteableBitmap(wb.PixelWidth, wb.PixelHeight);
			yey.LoadJpeg(file.Stream);

			for (int i = 0; i < yey.Pixels.Length; i++)
			{
				var oPx = yey.Pixels[i];
				var a2 = Convert.ToByte((oPx >> 24) & 0xff);
				var ax2 = (double)a2 / 255;

				if (a2 == 0)
				{
					// 透明なので下地を描画
				}
				else if (a2 == 255)
				{
					// 不透明なので上に書き込み
					wb.Pixels[i] = yey.Pixels[i];
				}
				else
				{
					// 下地の画像の色を取り出す
					var a = Convert.ToByte((wb.Pixels[i] >> 24) & 0xff);
					var r = Convert.ToByte((wb.Pixels[i] >> 16) & 0xff);
					var g = Convert.ToByte((wb.Pixels[i] >> 8) & 0xff);
					var b = Convert.ToByte((wb.Pixels[i]) & 0xff);
					// 枠の色を取り出す
					var r2 = Convert.ToByte((oPx >> 16) & 0xff);
					var g2 = Convert.ToByte((oPx >> 8) & 0xff);
					var b2 = Convert.ToByte((oPx) & 0xff);

					// アルファ値を元に合成
					var a3 = Convert.ToByte((a * (1.0 - ax2) + a2 * ax2));
					var r3 = Convert.ToByte((r * (1.0 - ax2) + r2 * ax2));
					var g3 = Convert.ToByte((g * (1.0 - ax2) + g2 * ax2));
					var b3 = Convert.ToByte((b * (1.0 - ax2) + b2 * ax2));

					wb.Pixels[i] = a3 << 24 | r3 << 16 | g3 << 8 | b3;
				}
			}

			wb.Invalidate();
			return wb;
		}
	}
}