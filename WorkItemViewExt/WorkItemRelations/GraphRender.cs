using Microsoft.VisualStudio.GraphModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YL.WorkItemViewExt.WorkItemRelations.Entities;

namespace YL.WorkItemViewExt.WorkItemRelations
{
	internal class GraphRender
	{
		private readonly Graph _graph;

		private readonly Dictionary<string, GraphCategory> _categories;

		private readonly GraphProperty _projectProperty;

		internal GraphRender(Graph graph)
		{
			_graph = graph;
			_categories = graph.DocumentSchema.Categories.ToDictionary(c => c.Id, c => c);
			_projectProperty = _graph.DocumentSchema.FindProperty("Project");
		}

		internal void Render(GraphPart part)
		{
			foreach (var node in part.Nodes)
			{
				var graphNode = _graph.Nodes.GetOrCreate(node.Id.ToString());
				graphNode.Label = node.Title;
				graphNode.SetValue<string>(_projectProperty, node.Project);
				GraphCategory category;
				if (_categories.TryGetValue(node.Category, out category))
				{
					graphNode.AddCategory(category);
				}
			}

			foreach (var link in part.Links)
			{
				var graphLink = _graph.Links.GetOrCreate(
					_graph.Nodes.Get(link.SourceId.ToString()),
					_graph.Nodes.Get(link.TargetId.ToString()));
				graphLink.Label = link.LinkEndType;

				GraphCategory category;
				if (_categories.TryGetValue(link.Category, out category))
				{
					graphLink.AddCategory(category);
				}

				_graph.Links.Add(graphLink);
			}
		}

		internal void Group()
		{
			var groups = new Dictionary<string, Microsoft.VisualStudio.GraphModel.GraphNode>();
			foreach (var node in _graph.Nodes.Where(n => !n.IsGroup))
			{
				Microsoft.VisualStudio.GraphModel.GraphNode projectNode;
				var projectName = node.GetValue<string>(_projectProperty);
				if (!groups.TryGetValue(projectName, out projectNode))
				{
					projectNode = _graph.Nodes.GetOrCreate(projectName);
					groups[projectName] = projectNode;
				}
				
				var link = _graph.Links.GetOrCreate(projectNode, node);
				link.SetValue<bool>(GraphCommonSchema.IsContainment, true);
			}

			foreach (var projectNode in groups.Values)
			{
				projectNode.SetValue<GraphGroupStyle>(GraphCommonSchema.Group, GraphGroupStyle.Expanded);
			}
		}

		internal GraphTransactionScope BeginUpdate()
		{
			return _graph.BeginUpdate(Guid.NewGuid(), "fill relations", UndoOption.Merge);
		}
	}
}
