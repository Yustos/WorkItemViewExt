using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YL.Timeline.Controls
{
	public class Title : UserControl
	{
		public Title(int id, string title)
		{
			var dock = new DockPanel { LastChildFill = true };
			dock.Children.Add(new TextBox { Text = Convert.ToString(id), FontWeight = FontWeights.Bold, BorderThickness = new Thickness(0), IsReadOnly = true, Background = Brushes.Transparent });
			dock.Children.Add(new TextBlock { Text = title, Margin = new Thickness(1) });

			Content = dock;
			Background = new LinearGradientBrush(Color.FromRgb(198, 206, 206), Color.FromRgb(218, 224, 224), 90);
		}
	}
}
