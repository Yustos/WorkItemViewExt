using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Controls.MainRegion;
using YL.Timeline.Controls.ThumbRegion;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls
{
	public class TimeLine : UserControl
	{
		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register("Items", typeof(Item[]),
			typeof(TimeLine),
			new UIPropertyMetadata(null,
				(d, doa) =>
				{
					var host = (TimeLine)d;
					host.UpdateItems((Item[])doa.NewValue);
				}));

		public static readonly DependencyProperty SelectedRecordsProperty =
			DependencyProperty.Register("SelectedRecords", typeof(ControlRecord[]),
			typeof(TimeLine),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		private readonly ControlThumb _thumb;
		private readonly ControlItems _host;

		public Item[] Items
		{
			get
			{
				return (Item[])GetValue(ItemsProperty);
			}
			set
			{
				SetValue(ItemsProperty, value);
			}
		}

		public ControlRecord[] SelectedRecords
		{
			get
			{
				return (ControlRecord[])GetValue(SelectedRecordsProperty);
			}
			set
			{
				SetValue(SelectedRecordsProperty, value);
			}
		}

		public TimeLine()
		{
			var dock = new DockPanel();
			dock.LastChildFill = true;

			_thumb = new ControlThumb();
			_thumb.Height = 20;
			DockPanel.SetDock(_thumb, Dock.Top);
			dock.Children.Add(_thumb);

			var scroll = new ScrollViewer();
			scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

			_host = new ControlItems();
			_host.SelectionChanged += hostSelectionChanged;
			scroll.Content = _host;
			
			dock.Children.Add(scroll);

			Content = dock;
		}

		private void UpdateItems(Item[] items)
		{
			var input = new TimelineInput(items);
			var controller = new ViewportController(input);
			controller.Relayouting += ControllerRelayouting;

			_thumb.SetController(controller);
			_host.SetController(controller);
			controller.Relayout();
		}

		private void ControllerRelayouting(object sender, EventArgs e)
		{
			_thumb.UpdateViewport();
			_host.UpdateViewport();
		}

		private void hostSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedRecords = _host.SelectedRecords.ToArray();
		}
	}
}
