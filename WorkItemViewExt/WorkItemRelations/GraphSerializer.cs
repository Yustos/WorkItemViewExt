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

		internal void GenerateDGML(string saveFilePath, GraphPart part, IEnumerable<string> types)
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
						types.Select(t => new XElement(XName.Get("Category", dgmlNS), new XAttribute("Id", t)))),
					GenerateStyles(types));

			var linksNode = new XElement(XName.Get("Links", dgmlNS),
						from item in part.Links
						select new XElement(XName.Get("Link", dgmlNS), new XAttribute("Source", item.SourceId), new XAttribute("Target", item.TargetId),
							new XAttribute("Category", item.Category)));
			rootNode.Add(linksNode);

			var document = new XDocument(rootNode);
			document.Save(saveFilePath);
		}

		private XElement GenerateStyles(IEnumerable<string> types)
		{
			return new XElement(XName.Get("Styles", dgmlNS),
				types.Select(t => StyleElement(t, "Node", "Background", GenerateColor(t))));
		}

		private static XElement StyleElement(string category, string targetType, string propertyName, string propertyValue)
		{
			return new XElement(XName.Get("Style", dgmlNS), new XAttribute("TargetType", targetType), new XAttribute("GroupLabel", category), new XAttribute("ValueLabel", "True"),
				new XElement(XName.Get("Condition", dgmlNS), new XAttribute("Expression", String.Format(CultureInfo.InvariantCulture, "HasCategory('{0}')", category))),
				new XElement(XName.Get("Setter", dgmlNS), new XAttribute("Property", propertyName), new XAttribute("Value", propertyValue)));
		}

		private static string GenerateColor(string type)
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
	}
}
