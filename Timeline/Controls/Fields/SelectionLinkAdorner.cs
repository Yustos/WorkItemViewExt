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
			var from = this.TranslatePoint(new System.Windows.Point(this.DesiredSize.Width / 2, this.DesiredSize.Height), this);
			var to = _target.TranslatePoint(new System.Windows.Point(_target.DesiredSize.Width / 2, 0), this);
			if (from.Y > to.Y)
			{
				return;
			}

			drawingContext.DrawLine(new System.Windows.Media.Pen(new SolidColorBrush(Color.FromArgb(80, 0,200,0)), 2), from, to);
		}
	}
}
