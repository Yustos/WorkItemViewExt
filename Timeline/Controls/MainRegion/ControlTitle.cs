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
using YL.Timeline.Entities;

namespace YL.Timeline.Controls
{
	public class ControlTitle : UserControl
	{
		public ControlTitle()
		{
			var dock = new DockPanel { LastChildFill = true };
			var idText = new TextBox
			{
				FontWeight = FontWeights.Bold,
				BorderThickness = new Thickness(0),
				IsReadOnly = true,
				Background = Brushes.Transparent
			};
			idText.SetBinding(TextBox.TextProperty, new Binding("Id"));
			dock.Children.Add(idText);

			var titleText = new TextBlock
			{
				Margin = new Thickness(1)
			};
			titleText.SetBinding(TextBlock.TextProperty, new Binding("Title"));
			dock.Children.Add(titleText);

			Content = dock;
		}
	}
}
