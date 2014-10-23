using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt.WorkItemRelations.Entities
{
	internal struct LinkTarget
	{
		public int TargetId;

		public bool IsForward;

		public string LinkType;

		public string LinkEndType;
	}
}
