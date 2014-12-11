using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YL.Timeline.Controls;
using YL.Timeline.Entities;

namespace YL.WorkItemViewExt.WorkItemTimeline.Controls
{
	public class TimelineModel : INotifyPropertyChanged
	{
		private readonly TimelineService _service;

		public event PropertyChangedEventHandler PropertyChanged;

		private Item[] _items;

		public Item[] Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}

		internal TimelineModel(TimelineService service)
		{
			_service = service;
		}

		internal void AddWorkItems(WorkItem[] workItems)
		{
			Items = _service.GetItems(workItems);
		}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
