using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YL.Timeline.Entities;

namespace YL.WorkItemViewExt.WorkItemTimeline
{
	internal sealed class TimelineService
	{
		private readonly ITeamFoundationContext _context;

		internal TimelineService(ITeamFoundationContext context)
		{
			_context = context;
		}

		internal YL.Timeline.Interaction.Field[] GetFields(int id, int rev)
		{
			var store = _context.TeamProjectCollection.GetService<WorkItemStore>();
			var workItem = store.GetWorkItem(id);
			var revision = workItem.Revisions.OfType<Revision>().FirstOrDefault(r => Convert.ToInt32(r.Fields[CoreField.Rev].Value) == rev);
			if (revision == null)
			{
				return new YL.Timeline.Interaction.Field[0];
			}
			return revision.Fields.OfType<Field>().
				Select(f => new YL.Timeline.Interaction.Field { Name = f.Name, OriginalValue = f.OriginalValue, Value = f.Value }).ToArray();
		}

		internal Item[] GetItems(WorkItem[] workItems)
		{
			var items = new Dictionary<WorkItem, Item>();
			foreach (var workItem in workItems)
			{
				items.Add(workItem, MapItem(workItem));
				foreach (var relatedItem in GetRelatedItems(workItem))
				{
					items.Add(relatedItem.Item1, relatedItem.Item2);
				}
			}
			FillLinks(items);
			return items.Values.ToArray();
		}

		private IEnumerable<Tuple<WorkItem, Item>> GetRelatedItems(WorkItem workItem)
		{
			return workItem.Links.OfType<RelatedLink>().
				Select(l => workItem.Store.GetWorkItem(l.RelatedWorkItemId)).
				Select(w => Tuple.Create(w, MapItem(w)));
		}

		private void FillLinks(Dictionary<WorkItem, Item> items)
		{
			foreach (var kvp in items)
			{
				var item = kvp.Key;
				var prevRevision = item.Revisions[0];
				var prevLinksHash = new HashSet<int>(prevRevision.Links.OfType<RelatedLink>().Select(l => l.RelatedWorkItemId));

				foreach (Revision revision in item.Revisions.Cast<Revision>().Skip(1))
				{
					var linksHash = new HashSet<int>(revision.Links.OfType<RelatedLink>().Select(l => l.RelatedWorkItemId));

					var addedLinks = linksHash.Except(prevLinksHash);
					var removedLinks = prevLinksHash.Except(linksHash);

					foreach (var added in addedLinks)
					{
						var targetWi = item.Store.GetWorkItem(added, Convert.ToDateTime(revision.Fields[CoreField.ChangedDate].Value));
						var t = FindLinkPoints(items, revision, targetWi.Revisions[targetWi.Revision - 1]);
						if (t != null)
						{
							if (t.Item1.AddedLinks == null)
							{
								t.Item1.AddedLinks = new[] { t.Item2 };
							}
							else
							{
								t.Item1.AddedLinks = t.Item1.AddedLinks.Concat(new[] { t.Item2 }).ToArray();
							}
						}
						else
						{
#warning Log?
						}
					}

					foreach (var removed in removedLinks)
					{
						var targetWi = item.Store.GetWorkItem(removed, Convert.ToDateTime(revision.Fields[CoreField.ChangedDate].Value));
						var t = FindLinkPoints(items, revision, targetWi.Revisions[targetWi.Revision - 1]);
						if (t != null)
						{
							if (t.Item1.RemovedLinks == null)
							{
								t.Item1.RemovedLinks = new[] { t.Item2 };
							}
							else
							{
								t.Item1.RemovedLinks = t.Item1.RemovedLinks.Concat(new[] { t.Item2 }).ToArray();
							}
						}
						else
						{
#warning Log?
						}
					}

					prevRevision = revision;
					prevLinksHash = linksHash;
				}
			}
		}

		private Tuple<Record, Record> FindLinkPoints(Dictionary<WorkItem, Item> items, Revision sourceItemRev, Revision targetItemRev)
		{
			Item source;
			if (!items.TryGetValue(sourceItemRev.WorkItem, out source))
			{
				return null;
			}
			var sourceRev = source.Records.FirstOrDefault(r => r.Rev == Convert.ToInt32(sourceItemRev.Fields[CoreField.Rev].Value));
			if (sourceRev == null)
			{
				return null;
			}

			Item target;
			if (!items.TryGetValue(targetItemRev.WorkItem, out target))
			{
				return null;
			}
			var targetRev = target.Records.FirstOrDefault(r => r.Rev == Convert.ToInt32(targetItemRev.Fields[CoreField.Rev].Value));
			if (targetRev == null)
			{
				return null;
			}
			return Tuple.Create(sourceRev, targetRev);
		}

		private Item MapItem(WorkItem workItem)
		{
			var item = new Item
			{
				Id = workItem.Id,
				Title = workItem.Title
			};
			item.Records = workItem.Revisions.Cast<Revision>().Select(r => MapRecord(r, item)).ToArray();
			return item;
		}

		private Record MapRecord(Revision r, Item owner)
		{
			return new Record(owner)
			{
				Rev = Convert.ToInt32(r.Fields[CoreField.Rev].Value),
				Date = Convert.ToDateTime(r.Fields[CoreField.ChangedDate].Value),
				State = Convert.ToString(r.Fields[CoreField.State].Value)
			};
		}
	}
}
