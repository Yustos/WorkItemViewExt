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
using YL.Timeline.Interfaces;

namespace YL.WorkItemViewExt.WorkItemTimeline.Controls
{
	public class TimelineModel : INotifyPropertyChanged
	{
		private readonly TimelineService _service;

		public event PropertyChangedEventHandler PropertyChanged;

		private string[] _displayFields;

		private WorkItem[] _workItems;

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

		internal string[] DisplayFields
		{
			get
			{
				return _displayFields;
			}
		}

		internal IDataService Service
		{
			get
			{
				return _service;
			}
		}

		internal TimelineModel(TimelineService service)
		{
			_service = service;
		}

		internal void AddWorkItems(WorkItem[] workItems)
		{
			_workItems = workItems;
			RefreshItems();
		}

		internal void SetDisplayFields(string[] displayFields)
		{
			_displayFields = displayFields;
			RefreshItems();
		}

		private void RefreshItems()
		{
			Items = _service.GetItems(_workItems, _displayFields);
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
