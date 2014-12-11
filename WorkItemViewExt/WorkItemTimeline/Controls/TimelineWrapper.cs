using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using YL.Timeline.Controls;
using YL.Timeline.Entities;

namespace YL.WorkItemViewExt.WorkItemTimeline.Controls
{
	public class TimelineWrapper : UserControl
	{
		/*public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register("Items", typeof(Item[]),
			typeof(TimelineWrapper),
			new UIPropertyMetadata(null, (d, doa) =>
				{
					var host = (TimelineWrapper)d;
					host._timeline.Items = ((Item[])doa.NewValue);
				}));*/

		private readonly ControlTimelinePane _timeline;

		/*public Item[] Items
		{
			get
			{
				return _timeline.Items;
			}
			set
			{
				_timeline.Items = value;
			}
		}*/

		public TimelineWrapper(TimelineModel model, Action<string> logger)
		{
			//var controllerProperty = ControlTimeLine.ControllerProperty;
			_timeline = new ControlTimelinePane(model);
			var timeLine = YL.Timeline.Controls.Behind.Helpers.FindChildrens<ControlTimeLine>(_timeline).FirstOrDefault();
			if (timeLine != null)
			{
				var controller = ControlTimeLine.GetController(timeLine);
				controller.Logger = logger;
			}
			Content = _timeline;
		}
	}
}
