using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YL.Timeline.Entities
{
	public class Record
	{
		public int Rev { get; set; }

		public DateTime Date { get; set; }

		public string State { get; set; }

		public Record[] AddedLinks { get; set; }

		public Record[] RemovedLinks { get; set; }

		public int AddedAttachments { get; set; }

		public int RemovedAttachments { get; set; }

		public int AddedChangesets { get; set; }

		public int RemovedChangesets { get; set; }

		public Item Owner { get; private set; }

		public Record(Item owner)
		{
			Owner = owner;
		}
	}
}
