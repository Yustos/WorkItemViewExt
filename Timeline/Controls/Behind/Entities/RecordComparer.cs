using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.Behind.Entities
{
	internal class RecordComparer : IComparer<Record>
	{
		public int Compare(Record x, Record y)
		{
			var result = DateTime.Compare(x.Date, y.Date);
			return result == 0 ? x.Rev.CompareTo(y.Rev) : result;
		}
	}
}
