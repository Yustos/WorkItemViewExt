using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Controls.Behind.Entities;
using YL.Timeline.Controls.Fields;
using YL.Timeline.Controls.MainRegion.Ornament;
using YL.Timeline.Controls.ThumbRegion;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.MainRegion
{
	public class ControlItems : MultiSelector
	{
		public ControlRecord[] SelectedRecords
		{
			get
			{
				return SelectedItems.OfType<ControlRecord>().ToArray();
			}
		}

		public static readonly DependencyProperty ScaleProperty =
			DependencyProperty.Register("Scale", typeof(double),
			typeof(ControlItems),
			new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty StartProperty =
			DependencyProperty.Register("Start", typeof(double),
			typeof(ControlItems),
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

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

		private readonly ViewportController _controller;

		public ControlItems(ViewportController controller)
		{
			_controller = controller;
			SetBinding(ScaleProperty, new Binding("Scale") { Source = controller });
			SetBinding(StartProperty, new Binding("Start") { Source = controller });

			MouseUp += ControlItemsMouseUp;

			var factory = new FrameworkElementFactory(typeof(ControlItem));
			ItemTemplate = new DataTemplate { VisualTree = factory };
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var childs = Helpers.FindVisualChildrens<ControlRecords>(this).ToArray();
			foreach (var child in childs)
			{
				child.InvalidateMeasure();
			}
			var measure = base.MeasureOverride(constraint);

			_controller.LastElementWidth = childs.Length > 0 ? childs.Max(c => c.LastElementWidth) : 0;

			return measure;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			for (var start = _controller.Input.Min.Date; start <= _controller.Input.Max; start = start.AddDays(1))
			{
				var pos = _controller.Interpolate(start, ActualWidth - 2, _controller.LastElementWidth);
				drawingContext.DrawLine(new Pen(new SolidColorBrush(Color.FromArgb(127, 127, 127, 127)), 1),
					new Point(pos, 0),
					new Point(pos, ActualHeight));
			}
		}

		internal void ToggleSelectedRecord(ControlRecord recordControl)
		{
			this.BeginUpdateSelectedItems();

			
			if (SelectedItems.Contains(recordControl))
			{
				SelectedItems.Remove(recordControl);
			}
			else
			{
				SelectedItems.Add(recordControl);
			}

			this.EndUpdateSelectedItems();
		}

		private void ControlItemsMouseUp(object sender, MouseButtonEventArgs e)
		{
			var recordControl = Helpers.FindParent<ControlRecord>((DependencyObject)e.OriginalSource);
			ToggleSelectedRecord(recordControl);
		}
	}
}
