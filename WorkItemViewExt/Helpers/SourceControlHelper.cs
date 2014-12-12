using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt.Helpers
{
	internal static class SourceControlHelper
	{
		internal static void ShowChangeset(EnvDTE.DTE dte, int id)
		{
			VersionControlExt vce;
			vce = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
			vce.ViewChangesetDetails(id);
		}
	}
}
