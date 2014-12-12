using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt.WorkItemTimeline
{
	internal sealed class WorkItemEqualityComparer : IEqualityComparer<WorkItem>
	{
		public bool Equals(WorkItem x, WorkItem y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(WorkItem obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
