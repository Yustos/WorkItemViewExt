using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using YL.Timeline.Interaction;

namespace YL.Timeline.Controls.Fields
{
	public class ControlRevisionsView : ListBox
	{
		public ControlRevisionsView()
		{
			VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
			var factory = new FrameworkElementFactory(typeof(WrapPanel));
			factory.SetValue(WrapPanel.IsItemsHostProperty, true);
			ItemsPanel = new ItemsPanelTemplate(factory);

			var itemFactory = new FrameworkElementFactory(typeof(ControlRevisionView));
			ItemTemplate = new DataTemplate() { VisualTree = itemFactory};
		}
	}
}
