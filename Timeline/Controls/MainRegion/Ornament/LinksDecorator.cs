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
					var host = (LinksDecorator)d;
					d.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => host.UpdateLinks()));
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

		public static readonly DependencyProperty ControllerProperty = ControlTimeLine.ControllerProperty.AddOwner(typeof(LinksDecorator));

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

		private void UpdateLinks()
		{
			var element = (UIElement)this;
			var adornerLayer = AdornerLayer.GetAdornerLayer(element);
			var existAdorners = adornerLayer.GetAdorners(element);
			if (existAdorners != null)
			{
				foreach (var adorner in existAdorners)
				{
					adornerLayer.Remove(adorner);
				}
			}

			var controlRecords = Helpers.FindVisualChildrens<ControlRecord>(this).ToDictionary(e => (Record)e.DataContext, e => e);

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
							Controller.Log(string.Format("Failed to get control for link {0} [{1}].", link.Owner.Id, link.Rev));
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
