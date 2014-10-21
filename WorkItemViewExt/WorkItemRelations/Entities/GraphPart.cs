using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt.WorkItemRelations.Entities
{
	internal class GraphPart
	{
		internal GraphPart() : this(null, null)
		{
		}

		internal GraphPart(IEnumerable<GraphNode> nodes, IEnumerable<GraphLink> links)
		{
			Nodes = new List<GraphNode>(nodes ?? Enumerable.Empty<GraphNode>());
			Links = new List<GraphLink>(links ?? Enumerable.Empty<GraphLink>());
		}

		internal List<GraphNode> Nodes { get; private set; }

		internal List<GraphLink> Links { get; private set; }
	}
}
