using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YL.Timeline.Entities.RecordDetails;

namespace YL.Timeline.Entities
{
	public class Record
	{
		public delegate RevisionChanges LoadDetailsDelegate(Record record);

		public int Rev { get; set; }

		public DateTime Date { get; set; }

		public string State { get; set; }

		public Field[] DisplayFields { get; set; }

		public Record[] AddedLinks { get; set; }

		public Record[] RemovedLinks { get; set; }

		public int AddedAttachments { get; set; }

		public int RemovedAttachments { get; set; }

		public int AddedChangesets { get; set; }

		public int RemovedChangesets { get; set; }

		public Item Owner { get; private set; }

		public RevisionChanges Details { get; private set; }

		private readonly LoadDetailsDelegate _loader;

		public Record(Item owner, LoadDetailsDelegate loader)
		{
			Owner = owner;
			_loader = loader;
		}

		internal void EnsureDetails()
		{
			if (Details == null)
			{
				Details = _loader(this);
			}
		}
	}
}
