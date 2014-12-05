using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Controls.MainRegion;
using YL.Timeline.Controls.MainRegion.Ornament;
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
					host._input.Items = (Item[])doa.NewValue;
				}));

		public static readonly DependencyProperty SelectedRecordsProperty =
			DependencyProperty.Register("SelectedRecords", typeof(ControlRecord[]),
			typeof(TimeLine),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public static readonly DependencyProperty ControllerProperty =
			DependencyProperty.RegisterAttached("Controller", typeof(ViewportController),
			typeof(TimeLine),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		private readonly ControlThumb _thumb;
		private readonly ControlItems _host;

		private readonly TimelineInput _input = new TimelineInput();
		private readonly ViewportController _controller;

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

		public static void SetController(UIElement element, ViewportController value)
		{
			element.SetValue(ControllerProperty, value);
		}
		public static ViewportController GetController(UIElement element)
		{
			return (ViewportController)element.GetValue(ControllerProperty);
		}

		public TimeLine()
		{
			_controller = new ViewportController(_input);
			SetController(this, _controller);
			
			var dock = new DockPanel();
			dock.LastChildFill = true;

			var thumbBorder = new Border
			{
				Padding = new Thickness(1),
				Background = new LinearGradientBrush(Color.FromRgb(206, 225, 243), Color.FromRgb(231, 240, 250), 90)
			};

			_thumb = new ControlThumb(_controller);
			_thumb.Height = 20;

			thumbBorder.Child = _thumb;

			DockPanel.SetDock(thumbBorder, Dock.Top);
			dock.Children.Add(thumbBorder);

			var scroll = new ScrollViewer();
			scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

			var linksDecorator = new LinksDecorator();

			_host = new ControlItems(_controller);

			var bindingHost = new Binding("Input.Items") { Source = _controller };
			_host.SetBinding(ControlItems.ItemsSourceProperty, bindingHost);

			var bindingLinks = new Binding("Input.Items") { Source = _controller };
			linksDecorator.SetBinding(LinksDecorator.ItemsProperty, bindingLinks);

			_host.SelectionChanged += hostSelectionChanged;

			linksDecorator.Child = _host;
			scroll.Content = linksDecorator;
			
			dock.Children.Add(scroll);

			Content = dock;
		}

		private void hostSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			SelectedRecords = _host.SelectedRecords.ToArray();
		}
	}
}
