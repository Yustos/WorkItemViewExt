using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YL.Timeline.Controls;
using YL.Timeline.Entities;

namespace YL.WorkItemViewExt.WorkItemTimeline.Controls
{
	public class TimelineWrapper : UserControl
	{
		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register("Items", typeof(Item[]),
			typeof(TimelineWrapper),
			new UIPropertyMetadata(null, (d, doa) =>
				{
					var host = (TimelineWrapper)d;
					host._timeline.Items = ((Item[])doa.NewValue);
				}));

		private readonly TimeLine _timeline = new TimeLine();

		public Item[] Items
		{
			get
			{
				return _timeline.Items;
			}
			set
			{
				_timeline.Items = value;
			}
		}

		public TimelineWrapper()
		{
			Content = _timeline;

			_timeline.SetBinding(TimeLine.SelectedRecordsProperty, new Binding("SelectedRecords") { Mode = BindingMode.TwoWay });
		}
	}
}
