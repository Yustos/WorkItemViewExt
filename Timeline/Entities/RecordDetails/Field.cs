using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.Timeline.Entities.RecordDetails
{
	public class Field
	{
		public string Name { get; set; }

		public string ReferenceName { get; set; }

		public object OriginalValue { get; set; }

		public object Value { get; set; }

		public bool IsChangedByUser { get; set; }
	}
}
