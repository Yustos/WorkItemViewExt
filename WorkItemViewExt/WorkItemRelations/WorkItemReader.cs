using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YL.WorkItemViewExt.WorkItemRelations.Entities;

namespace YL.WorkItemViewExt.WorkItemRelations
{
	internal class WorkItemReader
	{
		private readonly WorkItemStore _store;
		private readonly Dictionary<int, WorkItemLinkTypeEnd> _linkTypes = new Dictionary<int, WorkItemLinkTypeEnd>();

		internal WorkItemReader(WorkItemStore store)
		{
			_store = store;
			foreach (var type in _store.WorkItemLinkTypes)
			{
				if (!_linkTypes.ContainsKey(type.ForwardEnd.Id))
				{
					_linkTypes.Add(type.ForwardEnd.Id, type.ForwardEnd);
				}
				if (!_linkTypes.ContainsKey(type.ReverseEnd.Id))
				{
					_linkTypes.Add(type.ReverseEnd.Id, type.ReverseEnd);
				}
			}
		}

		internal IEnumerable<IGrouping<int, LinkTarget>> ReadLinks(int[] ids)
		{
#warning Split to partitions.
			var query = new Query(_store, string.Format("SELECT [System.Id] FROM [WorkItemLinks] " + 
				"WHERE [System.Links.LinkType] <> '' AND [Source].[System.Id] IN ({0})", string.Join(",", ids)));
			var queryResult = query.RunLinkQuery();

			return queryResult.Where(l => l.SourceId != 0).GroupBy(
				l => l.SourceId,
				l => 
					{
						WorkItemLinkTypeEnd linkType;
						_linkTypes.TryGetValue(l.LinkTypeId, out linkType);
						return new LinkTarget
						{
							TargetId = l.TargetId,
							IsForward = linkType == null ? false : linkType.IsForwardLink,
							LinkType = linkType == null ? "[unknown]" : linkType.LinkType.ReferenceName,
							LinkEndType = linkType == null ? "[unknown]" : linkType.Name
						};
					});
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
