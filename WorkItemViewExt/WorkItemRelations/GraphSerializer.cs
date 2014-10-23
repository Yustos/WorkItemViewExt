using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using YL.WorkItemViewExt.WorkItemRelations.Entities;

namespace YL.WorkItemViewExt.WorkItemRelations
{
	internal sealed class GraphSerializer
	{
		private const string dgmlNS = "http://schemas.microsoft.com/vs/2009/dgml";

		internal void GenerateDGML(string saveFilePath, GraphPart part, IEnumerable<string> nodeTypes, IEnumerable<string> linkTypes)
		{
			var rootNode = new XElement(XName.Get("DirectedGraph", dgmlNS),
					new XAttribute("GraphDirection", "LeftToRight"),
					new XElement(XName.Get("Nodes", dgmlNS),
						from item in part.Nodes
						select
							new XElement(XName.Get("Node", dgmlNS),
							new XAttribute("Id", item.Id),
							new XAttribute("Label", item.Title),
							new XAttribute("Project", item.Project),
							new XAttribute("Category", item.Category),
							new XAttribute("SourceLocation", item.SourceLocation))),
					new XElement(XName.Get("Categories", dgmlNS),
						nodeTypes.Select(t => new XElement(XName.Get("Category", dgmlNS), new XAttribute("Id", t))),
						linkTypes.Select(t => new XElement(XName.Get("Category", dgmlNS), new XAttribute("Id", t)))),
					GenerateStyles(nodeTypes, linkTypes));

			var linksNode = new XElement(XName.Get("Links", dgmlNS),
						from item in part.Links
						select new XElement(XName.Get("Link", dgmlNS), new XAttribute("Source", item.SourceId), new XAttribute("Target", item.TargetId),
							new XAttribute("Category", item.Category),
							new XAttribute("Label", item.LinkEndType)));
			rootNode.Add(linksNode);

			var document = new XDocument(rootNode);
			document.Save(saveFilePath);
		}

		private XElement GenerateStyles(IEnumerable<string> nodeTypes, IEnumerable<string> linkTypes)
		{
			return new XElement(XName.Get("Styles", dgmlNS),
				nodeTypes.Select(t => StyleElement(t, "Node", "Background", GenerateNodeColor(t))),
				linkTypes.Select(t => StyleElement(t, "Link", "Stroke", GenerateLinkColor(t))));
		}

		private static XElement StyleElement(string category, string targetType, string propertyName, string propertyValue)
		{
			return new XElement(XName.Get("Style", dgmlNS), new XAttribute("TargetType", targetType), new XAttribute("GroupLabel", category), new XAttribute("ValueLabel", "True"),
				new XElement(XName.Get("Condition", dgmlNS), new XAttribute("Expression", String.Format(CultureInfo.InvariantCulture, "HasCategory('{0}')", category))),
				new XElement(XName.Get("Setter", dgmlNS), new XAttribute("Property", propertyName), new XAttribute("Value", propertyValue)));
		}

		private static string GenerateNodeColor(string type)
		{
			switch (type)
			{
				case "Bug":
				case "Issue":
					return "Red";
				case "Task":
				case "Shared Steps":
					return "Yellow";
				case "User Story":
				case "Requirement":
				case "Feature":
					return "Green";
				case "Code Item":
				case "Code Review Request":
				case "Code Review Response":
					return "Gray";
				default:
					return string.Empty;
			}
		}

		private static string GenerateLinkColor(string type)
		{
			switch (type)
			{
				case "Microsoft.VSTS.Common.Affects":
					return "Red";
				case "System.LinkTypes.Dependency":
					return "Yellow";
				case "System.LinkTypes.Hierarchy":
					return "Green";
				case "System.LinkTypes.Related":
					return "Blue";
				default:
					return string.Empty;
			}
		}
	}
}
