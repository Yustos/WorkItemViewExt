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
using YL.Timeline.Controls.ThumbRegion;
using YL.Timeline.Entities;

namespace YL.Timeline.Controls.MainRegion
{
	public class ControlItems : MultiSelector, ITimelinePart
	{
		private readonly List<ControlRecords> _records = new List<ControlRecords>();

		private AdornerLayer _adornerLayer;

		private ViewportController _controller;

		public ControlRecord[] SelectedRecords
		{
			get
			{
				return SelectedItems.OfType<ControlRecord>().ToArray();
				/*return SelectedItems.OfType<FrameworkElement>().
					Select(r => r.DataContext as Record).
					Where(r => r != null).ToArray();*/
			}
		}

		public ControlItems()
		{
			
			this.MouseUp += ControlItemsMouseUp;
			this.Margin = new Thickness(10, 0, 10, 0);
		}

		public void SetController(ViewportController controller)
		{
			_adornerLayer = AdornerLayer.GetAdornerLayer(this);
			var controls = new Dictionary<Record, ControlRecord>();

			_controller = controller;
			foreach (var item in controller.Input.Items)
			{
				var title = new Title(item.Id, item.Title);
				Items.Add(title);

				var c = new ControlRecords(controller, item.Records);
				foreach (var r in c.RecordControls)
				{
					controls.Add(r.Key, r.Value);
				}
				_records.Add(c);
				Items.Add(c);
			}

			// Prepare links over items
			Action<IEnumerable<Record>, ControlRecord, bool> addControls = (sourceRecords, sourceControl, type) =>
				{
					if (sourceRecords != null)
					{
						foreach (var link in sourceRecords)
						{
							ControlRecord targetControl;
							if (controls.TryGetValue(link, out targetControl))
							{
								_adornerLayer.Add(new LinkRender(this, sourceControl, targetControl, type));
							}
							else
							{
#warning Log?
							}
						}
					}
				};

			foreach (var control in controls)
			{
				addControls(control.Key.AddedLinks, control.Value, true);
				addControls(control.Key.RemovedLinks, control.Value, false);
			}

			// Prepare revision paths
			ControlRecord last = null;
			foreach (var control in controls)
			{
				if (last != null)
				{
					_adornerLayer.Add(new ControlPath((UIElement)last.Parent, last, control.Value));
				}

				last = control.Value;
			}
		}

		public void UpdateViewport()
		{
			foreach (var control in _records)
			{
				control.UpdateViewport();
			}
			//InvalidateMeasure();
			//InvalidateVisual();
			_adornerLayer.Update();
		}

		private void ControlItemsMouseUp(object sender, MouseButtonEventArgs e)
		{
			this.BeginUpdateSelectedItems();

			var record = ((FrameworkElement)e.OriginalSource).Parent;
			if (SelectedItems.Contains(record))
			{
				SelectedItems.Remove(record);
			}
			else
			{
				SelectedItems.Add(record);
			}

			this.EndUpdateSelectedItems();
		}
	}
}
