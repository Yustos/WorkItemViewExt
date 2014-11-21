using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YL.Timeline.Controls.Behind;

namespace YL.Timeline.Controls.ThumbRegion
{
	public class ControlThumb : UserControl, ITimelinePart
	{
		private ViewportController _controller;

		public ControlThumb()
		{
			MouseLeftButtonUp += ThumbMouseLeftButtonUp;
			MouseWheel += ThumbMouseWheel;
			MouseMove += ThumbMouseMove;
			
			ToolTip = new ToolTip
			{
				StaysOpen = true,
				Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint
			};
		}

		public void SetController(ViewportController controller)
		{
			_controller = controller;
		}

		public void UpdateViewport()
		{
			InvalidateVisual();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.DrawRectangle(new LinearGradientBrush(Color.FromRgb(206, 225, 243), Color.FromRgb(231, 240, 250), 90),
				null,
				new Rect(0, 1, ActualWidth, ActualHeight - 2));

			if (_controller == null)
			{
				return;
			}

			drawingContext.DrawRoundedRectangle(null,
				new Pen(new SolidColorBrush(Color.FromArgb(175, 166, 185, 203)), 2),
				new Rect(ActualWidth * _controller.Start, 0, ActualWidth / _controller.Scale, ActualHeight),
				2, 2);

			var aggregation = _controller.AggregatePosisions(ActualWidth - 8);
			foreach (var kvp in aggregation.Aggregations)
			{
				var h = ActualHeight * kvp.Value / aggregation.Apex;
				drawingContext.DrawRectangle(Brushes.Green, null,
					new Rect(kvp.Key + 3, ActualHeight - h + 1, 2, h - 2));
			}
		}

		private double CalculateStart(double x)
		{
			var containerWidth = ActualWidth;
			var newStart = (x - (containerWidth / _controller.Scale / 2)) / containerWidth;
			if (newStart < 0)
			{
				newStart = 0;
			}
			else if (containerWidth * newStart + containerWidth / _controller.Scale > containerWidth)
			{
				newStart = (containerWidth - (containerWidth / _controller.Scale)) / containerWidth;
			}
			return newStart;
		}

		#region Events
		private void ThumbMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_controller.Start = CalculateStart(e.GetPosition(this).X);
		}

		private void ThumbMouseWheel(object sender, MouseWheelEventArgs e)
		{
			var newScale = _controller.Scale + e.Delta * 0.001;
			if (newScale < 1)
			{
				newScale = 1;
			}
			_controller.Scale = newScale;
			_controller.Start = CalculateStart(e.MouseDevice.GetPosition(this).X);
		}

		private void ThumbMouseMove(object sender, MouseEventArgs e)
		{
			var x = e.GetPosition(this).X;
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				_controller.Start = CalculateStart(x);
			}

			var dataPos = x / _controller.Scale / ActualWidth;
			var ticks = _controller.Input.Min.Ticks + (_controller.Input.Range.Ticks) * dataPos;
			var date = new DateTime((long)ticks);

			((ToolTip)ToolTip).Content = date;
		}

		#endregion
	}
}
