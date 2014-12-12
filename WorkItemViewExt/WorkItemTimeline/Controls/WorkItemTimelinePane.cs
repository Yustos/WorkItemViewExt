using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Integration;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using YL.WorkItemViewExt.Helpers;

namespace YL.WorkItemViewExt.WorkItemTimeline.Controls
{
	[Guid("F8CB1897-17D7-410F-8FC1-A965B93E667A")]
	public class WorkItemTimelinePane : ToolWindowPane
	{
		private TimelineModel _model;

		protected override void Initialize()
		{
			base.Initialize();

			var context = ((ITeamFoundationContextManager)GetService(typeof(ITeamFoundationContextManager))).CurrentContext;
			_model = new TimelineModel(new TimelineService(context, (message) => OutputWindowHelper.OutputString(this, message)));
			base.Content = new TimelineWrapper(_model,
				(message) => OutputWindowHelper.OutputString(this, message),
				(id) => { 
					var dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));
					SourceControlHelper.ShowChangeset(dte, id);
				});
		}

		internal void AddWorkItems(WorkItem[] workItems)
		{
			_model.AddWorkItems(workItems);
			var caption = "Work Item Timeline";
			/*if (_model.Items.Length == 1)
			{
				caption += " " + _model.Items[0].Id;
			}
			else
			{
				caption += string.Format(" ({0} items)", _model.Items.Length);
			}*/
			Caption = caption;
		}
	}
}
