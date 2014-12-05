using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.MainRegion.Ornament
{
	public class StateAdorner : Adorner
	{
		private static readonly LinearGradientBrush _invalidBrush = new LinearGradientBrush
		{
			StartPoint = new Point(2, 2),
			EndPoint = new Point(0, 0),
			MappingMode = BrushMappingMode.Absolute,
			SpreadMethod = GradientSpreadMethod.Repeat,
			GradientStops = new GradientStopCollection
			{
				new GradientStop(Colors.Red, 0),
				new GradientStop(Colors.Red, 0.1),
				new GradientStop(Colors.White, 0.1),
				new GradientStop(Colors.White, 1),
			}
		};

		private readonly ControlRecord _from;
		private readonly ControlRecord _to;

		public StateAdorner(UIElement adornedElement, ControlRecord from, ControlRecord to)
			: base(adornedElement)
		{
			IsClipEnabled = true;
			IsHitTestVisible = false;
			_from = from;
			_to = to;
		}

		protected override void OnRender(System.Windows.Media.DrawingContext dc)
		{
			var posFrom = _from.TranslatePoint(new Point(0, 0), AdornedElement);
			var posTo = _to.TranslatePoint(new Point(0, 0), AdornedElement);
			if (posFrom.Y >= posTo.Y)
			{
				posFrom.X += _from.DesiredSize.Width;
			}
			var mid = posTo.Y + _to.DesiredSize.Height / 2;
			posFrom.Y = mid - 10;
			posTo.Y = mid + 10;

			var record = (Record)_from.DataContext;
			
			dc.DrawRectangle(record.State == null ? (Brush)_invalidBrush : new SolidColorBrush(ColorGenerator.GetColor(record.State)),
				null, new Rect(posFrom, posTo));
		}
	}
}
