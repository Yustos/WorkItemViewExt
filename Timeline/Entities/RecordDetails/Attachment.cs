using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.Timeline.Entities.RecordDetails
{
	public sealed class Attachment
	{
		public bool? IsAdded { get; set; }

		public string Name { get; set; }

		public Uri Uri { get; set; }
	}
}
