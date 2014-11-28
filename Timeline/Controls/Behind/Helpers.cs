using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace YL.Timeline.Controls.Behind
{
	internal static class Helpers
	{
		internal static T FindParent<T>(DependencyObject child) where T : DependencyObject
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
	}
}
