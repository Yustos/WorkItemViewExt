using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt.Helpers
{
	internal static class WorkItemHelper
	{
		private static readonly object _lockToken = new object();

		internal static WorkItem[] GetSelectedWorkItems(EnvDTE.DTE dte)
		{
			var activeDocument = GetActiveDocument(dte);
			var resultDocument = activeDocument as IResultsDocument;
			if (resultDocument != null)
			{
				var selectedIds = resultDocument.SelectedItemIds;
				if (selectedIds == null || selectedIds.Length < 1)
					return null;
				var workItemStore = resultDocument.TeamProjectCollection.GetService<WorkItemStore>();
				return selectedIds.OrderBy(id => id).Select(id => workItemStore.GetWorkItem(id)).ToArray();
			}
			var workItemDocument = activeDocument as IWorkItemDocument;
			if (workItemDocument != null && !workItemDocument.IsNew)
			{
				return new[] { workItemDocument.Item };
			}
			return null;
		}
		private static IWorkItemTrackingDocument GetActiveDocument(EnvDTE.DTE dte)
		{
			if (dte.ActiveDocument != null)
			{
				var activeDocumentMoniker = dte.ActiveDocument.FullName;
				var doc = GetDocService(dte).FindDocument(activeDocumentMoniker, _lockToken);
				if (doc != null)
				{
					doc.Release(_lockToken);
				}

				return doc;
			}

			return null;
		}

		private static DocumentService GetDocService(EnvDTE.DTE dte)
		{
			return dte.GetObject("Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.DocumentService")
				as DocumentService;
		}
	}
}
