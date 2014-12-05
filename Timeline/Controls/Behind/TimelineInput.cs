using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.Behind
{
	public class TimelineInput : INotifyPropertyChanged
	{
		private Item[] _items;

		public DateTime Min { get; set; }

		public DateTime Max { get; set; }

		public TimeSpan Range { get; set; }

		public Item[] Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;
				UpdateStatValues();
				OnPropertyChanged();
			}
		}

		internal TimelineInput()
		{
			Min = DateTime.MaxValue;
			Max = DateTime.MinValue;
		}

		private void UpdateStatValues()
		{
			foreach (var item in Items)
			{
				foreach (var record in item.Records)
				{
					if (record.Date < Min)
					{
						Min = record.Date;
					}
					if (record.Date > Max)
					{
						Max = record.Date;
					}
				}
			}

			Range = Max - Min;
		}

		public event PropertyChangedEventHandler PropertyChanged;

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
