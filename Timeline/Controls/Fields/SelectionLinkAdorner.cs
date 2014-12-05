using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.Fields
{
	public class SelectionLinkAdorner : Adorner
	{
		private readonly ControlRevisionView _target;

		private readonly Pen _pen = new Pen(new SolidColorBrush(Color.FromArgb(80, 0, 200, 0)), 2);

		public SelectionLinkAdorner(UIElement adornedElement, ControlRevisionView target)
			: base(adornedElement)
		{
			_target = target;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			InvalidateVisual();
			return base.ArrangeOverride(finalSize);
		}

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			var from = AdornedElement.IsVisible
				? TranslatePoint(new Point(DesiredSize.Width / 2, DesiredSize.Height), this)
				: ((FrameworkElement)((FrameworkElement)AdornedElement).Parent).TranslatePoint(new Point(0,0), this);
			_pen.DashStyle = AdornedElement.IsVisible ? DashStyles.Solid : DashStyles.Dot;

			var to = _target.TranslatePoint(new Point(_target.DesiredSize.Width / 2, 0), this);
			if (from.Y > to.Y)
			{
				return;
			}

			drawingContext.DrawLine(_pen, from, to);
		}
	}
}
