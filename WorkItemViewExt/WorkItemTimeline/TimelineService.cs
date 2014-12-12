using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YL.Timeline.Entities;
using Details = YL.Timeline.Entities.RecordDetails;

namespace YL.WorkItemViewExt.WorkItemTimeline
{
	internal sealed class TimelineService
	{
		private readonly ITeamFoundationContext _context;
		private readonly Action<string> _logger;

		internal TimelineService(ITeamFoundationContext context, Action<string> logger)
		{
			_context = context;
			_logger = logger;
		}

		internal Details.RevisionChanges GetChanges(int id, int rev)
		{
			_logger(string.Format("Load info for work item {0} revision {1}.{2}", id, rev, Environment.NewLine));
			var result = new Details.RevisionChanges();
			var store = _context.TeamProjectCollection.GetService<WorkItemStore>();
			var workItem = store.GetWorkItem(id);
			var revision = workItem.Revisions.OfType<Revision>().
				FirstOrDefault(r => Convert.ToInt32(r.Fields[CoreField.Rev].Value) == rev);
			if (revision == null)
			{
				return null;
			}
			result.Fields = revision.Fields.OfType<Field>().
				Select(f => new Details.Field { Name = f.Name, OriginalValue = f.OriginalValue, Value = f.Value }).ToArray();

			var prevRevision = workItem.Revisions.OfType<Revision>().
				FirstOrDefault(r => Convert.ToInt32(r.Fields[CoreField.Rev].Value) == rev - 1);
			if (prevRevision != null)
			{
				var prevAttachments = new HashSet<int>(prevRevision.Attachments.OfType<Attachment>().Select(a => a.Id));
				var currentAttachments = new HashSet<int>(revision.Attachments.OfType<Attachment>().Select(a => a.Id));
				var attachments = revision.Attachments.OfType<Attachment>().
					Select(a => new Details.Attachment { IsAdded = prevAttachments.Contains(a.Id) ? null : (bool?)true, Uri = a.Uri, Name = a.Name }).ToList();
				attachments.AddRange(prevRevision.Attachments.OfType<Attachment>().Where(a => !currentAttachments.Contains(a.Id)).
					Select(a => new Details.Attachment { IsAdded = false, Uri = a.Uri, Name = a.Name }));
				result.Attachments = attachments.ToArray();

				var prevChangesets = new HashSet<string>(prevRevision.Links.OfType<ExternalLink>().Select(a => a.LinkedArtifactUri));
				var currentChangesets = new HashSet<string>(revision.Links.OfType<ExternalLink>().Select(a => a.LinkedArtifactUri));
				var changesets = revision.Links.OfType<ExternalLink>().
					Select(a => MapChangeset(a, prevChangesets.Contains(a.LinkedArtifactUri) ? null : (bool?)true)).ToList();
				changesets.AddRange(prevRevision.Links.OfType<ExternalLink>().Where(a => !currentChangesets.Contains(a.LinkedArtifactUri)).
					Select(a => MapChangeset(a, false)));
				result.Changesets = changesets.ToArray();
			}
			else
			{
				result.Attachments = revision.Attachments.OfType<Attachment>().
					Select(a => new Details.Attachment { IsAdded = true, Uri = a.Uri, Name = a.Name }).ToArray();
				result.Changesets = revision.Links.OfType<ExternalLink>().
					Select(l => MapChangeset(l, true)).ToArray();
			}

			return result;
		}

		private Details.Field[] GetFields(int id, int rev)
		{
			var store = _context.TeamProjectCollection.GetService<WorkItemStore>();
			var workItem = store.GetWorkItem(id);
			var revision = workItem.Revisions.OfType<Revision>().FirstOrDefault(r => Convert.ToInt32(r.Fields[CoreField.Rev].Value) == rev);
			if (revision == null)
			{
				return new Details.Field[0];
			}
			return revision.Fields.OfType<Field>().
				Select(f => new Details.Field { Name = f.Name, OriginalValue = f.OriginalValue, Value = f.Value }).ToArray();
		}

		internal Item[] GetItems(WorkItem[] workItems)
		{
			var loader = new Record.LoadDetailsDelegate((r) =>
			{
				return GetChanges(r.Owner.Id, r.Rev);
			});
			var items = new Dictionary<WorkItem, Item>(new WorkItemEqualityComparer());
			foreach (var workItem in workItems)
			{
				items.Add(workItem, MapItem(workItem, loader));
				foreach (var relatedItem in GetRelatedItems(workItem, loader))
				{
					items.Add(relatedItem.Item1, relatedItem.Item2);
				}
			}
			FillLinks(items);
			return items.Values.ToArray();
		}

		private IEnumerable<Tuple<WorkItem, Item>> GetRelatedItems(WorkItem workItem, Record.LoadDetailsDelegate loader)
		{
			return workItem.Links.OfType<RelatedLink>().
				Select(l => workItem.Store.GetWorkItem(l.RelatedWorkItemId)).
				Select(w => Tuple.Create(w, MapItem(w, loader)));
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
							_logger(string.Format("Missing added link from {0} [{1}] to {2} [{3}].{4}",
								revision.WorkItem.Id,
								revision.Index,
								targetWi.Revisions[targetWi.Revision - 1].WorkItem.Id,
								targetWi.Revisions[targetWi.Revision - 1].Index,
								Environment.NewLine));
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
							_logger(string.Format("Missing removed link from {0} [{1}] to {2} [{3}].{4}",
								revision.WorkItem.Id,
								revision.Index,
								targetWi.Revisions[targetWi.Revision - 1].WorkItem.Id,
								targetWi.Revisions[targetWi.Revision - 1].Index,
								Environment.NewLine));
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

		private Item MapItem(WorkItem workItem, Record.LoadDetailsDelegate loader)
		{
			var item = new Item
			{
				Id = workItem.Id,
				Title = workItem.Title
			};
			var records = new List<Record>();
			Revision prev = null;
			foreach (Revision r in workItem.Revisions)
			{
				records.Add(MapRecord(r, prev, item, loader));
				prev = r;
			}
			item.Records = records.ToArray();
			return item;
		}

		private Record MapRecord(Revision r, Revision prev, Item owner, Record.LoadDetailsDelegate loader)
		{
			var prevChangesetHash = new HashSet<string>(prev == null ? Enumerable.Empty<string>() : prev.Links.OfType<ExternalLink>().Select(a => a.LinkedArtifactUri));
			var prevAttachmentsHash = new HashSet<int>(prev == null ? Enumerable.Empty<int>() : prev.Attachments.OfType<Attachment>().Select(a => a.Id));
			var changesetHash = new HashSet<string>(r.Links.OfType<ExternalLink>().Select(a => a.LinkedArtifactUri));
			var attachmentsHash = new HashSet<int>(r.Attachments.OfType<Attachment>().Select(a => a.Id));

			return new Record(owner, loader)
			{
				Rev = Convert.ToInt32(r.Fields[CoreField.Rev].Value),
				Date = Convert.ToDateTime(r.Fields[CoreField.ChangedDate].Value),
				State = Convert.ToString(r.Fields[CoreField.State].Value),
				AddedAttachments = attachmentsHash.Except(prevAttachmentsHash).Count(),
				RemovedAttachments = prevAttachmentsHash.Except(attachmentsHash).Count(),
				AddedChangesets = changesetHash.Except(prevChangesetHash).Count(),
				RemovedChangesets = prevChangesetHash.Except(changesetHash).Count(),
			};
		}

		private Details.Changeset MapChangeset(ExternalLink link, bool? isAdded)
		{
			ArtifactId artifactId = LinkingUtilities.DecodeUri(link.LinkedArtifactUri);
			var id = int.Parse(artifactId.ToolSpecificId);

			return new Details.Changeset
				{
					Id = id,
					IsAdded = isAdded,
					Uri = new Uri(link.LinkedArtifactUri),
					Comment = link.Comment
				};
		}
	}
}
