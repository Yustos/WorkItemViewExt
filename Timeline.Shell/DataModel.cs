using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YL.Timeline.Controls;
using YL.Timeline.Entities;
using YL.Timeline.Interaction;

namespace YL.Timeline.Shell
{
	public class DataModel : INotifyPropertyChanged
	{
		private Item[] _items;
		private ControlRecord[] _records;

		public event PropertyChangedEventHandler PropertyChanged;

		public DataModel(Item[] items, RevisionLoader.LoadInfoDelegate loader)
		{
			Items = items;
			LoadInfoCommand = new LoaderCommand(loader);
		}

		public Item[] Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("Items"));
				}
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
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("SelectedRecords"));
				}
			}
		}

		public ICommand LoadInfoCommand
		{
			get; set;
		}
	}
}
