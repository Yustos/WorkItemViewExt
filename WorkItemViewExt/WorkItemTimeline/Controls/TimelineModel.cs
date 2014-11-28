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
using YL.Timeline.Interaction;

namespace YL.WorkItemViewExt.WorkItemTimeline.Controls
{
	public class TimelineModel : INotifyPropertyChanged
	{
		private readonly TimelineService _service;

		public event PropertyChangedEventHandler PropertyChanged;

		private Item[] _items;
		private ControlRecord[] _records;

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

#warning Must be Record type
		public ControlRecord[] SelectedRecords
		{
			get
			{
				return _records;
			}
			set
			{
				_records = value;
				OnPropertyChanged();
			}
		}

		public ICommand LoadInfoCommand
		{
			get;
			set;
		}

		internal TimelineModel(TimelineService service)
		{
			_service = service;
			LoadInfoCommand = new LoaderCommand((id, rev) =>
				{
					return _service.GetChanges(id, rev);
				});
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
