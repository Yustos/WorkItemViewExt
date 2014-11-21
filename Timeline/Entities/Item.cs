using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YL.Timeline.Entities
{
	public class Item
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public Record[] Records { get; set; }
	}
}
