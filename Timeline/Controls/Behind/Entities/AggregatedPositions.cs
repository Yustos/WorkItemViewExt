using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YL.Timeline.Controls.Behind.Entities
{
	public sealed class AggregatedPositions
	{
		public int Apex { get; set; }

		public IDictionary<double, int> Aggregations { get; set; }
	}
}
