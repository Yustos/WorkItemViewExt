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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YL.Timeline.Forms;

namespace YL.WorkItemViewExt.WorkItemTimeline.Controls
{
	public partial class ControlTimelinePane : UserControl
	{
		public ControlTimelinePane(TimelineModel model)
		{
			InitializeComponent();
			DataContext = model;
		}

		private void DisplayFieldsClick(object sender, RoutedEventArgs e)
		{
			var model = (TimelineModel)DataContext;
			var displayFields = DisplayFieldsForm.ShowAndGetResult(model.Service, model.DisplayFields, GetOwnerWindow());
			if (displayFields != null)
			{
				model.SetDisplayFields(displayFields);
			}
		}

		private Window GetOwnerWindow()
		{
			var parent = VisualTreeHelper.GetParent(this);
			while (!(parent is Window))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}
			return (Window)parent;
		}
	}
}
