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
		private readonly ControlTimelinePane _timeline;

		public TimelineWrapper(TimelineModel model,
			Action<string> logger,
			Action<int> showChangeset)
		{
			_timeline = new ControlTimelinePane(model);
			var timeLine = YL.Timeline.Controls.Behind.Helpers.FindChildrens<ControlTimeLine>(_timeline).FirstOrDefault();
			if (timeLine != null)
			{
				var controller = ControlTimeLine.GetController(timeLine);
				controller.Logger = logger;
				controller.ShowChangeset = showChangeset;
			}
			Content = _timeline;
		}
	}
}
