using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using YL.Timeline.Interfaces;

namespace YL.Timeline.Forms
{
	internal class DisplayFieldsModel : INotifyPropertyChanged
	{
		private ICollectionView _displayFields;
		private ICollectionView _selectedFields;
		private string _displayFieldsFilter;
		private string _selectedFieldsFilter;

		public ICollectionView DisplayFields
		{
			get
			{
				return _displayFields;
			}
			set
			{
				_displayFields = value;
				OnPropertyChanged();
			}
		}

		public string DisplayFieldsFilter
		{
			get
			{
				return _displayFieldsFilter;
			}
			set
			{
				_displayFieldsFilter = value;
				OnPropertyChanged();
				_displayFields.Refresh();
			}
		}

		public ICollectionView SelectedFields
		{
			get
			{
				return _selectedFields;
			}
			set
			{
				_selectedFields = value;
				OnPropertyChanged();
			}
		}

		public string SelectedFieldsFilter
		{
			get
			{
				return _selectedFieldsFilter;
			}
			set
			{
				_selectedFieldsFilter = value;
				OnPropertyChanged();
				_selectedFields.Refresh();
			}
		}

		internal DisplayFieldsModel(IDataService service, string[] displayFields)
		{
			_displayFields = CollectionViewSource.GetDefaultView(new HashSet<string>(service.GetFields()));
			_selectedFields = CollectionViewSource.GetDefaultView(displayFields == null ? new HashSet<string>() : new HashSet<string>(displayFields));

			_displayFields.SortDescriptions.Add(new SortDescription { Direction = ListSortDirection.Ascending });
			_selectedFields.SortDescriptions.Add(new SortDescription { Direction = ListSortDirection.Ascending });

			_displayFields.Filter = (o) => Filter(o, DisplayFieldsFilter);
			_selectedFields.Filter = (o) => Filter(o, SelectedFieldsFilter);
		}

		private bool Filter(object item, string filter)
		{
			if (string.IsNullOrEmpty(filter))
			{
				return true;
			}
			string s = item as string;
			if (string.IsNullOrEmpty(s))
			{
				return false;
			}
			return s.IndexOf(filter, StringComparison.OrdinalIgnoreCase) != -1;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		internal void SelectItem(string item)
		{
			var all = (ICollection<string>)_displayFields.SourceCollection;
			all.Remove(item);
			var sel = (ICollection<string>)_selectedFields.SourceCollection;
			sel.Add(item);
			_selectedFields.Refresh();
			_displayFields.Refresh();
		}

		internal void UnselectItem(string item)
		{
			var all = (ICollection<string>)_displayFields.SourceCollection;
			all.Add(item);
			var sel = (ICollection<string>)_selectedFields.SourceCollection;
			sel.Remove(item);
			_selectedFields.Refresh();
			_displayFields.Refresh();
		}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
