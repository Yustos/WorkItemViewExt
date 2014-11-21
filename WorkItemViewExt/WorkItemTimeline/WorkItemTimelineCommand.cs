using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YL.WorkItemViewExt.WorkItemTimeline.Controls;

namespace YL.WorkItemViewExt.WorkItemTimeline
{
	internal class WorkItemTimelineCommand : IWorkItemViewExtCommand
	{
		void IWorkItemViewExtCommand.Execute(IServiceProvider serviceProvider, Microsoft.TeamFoundation.WorkItemTracking.Client.WorkItem[] selectedWorkItems)
		{
			var pane = AcquirePane(serviceProvider);
			pane.AddWorkItems(selectedWorkItems);
		}

		private WorkItemTimelinePane AcquirePane(IServiceProvider serviceProvider)
		{
#warning Cast serviceProvider to Package.
			var package = (Package)serviceProvider.GetService(typeof(Package));
			var window = package.FindToolWindow(typeof(WorkItemTimelinePane), 0, true);
			if (window == null || window.Frame == null)
			{
				throw new NotSupportedException("Pane not found.");
			}
			var windowFrame = (IVsWindowFrame)window.Frame;
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
			return (WorkItemTimelinePane)window;
		}
	}
}
