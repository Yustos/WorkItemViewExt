using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace YL.Timeline.Controls.MainRegion.Ornament
{
	public class RecordTransitionAdorner : Adorner
	{
		private readonly ControlRecord _from;
		private readonly ControlRecord _to;

		private static readonly Pen _pen = new Pen(new SolidColorBrush(Color.FromArgb(160, 0, 170, 0)), 2);

		public RecordTransitionAdorner(UIElement adornedElement, ControlRecord from, ControlRecord to)
			: base(adornedElement)
		{
			IsClipEnabled = true;
			IsHitTestVisible = false;
			_from = from;
			_to = to;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var posFrom = _from.TranslatePoint(new Point(0, 0), AdornedElement);
			var posTo = _to.TranslatePoint(new Point(0, 0), AdornedElement);

			var topFrom = posFrom.Y;
			var topTo = posTo.Y;

			var leftFrom = posFrom.X;
			var leftTo = posTo.X;

			var g = new PathGeometry();
			var path = new PathFigure();

			PathSegment[] segments;

			if (topFrom == topTo)
			{
				path.StartPoint = new Point(leftFrom + _from.DesiredSize.Width, topFrom + _from.DesiredSize.Height / 2 - 4);
				segments =
					new PathSegment[] 
				{
					new LineSegment(new Point(leftTo, topTo + _to.DesiredSize.Height / 2 - 4), true),
					// Triangle
					new LineSegment(new Point(leftTo - 2, topTo + _to.DesiredSize.Height / 2 - 6), true),
					new LineSegment(new Point(leftTo - 2, topTo + _to.DesiredSize.Height / 2 - 2), true),
					new LineSegment(new Point(leftTo, topTo + _to.DesiredSize.Height / 2 - 4), true),
				};
			}
			else
			{
				if (leftTo > leftFrom + _from.DesiredSize.Width + 8)
				{
					leftFrom = leftFrom + _from.DesiredSize.Width;
					path.StartPoint = new Point(leftFrom, topFrom + _from.DesiredSize.Height / 2 + 4);
					segments =
						new PathSegment[] 
					{
						new LineSegment(new Point(leftFrom + 7, topFrom + _from.DesiredSize.Height / 2 + 4), true),

						new LineSegment(new Point(leftFrom + 7, topTo + _to.DesiredSize.Height / 2 - 4), true),
						//new LineSegment(new Point(leftFrom - 7, topFrom + _from.DesiredSize.Height), true),
						//new LineSegment(new Point(leftTo - 7, topFrom + _from.DesiredSize.Height), true),
						new LineSegment(new Point(leftTo - 7, topTo + _to.DesiredSize.Height / 2 - 4), true),
						new LineSegment(new Point(leftTo, topTo + _to.DesiredSize.Height / 2 - 4), true),
						// Triangle
						new LineSegment(new Point(leftTo - 2, topTo + _to.DesiredSize.Height / 2 - 6), true),
						new LineSegment(new Point(leftTo - 2, topTo + _to.DesiredSize.Height / 2 - 2), true),
						new LineSegment(new Point(leftTo, topTo + _to.DesiredSize.Height / 2 - 4), true),
					};
				}
				else
				{
					path.StartPoint = new Point(leftFrom, topFrom + _from.DesiredSize.Height / 2 + 4);
					segments =
						new PathSegment[] 
					{
						new LineSegment(new Point(leftFrom - 7, topFrom + _from.DesiredSize.Height / 2 + 4), true),

						new LineSegment(new Point(leftFrom - 7, topTo + _to.DesiredSize.Height / 2 - 4), true),
						//new LineSegment(new Point(leftFrom - 7, topFrom + _from.DesiredSize.Height), true),
						//new LineSegment(new Point(leftTo - 7, topFrom + _from.DesiredSize.Height), true),
						new LineSegment(new Point(leftTo - 7, topTo + _to.DesiredSize.Height / 2 - 4), true),
						new LineSegment(new Point(leftTo, topTo + _to.DesiredSize.Height / 2 - 4), true),
						// Triangle
						new LineSegment(new Point(leftTo - 2, topTo + _to.DesiredSize.Height / 2 - 6), true),
						new LineSegment(new Point(leftTo - 2, topTo + _to.DesiredSize.Height / 2 - 2), true),
						new LineSegment(new Point(leftTo, topTo + _to.DesiredSize.Height / 2 - 4), true),
					};
				}
			}

			path.Segments.Clear();
			foreach (var segment in segments)
			{
				path.Segments.Add(segment);
			}
			
			g.Figures.Add(path);

			drawingContext.DrawGeometry(null, _pen, g);
		}
	}
}
