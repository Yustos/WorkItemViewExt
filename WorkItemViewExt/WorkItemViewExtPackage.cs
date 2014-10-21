using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using YL.WorkItemViewExt.WorkItemRelations;
using YL.WorkItemViewExt.Helpers;
using Microsoft.VisualStudio.Progression;
using Microsoft.VisualStudio.GraphModel;
using System.IO;
using YL.WorkItemViewExt.WorkItemRelations.Entities;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Collections.Generic;

namespace YL.WorkItemViewExt
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideAutoLoad("{e13eedef-b531-4afe-9725-28a69fa4f896}")]
	[Guid(GuidList.guidWorkItemViewExtPkgString)]
	public sealed class WorkItemViewExtPackage : Package, IOleCommandTarget
	{
		public WorkItemViewExtPackage()
		{
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		private const int OlecmderrEUnknowngroup = unchecked((int)0x80040104);

		/// <summary>
		/// Execute command.
		/// </summary>
		int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint cmdId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			if (pguidCmdGroup != GuidList.guidWorkItemViewExtCmdSet)
			{
				return OlecmderrEUnknowngroup;
			}
			if (cmdId != ViewRelationsCommand.CmdidViewRelations)
			{
				return OlecmderrEUnknowngroup;
			}

			var dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
			var selectedWorkItems = WorkItemHelper.GetSelectedWorkItems(dte);

			if (selectedWorkItems == null)
			{
				return VSConstants.S_OK;
			}

			var traverser = new WorkItemTraverser(selectedWorkItems);
			var part = traverser.InitStep();

			var path = Path.Combine(Path.GetTempPath(), "WorkItems.dgml");
			var existingRender = dte.ItemOperations.IsFileOpen(path);
			var render = GetRender(dte, part, path, selectedWorkItems[0].Project.WorkItemTypes.Cast<WorkItemType>().Select(t => t.Name));
			if (existingRender)
			{
				render.Render(part);
			}

			System.Threading.Tasks.Task.Factory.StartNew(
				() =>
				{
					var aggregated = new GraphPart();

					while ((part = traverser.StepDeep()) != null)
					{
						aggregated.Nodes.AddRange(part.Nodes);
						aggregated.Links.AddRange(part.Links);
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
				});
			return VSConstants.S_OK;
		}

		/// <summary>
		/// Query command status.
		/// </summary>
		int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
		{
			if (pguidCmdGroup != GuidList.guidWorkItemViewExtCmdSet)
			{
				return OlecmderrEUnknowngroup;
			}

			if (prgCmds[0].cmdID != ViewRelationsCommand.CmdidViewRelations)
			{
				return OlecmderrEUnknowngroup;
			}

			prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED;

			var dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
			var selectedWorkItems = WorkItemHelper.GetSelectedWorkItems(dte);

			if (selectedWorkItems != null && selectedWorkItems.Length > 0)
			{
				prgCmds[0].cmdf = prgCmds[0].cmdf | (uint)OLECMDF.OLECMDF_ENABLED;
			}

			return VSConstants.S_OK;
		}

		private GraphRender GetRender(EnvDTE.DTE dte, GraphPart part, string path, IEnumerable<string> types)
		{
			var serializer = new GraphSerializer();
			serializer.GenerateDGML(path, part, types);

			var window = dte.ItemOperations.OpenFile(path);
			var automation = window.Object as GraphControlAutomationObject;
			if (automation == null)
			{
				throw new ApplicationException("Missing graph control automation.");
			}

#warning Suppress error
			File.Delete(path);

			var graph = automation.Graph;
			return new GraphRender(graph);
		}
	}
}
