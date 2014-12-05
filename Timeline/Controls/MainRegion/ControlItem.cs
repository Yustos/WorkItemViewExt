using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Controls.MainRegion.Ornament;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.MainRegion
{
	public class ControlItem : Expander
	{
		public ControlItem()
		{
			IsExpanded = true;
			ClipToBounds = true;
			BorderBrush = Brushes.LightBlue;
			BorderThickness = new Thickness(1);

			Header = new ControlTitle();
			var controlRecords = new ControlRecords();
			controlRecords.SetBinding(ControlRecords.RecordsProperty, new Binding("Records"));
			Content = controlRecords;

			Loaded += ControlItemLoaded;
		}

		private void ControlItemLoaded(object sender, RoutedEventArgs e)
		{
			// Prepare revision paths
			var adornerLayer = AdornerLayer.GetAdornerLayer(this);
			var recordControls = Helpers.FindChildrens<ControlRecord>(this);

			ControlRecord last = null;
			foreach (var child in recordControls)
			{
				if (last != null)
				{
					adornerLayer.Add(new RecordTransitionAdorner(last, last, child));
				}
				last = child;
			}
		}
	}
}
