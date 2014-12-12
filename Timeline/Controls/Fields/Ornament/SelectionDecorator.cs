using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace YL.Timeline.Controls.Fields.Ornament
{
	public class SelectionDecorator : Decorator
	{
		public static readonly DependencyProperty ControllerProperty = ControlTimeLine.ControllerProperty.AddOwner(typeof(SelectionDecorator));

		public static readonly DependencyProperty SelectedItemsProperty =
			DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<Record>),
			typeof(SelectionDecorator),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, doa) =>
				{
					var collection = (ObservableCollection<Record>)doa.NewValue;
					var host = (SelectionDecorator)d;
					collection.CollectionChanged += (s, o) => host.Update();
				}));

		public ObservableCollection<Record> SelectedItems
		{
			get
			{
				return (ObservableCollection<Record>)GetValue(SelectedItemsProperty);
			}
			set
			{
				SetValue(SelectedItemsProperty, value);
			}
		}

		public static readonly DependencyProperty ScaleProperty =
			DependencyProperty.Register("Scale", typeof(double),
			typeof(SelectionDecorator),
			new FrameworkPropertyMetadata(1.0,
				(d, doa) =>
				{
					var host = (SelectionDecorator)d;
					host.Update();
				}));

		public static readonly DependencyProperty StartProperty =
			DependencyProperty.Register("Start", typeof(double),
			typeof(SelectionDecorator),
			new FrameworkPropertyMetadata(0.0, 
				(d, doa) =>
				{
					var host = (SelectionDecorator)d;
					host.Update();
				}));

		public SelectionDecorator()
		{
			ClipToBounds = true;
			Loaded += SelectionDecoratorLoaded;
		}

		internal void Update()
		{
			var controller = (ViewportController)(GetValue(ControllerProperty));
			Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => 
				{
					try
					{
						UpdateAdorners();
					}
					catch (Exception ex)
					{
						controller.Log(ex.Message);
					}
				}));
		}

		private void SelectionDecoratorLoaded(object sender, RoutedEventArgs e)
		{
			var controller = (ViewportController)(GetValue(ControllerProperty));
			if (controller != null)
			{
				SetBinding(StartProperty, new Binding("Start") { Source = controller });
				SetBinding(ScaleProperty, new Binding("Scale") { Source = controller });
			}
		}

		private void UpdateAdorners()
		{
			var adornerLayer = AdornerLayer.GetAdornerLayer(this);

			var existAdorners = adornerLayer.GetAdorners(this);
			if (existAdorners != null)
			{
				foreach (var adorner in existAdorners)
				{
					adornerLayer.Remove(adorner);
				}
			}

			var records = Helpers.FindVisualChildrens<ControlRecord>(this).ToList();
			var details = Helpers.FindVisualChildrens<ControlRevisionView>(this).ToList();

			foreach (var link in details.Join(records, d => d.DataContext, d => d.DataContext, (d, r) => new { Detail = d, Record = r}))
			{
				var linkAdorner = new SelectionLinkAdorner(this, link.Record, link.Detail);
				adornerLayer.Add(linkAdorner);
			}
		}
	}
}
