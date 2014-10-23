using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using YL.WorkItemViewExt.WorkItemRelations.Entities;

namespace YL.WorkItemViewExt.WorkItemRelations
{
	internal class WorkItemTraverser
	{
		private WorkItem[] _currentWorkItems;

		private readonly WorkItemReader _reader;

		private readonly HashSet<int> _visitedWorkItems = new HashSet<int>();

		internal WorkItemTraverser(WorkItem[] workItems)
		{
			_currentWorkItems = workItems;
			_reader = new WorkItemReader(workItems[0].Store);
		}

		internal GraphPart InitStep()
		{
			foreach (var workItem in _currentWorkItems)
			{
				_visitedWorkItems.Add(workItem.Id);
			}

			return new GraphPart(_currentWorkItems.Select(MapToNode), null);
		}

		internal GraphPart StepDeep()
		{
			if (_currentWorkItems.Length == 0)
			{
				return null;
			}

			var part = new GraphPart();
			var relations = _reader.ReadLinks(_currentWorkItems.Select(w => w.Id).ToArray());
			var info = new Dictionary<int, WorkItem>();
			foreach (var relation in relations)
			{
				foreach (var target in relation)
				{
					if (_visitedWorkItems.Add(target.TargetId))
					{
						info.Add(target.TargetId, null);
					}
					if (target.IsForward)
					{
						part.Links.Add(new GraphLink { SourceId = relation.Key, TargetId = target.TargetId, Category = target.LinkType, LinkEndType = target.LinkEndType });
					}
				}
			}
			_reader.FillInfo(info);

			foreach (var workItem in info)
			{
				part.Nodes.Add(MapToNode(workItem.Value));
			}

			_currentWorkItems = info.Values.ToArray();
			return part;
		}

		private GraphNode MapToNode(WorkItem workItem)
		{
			try
			{
				return new GraphNode
				{
					Id = workItem.Id,
					Title = string.Format("[{0}] {1}", workItem.Id, workItem.Title),
					Project = workItem.Project.Name,
					Category = workItem.Type.Name,
					SourceLocation = workItem.Uri.ToString()
				};
			}
			catch (DeniedOrNotExistException)
			{
				return new GraphNode
				{
					Id = workItem.Id,
					Title = string.Format("[Denied] {0}", workItem.Id),
					Project = "[Unknown]",
					Category = "[Denied]"
				};
			}
		}
	}
}
