using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt
{
	internal interface IWorkItemViewExtCommand
	{
		void Execute(IServiceProvider serviceProvider, WorkItem[] selectedWorkItems);
	}
}
