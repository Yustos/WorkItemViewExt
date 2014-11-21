using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YL.Timeline.Controls.Behind.Entities
{
	internal sealed class AggregatedPositions
	{
		internal int Apex { get; set; }

		internal IDictionary<int, int> Aggregations { get; set; }
	}
}
