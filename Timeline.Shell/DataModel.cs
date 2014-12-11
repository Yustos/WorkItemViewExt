using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YL.Timeline.Controls;
using YL.Timeline.Entities;

namespace YL.Timeline.Shell
{
	public class DataModel : INotifyPropertyChanged
	{
		private Item[] _items;
		public event PropertyChangedEventHandler PropertyChanged;

		public DataModel(Item[] items)
		{
			Items = items;
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
	}
}
