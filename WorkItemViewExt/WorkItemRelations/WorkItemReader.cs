using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt.WorkItemRelations
{
	internal class WorkItemReader
	{
		private readonly WorkItemStore _store;

		internal WorkItemReader(WorkItemStore store)
		{
			_store = store;
		}

		internal IEnumerable<IGrouping<int, int>> ReadLinks(int[] ids)
		{
#warning Split to partitions. Display link types.
			var query = new Query(_store, string.Format("SELECT [System.Id] FROM [WorkItemLinks] " + 
				"WHERE [System.Links.LinkType] <> '' AND [Source].[System.Id] IN ({0})", string.Join(",", ids)));
			var queryResult = query.RunLinkQuery();

			return queryResult.Where(l => l.SourceId != 0).GroupBy(l => l.SourceId, l => l.TargetId);
		}

		internal void FillInfo(IDictionary<int, WorkItem> items)
		{
			var query = new Query(_store, "SELECT [System.Id], [System.WorkItemType], [System.Title] FROM [WorkItems]", items.Keys.ToArray());
			var queryResult = query.RunQuery();
			foreach (WorkItem workItem in queryResult)
			{
				items[workItem.Id] = workItem;
			}
		}
	}
}
