using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt.WorkItemRelations.Entities
{
	internal class GraphLink
	{
		public int SourceId { get; set; }

		public int TargetId { get; set; }

		public string Category { get; set; }
	}
}
