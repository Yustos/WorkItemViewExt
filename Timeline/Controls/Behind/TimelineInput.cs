using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.Behind
{
	public class TimelineInput
	{
		public DateTime Min { get; set; }

		public DateTime Max { get; set; }

		public TimeSpan Range { get; set; }

		public Item[] Items { get; private set; }

		internal TimelineInput(Item[] items)
		{
			Min = DateTime.MaxValue;
			Max = DateTime.MinValue;
			foreach (var item in items)
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
			Items = items;
		}
	}
}
