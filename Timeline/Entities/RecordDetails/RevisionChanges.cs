using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.Timeline.Entities.RecordDetails
{
	public sealed class RevisionChanges
	{
		public string ChangedBy { get; set; }

		public string AuthorizedAs { get; set; }

		public Field[] Fields { get; set; }

		public Changeset[] Changesets { get; set; }

		public Attachment[] Attachments { get; set; }
	}
}
