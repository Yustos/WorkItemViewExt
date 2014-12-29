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
using System.Windows.Shapes;
using YL.Timeline.Interfaces;

namespace YL.Timeline.Forms
{
	/// <summary>
	/// Interaction logic for DisplayFieldsForm.xaml
	/// </summary>
	public partial class DisplayFieldsForm : Window
	{
		private DisplayFieldsModel Model
		{
			get
			{
				return (DisplayFieldsModel)DataContext;
			}
		}

		public DisplayFieldsForm()
		{
			InitializeComponent();
		}

		public static string[] ShowAndGetResult(IDataService service, string[] displayFields, Window owner)
		{
			var model = new DisplayFieldsModel(service, displayFields);
			var form = new DisplayFieldsForm();
			form.Owner = owner;
			form.DataContext = model;
			if (form.ShowDialog().Equals(true))
			{
				return model.SelectedFields.Cast<string>().ToArray();
			}
			return null;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
		private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var item = DisplayFieldsListBox.SelectedValue as string;
			var model = (DisplayFieldsModel)DataContext;
			model.SelectItem(item);
		}

		private void SelectedFieldsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var item = SelectedFieldsListBox.SelectedValue as string;
			var model = (DisplayFieldsModel)DataContext;
			model.UnselectItem(item);
		}
	}
}
