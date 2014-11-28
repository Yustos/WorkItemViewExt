using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Interaction;

namespace YL.Timeline.Controls.Fields
{
	public class ControlRevisionsView : ListBox
	{
		private readonly Dictionary<ControlRecord, ControlRevisionView> _controlsCache = new Dictionary<ControlRecord, ControlRevisionView>();

		public ControlRevisionsView()
		{
			VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
			var factory = new FrameworkElementFactory(typeof(WrapPanel));
			factory.SetValue(WrapPanel.IsItemsHostProperty, true);
			ItemsPanel = new ItemsPanelTemplate(factory);

			//var itemFactory = new FrameworkElementFactory(typeof(ControlRevisionView));
			//ItemTemplate = new DataTemplate() { VisualTree = itemFactory };
		}

		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
			var hash = new HashSet<ControlRecord>(Items.OfType<ControlRecord>());
			foreach (var toRemove in _controlsCache.Where(kvp => !hash.Contains(kvp.Key)).ToList())
			{
				_controlsCache.Remove(toRemove.Key);
			}
		}

		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			var control = item as ControlRecord;
			if (control == null)
			{
				base.PrepareContainerForItemOverride(element, item);
				return;
			}
			
			ControlRevisionView instance;
			if (!_controlsCache.TryGetValue(control, out instance))
			{
				instance = new ControlRevisionView();
				_controlsCache.Add(control, instance);
			}
			base.PrepareContainerForItemOverride(element, instance);
		}
	}
}
