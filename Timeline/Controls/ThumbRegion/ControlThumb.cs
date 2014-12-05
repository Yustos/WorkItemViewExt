using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Controls.Behind.Entities;

namespace YL.Timeline.Controls.ThumbRegion
{
	public class ControlThumb : UserControl
	{
		private readonly ViewportController _controller;

		private readonly Typeface _typeface;

		public static readonly DependencyProperty ScaleProperty =
			DependencyProperty.Register("Scale", typeof(double),
			typeof(ControlThumb),
			new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty StartProperty =
			DependencyProperty.Register("Start", typeof(double),
			typeof(ControlThumb),
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty ViewportWidthProperty =
			DependencyProperty.Register("ViewportWidth", typeof(double),
			typeof(ControlThumb),
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, (d, doa) =>
				{
					var host = (ControlThumb)d;
					host._controller.ViewportWidth = (double)doa.NewValue - 8.0;
				}));

		public static readonly DependencyProperty AggregatedPositionsProperty =
			DependencyProperty.Register("AggregatedPositions", typeof(AggregatedPositions),
			typeof(ControlThumb),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public double Scale
		{
			get
			{
				return (double)GetValue(ScaleProperty);
			}
			set
			{
				SetValue(ScaleProperty, value);
			}
		}

		public double Start
		{
			get
			{
				return (double)GetValue(StartProperty);
			}
			set
			{
				SetValue(StartProperty, value);
			}
		}

		public double ViewportWidth
		{
			get
			{
				return (double)GetValue(ViewportWidthProperty);
			}
			set
			{
				SetValue(ViewportWidthProperty, value);
			}
		}

		public AggregatedPositions AggregatedPositions
		{
			get
			{
				return (AggregatedPositions)GetValue(AggregatedPositionsProperty);
			}
			set
			{
				SetValue(AggregatedPositionsProperty, value);
			}
		}

		public ControlThumb(ViewportController controller)
		{
			Background = Brushes.Transparent;
			_controller = controller;
			_typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
			SetBinding(ScaleProperty, new Binding("Scale") { Source = controller });
			SetBinding(StartProperty, new Binding("Start") { Source = controller });

			MouseLeftButtonUp += ThumbMouseLeftButtonUp;
			MouseWheel += ThumbMouseWheel;
			MouseMove += ThumbMouseMove;

			ToolTip = new ToolTip
			{
				StaysOpen = true,
				Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint
			};

			SetBinding(ViewportWidthProperty, new Binding("ActualWidth") { Mode = BindingMode.OneWay, Source = this });
			SetBinding(AggregatedPositionsProperty, new Binding("AggregatedPositions") { Mode = BindingMode.OneWay, Source = _controller });
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (ActualWidth < 0 || ActualHeight < 2)
			{
				return;
			}

			//drawingContext.DrawRectangle(new LinearGradientBrush(Color.FromRgb(206, 225, 243), Color.FromRgb(231, 240, 250), 90),
			//	null,
			//	new Rect(0, 1, ActualWidth, ActualHeight - 2));

			// Current viewport position
			drawingContext.DrawRoundedRectangle(null,
				new Pen(new SolidColorBrush(Color.FromArgb(175, 166, 185, 203)), 2),
				new Rect(ActualWidth * _controller.Start, 0, ActualWidth / _controller.Scale, ActualHeight),
				2, 2);

			// Activities
			var aggregation = _controller.AggregatedPositions;
			if (aggregation != null)
			{
				foreach (var kvp in aggregation.Aggregations)
				{
					var h = ActualHeight * kvp.Value / aggregation.Apex;
					drawingContext.DrawLine(new Pen(Brushes.Green, 2),
						new Point(kvp.Key, ActualHeight - h + 1),
						new Point(kvp.Key, ActualHeight));
				}
			}

			// Date markers
			double lastPos = 0;
			FormattedText text = null;
			for (var start = _controller.Input.Min.Date; start <= _controller.Input.Max; start = start.AddDays(1))
			{
				var pos = _controller.Interpolate(start);
				drawingContext.DrawLine(new Pen(new SolidColorBrush(Color.FromArgb(127,127,127,127)), 1),
					new Point(pos, 0),
					new Point(pos, ActualHeight));

				if (text != null)
				{
					if (text.Width + 10 > pos - lastPos)
					{
						text = new FormattedText(start.Day.ToString(),
							CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight,
							_typeface, FontSize, Brushes.Gray);
					}
					drawingContext.DrawText(text, new Point(lastPos + (pos - lastPos) / 2 - text.Width / 2, ActualHeight / 2 - text.Height / 2));
				}

				text = new FormattedText(start.ToShortDateString(),
					CultureInfo.CurrentUICulture, System.Windows.FlowDirection.LeftToRight,
					_typeface, FontSize, Brushes.Gray);
				
				lastPos = pos;
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

			var dataPos = x / _controller.ViewportWidth;
			var ticks = _controller.Input.Min.Ticks + (_controller.Input.Range.Ticks) * dataPos;
			var date = new DateTime((long)ticks);

			((ToolTip)ToolTip).Content = date.ToString();

			var pos = _controller.Interpolate(date);

		}

		#endregion
	}
}
