using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.MainRegion.Ornament
{
	public class LinksDecorator : Decorator
	{
		public static readonly DependencyProperty ItemsProperty =
			DependencyProperty.Register("Items", typeof(Item[]),
			typeof(LinksDecorator),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, doa) =>
				{
					d.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => UpdateLinks(d)));
				}));

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

		private static void UpdateLinks(DependencyObject d)
		{
			var element = (UIElement)d;
			var adornerLayer = AdornerLayer.GetAdornerLayer(element);
			var existAdorners = adornerLayer.GetAdorners(element);
			if (existAdorners != null)
			{
				foreach (var adorner in existAdorners)
				{
					adornerLayer.Remove(adorner);
				}
			}

			var controlRecords = Helpers.FindVisualChildrens<ControlRecord>(d).ToDictionary(e => (Record)e.DataContext, e => e);

			Action<IEnumerable<Record>, ControlRecord, bool> addControls = (sourceRecords, sourceControl, type) =>
			{
				if (sourceRecords != null)
				{
					foreach (var link in sourceRecords)
					{
						ControlRecord targetControl;
						if (controlRecords.TryGetValue(link, out targetControl))
						{
							adornerLayer.Add(new LinkAdorner(element, sourceControl, targetControl, type));
						}
						else
						{
#warning Log?
						}
					}
				}
			};

			foreach (var control in controlRecords)
			{
				addControls(control.Key.AddedLinks, control.Value, true);
				addControls(control.Key.RemovedLinks, control.Value, false);
			}
		}
	}
}
