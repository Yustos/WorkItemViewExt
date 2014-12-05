using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YL.Timeline.Controls.Behind
{
	internal static class Helpers
	{
		internal static T FindParent<T>(DependencyObject child)
			where T : DependencyObject
		{
			var current = child as T;
			if (current != null)
			{
				return current;
			}

			var parentObject = VisualTreeHelper.GetParent(child);
			if (parentObject == null)
			{
				return null;
			}
			return FindParent<T>(parentObject);
		}

		internal static IEnumerable<T> FindChildrens<T>(DependencyObject depObj)
			where T : DependencyObject
		{
			if (depObj != null)
			{
				foreach (var child in LogicalTreeHelper.GetChildren(depObj).OfType<DependencyObject>())
				{
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in FindChildrens<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}

		internal static IEnumerable<T> FindVisualChildrens<T>(DependencyObject depObj)
			where T : DependencyObject
		{
			if (depObj != null)
			{
				var count = VisualTreeHelper.GetChildrenCount(depObj);
				for (var i = 0; i < count; i++)
				{
					var child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in FindVisualChildrens<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}

		internal static BitmapSource ToSource(this Bitmap bitmap)
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
