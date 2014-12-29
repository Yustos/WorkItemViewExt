using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
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

		public static readonly DependencyProperty ControllerProperty = ControlTimeLine.ControllerProperty.AddOwner(typeof(ControlItems));

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

		public ControlItems()
		{
			ToolTip = new ToolTip
			{
				StaysOpen = true,
				Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint
			};

			MouseUp += ControlItemsMouseUp;
			MouseMove += ControlItemsMouseMove;

			var factory = new FrameworkElementFactory(typeof(ControlItem));
			ItemTemplate = new DataTemplate { VisualTree = factory };
			Loaded += ControlItemsLoaded;
		}

		private void ControlItemsLoaded(object sender, RoutedEventArgs e)
		{
			SetBinding(ScaleProperty, new Binding("Scale") { Source = Controller });
			SetBinding(StartProperty, new Binding("Start") { Source = Controller });
			Controller.Selection.CollectionChanged += SelectionCollectionChanged;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			var childs = Helpers.FindVisualChildrens<ControlRecords>(this).ToArray();
			foreach (var child in childs)
			{
				child.InvalidateMeasure();
			}
			var measure = base.MeasureOverride(constraint);

			Controller.LastElementWidth = childs.Length > 0 ? childs.Max(c => c.LastElementWidth) : 0;

			return measure;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			var controller = Controller;
			
			var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
			var textMeasure = new FormattedText(DateTime.Now.ToShortDateString(),
					CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
					typeface, FontSize, Brushes.Gray);
			var dayStep = Math.Max(1, Controller.DayStep(textMeasure.Width, ActualWidth - 2));

			for (var start = controller.Input.Min.Date; start <= controller.Input.Max; start = start.AddDays(dayStep))
			{
				var pos = controller.Interpolate(start, ActualWidth - 2);
				drawingContext.DrawLine(new Pen(new SolidColorBrush(Color.FromArgb(90, 127, 127, 127)), 1),
					new Point(pos, 0),
					new Point(pos, ActualHeight));
			}
		}

		internal void ToggleSelectedRecord(ControlRecord recordControl)
		{
			if (recordControl == null)
			{
				return;
			}
			var controller = Controller;
			var record = (Record)recordControl.DataContext;
			if (controller.Selection.Contains(record))
			{
				controller.Selection.Remove(record);
			}
			else
			{
				controller.Selection.Add(record);
			}
		}

		private void SelectionCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			var items = (IList<Record>)sender;
			BeginUpdateSelectedItems();
			SelectedItems.Clear();

			if (items.Count > 0)
			{
				var childs = Helpers.FindVisualChildrens<ControlRecord>(this).ToArray();
				foreach (var child in childs)
				{
					if (items.Contains(child.DataContext))
					{
						SelectedItems.Add(child);
					}
				}
			}

			EndUpdateSelectedItems();
		}

		private void ControlItemsMouseUp(object sender, MouseButtonEventArgs e)
		{
			var recordControl = Helpers.FindParent<ControlRecord>((e.OriginalSource as Visual) ?? ((FrameworkContentElement)e.OriginalSource).Parent);
			ToggleSelectedRecord(recordControl);
		}

		private void ControlItemsMouseMove(object sender, MouseEventArgs e)
		{
			var controller = Controller;
			var x = e.GetPosition(this).X;
			var dataPos = x / (ActualWidth - 2 - controller.LastElementWidth) / controller.Scale + controller.Start;
			var ticks = controller.Input.Min.Ticks + (controller.Input.Range.Ticks) * dataPos;
			var date = new DateTime((long)ticks);
			((ToolTip)ToolTip).Content = date.ToString();
		}
	}
}
