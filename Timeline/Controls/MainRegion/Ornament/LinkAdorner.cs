using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace YL.Timeline.Controls.MainRegion.Ornament
{
	internal class LinkAdorner : Adorner
	{
		private readonly ControlRecord _from;
		private readonly ControlRecord _to;
		private readonly bool _added;

		private static readonly Pen _penAdded = new Pen(new SolidColorBrush(Color.FromArgb(160, 0, 0, 170)), 2);
		private static readonly Pen _penRemoved = new Pen(new SolidColorBrush(Color.FromArgb(160, 170, 0, 0)), 2) { DashStyle = DashStyles.Dot };

		internal LinkAdorner(UIElement adornedElement, ControlRecord from, ControlRecord to, bool added)
			: base(adornedElement)
		{
			IsClipEnabled = true;
			_from = from;
			_to = to;
			_added = added;
		}
		protected override void OnRender(DrawingContext drawingContext)
		{
			var posFrom = _from.TranslatePoint(new Point(_from.DesiredSize.Width, _from.DesiredSize.Height / 2), AdornedElement);
			var posTo = _to.TranslatePoint(new Point(_to.DesiredSize.Width, _to.DesiredSize.Height / 2), AdornedElement);

			var midFrom = new Point(posFrom.X + 8, posFrom.Y);
			var midTo = new Point(posFrom.X + 8, posTo.Y);

			var g = new PathGeometry();
			var path = new PathFigure();

			path.StartPoint = posFrom;
			path.Segments.Add(new LineSegment(midFrom, true));
			path.Segments.Add(new LineSegment(midTo, true));
			path.Segments.Add(new LineSegment(posTo, true));
			// Triangle
			if (_added)
			{
				path.Segments.Add(new LineSegment(new Point(posTo.X + 2, posTo.Y + 2), true));
				path.Segments.Add(new LineSegment(new Point(posTo.X + 2, posTo.Y - 2), true));
				path.Segments.Add(new LineSegment(posTo, true));
			}

			g.Figures.Add(path);

			drawingContext.DrawGeometry(null, _added ? _penAdded : _penRemoved, g);
		}
	}
}
