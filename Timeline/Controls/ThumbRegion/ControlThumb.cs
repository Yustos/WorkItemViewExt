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
					host.Controller.ViewportWidth = (double)doa.NewValue - 8.0;
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

		public static readonly DependencyProperty ControllerProperty = ControlTimeLine.ControllerProperty.AddOwner(typeof(ControlThumb));

		public ViewportController Controller
		{
			get
			{
				return (ViewportController)GetValue(ControllerProperty);
			}
			set
			{
				SetValue(ControllerProperty, value);
			}
		}

		public ControlThumb()
		{
			Background = Brushes.Transparent;
			_typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);

			MouseLeftButtonUp += ThumbMouseLeftButtonUp;
			MouseWheel += ThumbMouseWheel;
			MouseMove += ThumbMouseMove;

			ToolTip = new ToolTip
			{
				StaysOpen = true,
				Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint
			};
			
			Loaded += ControlThumb_Loaded;
		}

		private void ControlThumb_Loaded(object sender, RoutedEventArgs e)
		{
			SetBinding(ScaleProperty, new Binding("Scale") { Source = Controller });
			SetBinding(StartProperty, new Binding("Start") { Source = Controller });

			SetBinding(ViewportWidthProperty, new Binding("ActualWidth") { Mode = BindingMode.OneWay, Source = this });
			SetBinding(AggregatedPositionsProperty, new Binding("AggregatedPositions") { Mode = BindingMode.OneWay, Source = Controller });
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (ActualWidth < 0 || ActualHeight < 2)
			{
				return;
			}

			// Current viewport position
			drawingContext.DrawRoundedRectangle(null,
				new Pen(new SolidColorBrush(Color.FromArgb(175, 166, 185, 203)), 2),
				new Rect(ActualWidth * Controller.Start, 0, ActualWidth / Controller.Scale, ActualHeight),
				2, 2);

			// Activities
			var aggregation = Controller.AggregatedPositions;
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
			int day = -1;
			for (var start = Controller.Input.Min.Date; start <= Controller.Input.Max; start = start.AddDays(1))
			{
				var pos = Controller.Interpolate(start);
				drawingContext.DrawLine(new Pen(new SolidColorBrush(Color.FromArgb(127,127,127,127)), 1),
					new Point(pos, 0),
					new Point(pos, ActualHeight));

				if (text != null)
				{
					if (lastPos < 0)
					{
						lastPos = 0;
					}

					if (text.Width + 10 > pos - lastPos)
					{
						
						text = new FormattedText(day.ToString(),
							CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
							_typeface, FontSize, Brushes.Gray);
					}
					drawingContext.DrawText(text, new Point(lastPos + (pos - lastPos) / 2 - text.Width / 2, ActualHeight / 2 - text.Height / 2));
				}

				day = start.Day;
				text = new FormattedText(start.ToShortDateString(),
					CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
					_typeface, FontSize, Brushes.Gray);
				
				lastPos = pos;
			}

			if (text != null)
			{
				var pos = ActualWidth;
				if (text.Width + 10 > pos - lastPos)
				{
						
					text = new FormattedText(day.ToString(),
						CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
						_typeface, FontSize, Brushes.Gray);
				}
				drawingContext.DrawText(text, new Point(lastPos + (pos - lastPos) / 2 - text.Width / 2, ActualHeight / 2 - text.Height / 2));
			}
		}

		private double CalculateStart(double x)
		{
			var containerWidth = ActualWidth;
			var newStart = (x - (containerWidth / Controller.Scale / 2)) / containerWidth;
			if (newStart < 0)
			{
				newStart = 0;
			}
			else if (containerWidth * newStart + containerWidth / Controller.Scale > containerWidth)
			{
				newStart = (containerWidth - (containerWidth / Controller.Scale)) / containerWidth;
			}
			return newStart;
		}

		#region Events
		private void ThumbMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Controller.Start = CalculateStart(e.GetPosition(this).X);
		}

		private void ThumbMouseWheel(object sender, MouseWheelEventArgs e)
		{
			var newScale = Controller.Scale + e.Delta * 0.001;
			if (newScale < 1)
			{
				newScale = 1;
			}
			Controller.Scale = newScale;
			Controller.Start = CalculateStart(e.MouseDevice.GetPosition(this).X);
		}

		private void ThumbMouseMove(object sender, MouseEventArgs e)
		{
			var x = e.GetPosition(this).X;
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				Controller.Start = CalculateStart(x);
			}

			var dataPos = x / Controller.ViewportWidth;
			var ticks = Controller.Input.Min.Ticks + (Controller.Input.Range.Ticks) * dataPos;
			var date = new DateTime((long)ticks);

			((ToolTip)ToolTip).Content = date.ToString();
		}

		#endregion
	}
}
