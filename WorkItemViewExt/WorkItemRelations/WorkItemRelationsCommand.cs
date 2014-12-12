using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Progression;
using Microsoft.VisualStudio.Services.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YL.WorkItemViewExt.Helpers;
using YL.WorkItemViewExt.WorkItemRelations.Entities;

namespace YL.WorkItemViewExt.WorkItemRelations
{
	internal class WorkItemRelationsCommand : IWorkItemViewExtCommand
	{
		void IWorkItemViewExtCommand.Execute(IServiceProvider serviceProvider, WorkItem[] selectedWorkItems)
		{
			var dte = (EnvDTE.DTE)serviceProvider.GetService(typeof(EnvDTE.DTE));
			var traverser = new WorkItemTraverser(selectedWorkItems);
			var part = traverser.InitStep();

			var path = Path.Combine(Path.GetTempPath(), "WorkItems.dgml");
			var existingRender = dte.ItemOperations.IsFileOpen(path);
			var render = GetRender(serviceProvider, dte, part, path,
				selectedWorkItems[0].Project.WorkItemTypes.Cast<WorkItemType>().Select(t => t.Name),
				selectedWorkItems[0].Store.WorkItemLinkTypes.Select(l => l.ReferenceName));
			if (existingRender)
			{
				render.Render(part);
			}

			System.Threading.Tasks.Task.Factory.StartNew(
				() =>
				{
					using (var progress = new ProgressBar(serviceProvider))
					{
						var aggregated = new GraphPart();

						while ((part = traverser.StepDeep()) != null)
						{
							aggregated.Nodes.AddRange(part.Nodes);
							aggregated.Links.AddRange(part.Links);
							progress.Progress(part.Nodes.Count, part.Links.Count);
						}

						Application.Current.Dispatcher.Invoke(
							() =>
							{
								using (var scope = render.BeginUpdate())
								{
									render.Render(aggregated);
									render.Group();
									scope.Complete();
								}
							});

					}
				});
		}

		private GraphRender GetRender(IServiceProvider provider, EnvDTE.DTE dte,
			GraphPart part, string path, IEnumerable<string> nodeTypes, IEnumerable<string> linkTypes)
		{
			var serializer = new GraphSerializer();
			serializer.GenerateDGML(path, part, nodeTypes, linkTypes);

			var window = dte.ItemOperations.OpenFile(path);
			var automation = window.Object as GraphControlAutomationObject;
			if (automation == null)
			{
				throw new ApplicationException("Missing graph control automation.");
			}

			try
			{
				File.Delete(path);
			}
			catch (Exception ex)
			{
				OutputWindowHelper.OutputString(provider, ex.Message);
			}

			var graph = automation.Graph;
			return new GraphRender(graph);
		}
	}
}
