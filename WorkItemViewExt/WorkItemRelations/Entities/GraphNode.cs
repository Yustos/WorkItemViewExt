using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YL.WorkItemViewExt.WorkItemRelations.Entities
{
	internal class GraphNode : IEquatable<GraphNode>
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Project { get; set; }

		public string Category { get; set; }

		public string SourceLocation { get; set; }

		public bool Equals(GraphNode other)
		{
			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var other = obj as GraphNode;
			return other != null && Equals(other);
		}
	}

}
