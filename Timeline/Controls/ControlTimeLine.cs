using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Controls.Fields.Ornament;
using YL.Timeline.Controls.MainRegion;
using YL.Timeline.Controls.MainRegion.Ornament;
using YL.Timeline.Controls.ThumbRegion;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls
{
	public class ControlTimeLine : UserControl
	{
		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register("Items", typeof(Item[]),
			typeof(ControlTimeLine),
			new UIPropertyMetadata(null,
				(d, doa) =>
				{
					var host = (ControlTimeLine)d;
					host.Controller.Input.Items = (Item[])doa.NewValue;
				}));

		public static readonly DependencyProperty ControllerProperty =
			DependencyProperty.RegisterAttached("Controller", typeof(ViewportController),
			typeof(ControlTimeLine),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		private readonly ControlThumb _thumb;
		private readonly ControlItems _host;
		private readonly LinksDecorator _linksDecorator;

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

		public static void SetController(UIElement element, ViewportController value)
		{
			element.SetValue(ControllerProperty, value);
		}
		public static ViewportController GetController(UIElement element)
		{
			return (ViewportController)element.GetValue(ControllerProperty);
		}

		public ControlTimeLine()
		{
			var dock = new DockPanel();
			dock.LastChildFill = true;

			var thumbBorder = new Border
			{
				Padding = new Thickness(1),
				Background = new LinearGradientBrush(Color.FromRgb(206, 225, 243), Color.FromRgb(231, 240, 250), 90)
			};

			_thumb = new ControlThumb();
			_thumb.Height = 20;

			thumbBorder.Child = _thumb;

			DockPanel.SetDock(thumbBorder, Dock.Top);
			dock.Children.Add(thumbBorder);

			var scroll = new ScrollViewer();
			scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

			_linksDecorator = new LinksDecorator();

			_host = new ControlItems();

			_linksDecorator.Child = _host;
			scroll.Content = _linksDecorator;

			scroll.ScrollChanged += ScrollScrollChanged;

			dock.Children.Add(scroll);

			Content = dock;
			Loaded += TimeLineLoaded;
		}

		private void ScrollScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			var decorator = Helpers.FindParent<SelectionDecorator>(this);
			decorator.Update();
		}

		private void TimeLineLoaded(object sender, RoutedEventArgs e)
		{
			var bindingHost = new Binding("Input.Items") { Source = Controller };
			_host.SetBinding(ControlItems.ItemsSourceProperty, bindingHost);

			var bindingLinks = new Binding("Input.Items") { Source = Controller };
			_linksDecorator.SetBinding(LinksDecorator.ItemsProperty, bindingLinks);
		}
	}
}
