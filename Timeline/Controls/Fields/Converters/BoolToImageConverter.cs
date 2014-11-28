using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YL.Timeline.Properties;

namespace YL.Timeline.Controls.Fields.Converters
{
	public sealed class BoolToImageConverter: IValueConverter
	{
		private static readonly BitmapSource _addedSource = Bitmap2BitmapImage(Resources.Added);
		private static readonly BitmapSource _deletedSource = Bitmap2BitmapImage(Resources.Deleted);

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}
			return System.Convert.ToBoolean(value) ? _addedSource : _deletedSource;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
		{
			BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
						   bitmap.GetHbitmap(),
						   IntPtr.Zero,
						   Int32Rect.Empty,
						   BitmapSizeOptions.FromEmptyOptions());
			return (BitmapSource)i;
		}
	}
}
