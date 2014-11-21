using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using YL.WorkItemViewExt.Helpers;
using YL.WorkItemViewExt.WorkItemTimeline.Controls;
using System.Collections.Generic;

namespace YL.WorkItemViewExt
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideAutoLoad("{e13eedef-b531-4afe-9725-28a69fa4f896}")]
	[ProvideToolWindow(typeof(WorkItemTimelinePane), Style = VsDockStyle.Tabbed, Transient = true)]
	[Guid(GuidList.guidWorkItemViewExtPkgString)]
	public sealed class WorkItemViewExtPackage : Package, IOleCommandTarget
	{
		private static readonly Dictionary<uint, IWorkItemViewExtCommand> _commands = new Dictionary<uint, IWorkItemViewExtCommand>();

		static WorkItemViewExtPackage()
		{
			_commands.Add(WorkItemViewExtCommands.CmdidViewRelations, new WorkItemRelations.WorkItemRelationsCommand());
			_commands.Add(WorkItemViewExtCommands.CmdidViewTimeline, new WorkItemTimeline.WorkItemTimelineCommand());
		}

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
			IWorkItemViewExtCommand command;
			if (!_commands.TryGetValue(cmdId, out command))
			{
				return OlecmderrEUnknowngroup;
			}

			var dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
			var selectedWorkItems = WorkItemHelper.GetSelectedWorkItems(dte);

			if (selectedWorkItems == null)
			{
				return VSConstants.S_OK;
			}

			command.Execute(this, selectedWorkItems);
			
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

			if (!_commands.ContainsKey(prgCmds[0].cmdID))
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
	}
}
