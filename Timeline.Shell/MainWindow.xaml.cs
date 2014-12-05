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
using YL.Timeline.Entities;
using YL.Timeline.Interaction;

namespace YL.Timeline.Shell
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int _offset = 0;

		public MainWindow()
		{
			InitializeComponent();
			var items = GetData(_offset);
			DataContext = new DataModel(items, (id, rev) => {
				return new RevisionChanges
				{
					Fields = new[]
					{
						new Field { Name = "test", OriginalValue = "Orig", Value = "val"},
						new Field { Name = "testeq", OriginalValue = "eq", Value = "eq"}
					},
					Attachments = new []
					{
						new Attachment
						{
							IsAdded = true,
							Name = "testAttach",
							Uri = new Uri("http://localhost/attachment/test.txt")
						}
					},
					Changesets = new[]
					{
						new Changeset
						{
							Uri = new Uri("http://localhost/changeset/id"),
							IsAdded = true,
							Comment= "TestCS"
						}
					}
				};
			});
		}

		private Item[] GetData(int offset)
		{
			var item4 = new Item { Id = offset + 18, Title = "Test " + (offset + 18) };
			var linkTrg2 = new Record(item4) { Rev = 1, Date = DateTime.Now.AddDays(-3), State = "Active" };
			item4.Records = new[]
				{
					new Record(item4) {Rev = 0, Date = DateTime.Now.AddDays(-4), State = "Proposed" },
					linkTrg2,
					new Record(item4) { Rev = 2, Date = DateTime.Now.AddDays(-2), State = "Active", },
					new Record(item4) {Rev = 3, Date = DateTime.Now.AddDays(-2.98), State = "Active", },
					new Record(item4) {Rev = 4, Date = DateTime.Now.AddDays(-1), State = "Resolved", }
				};
			
			var result = new List<Item>();
			var item = new Item { Id = offset + 5, Title = "Test " + (offset + 5) };
			var linkSrc2 = new Record(item) { Rev = 0, Date = DateTime.Now.AddDays(-2), RemovedLinks =
				offset % 2 == 0 ? null : new[] { linkTrg2 }
			};
			item.Records = new []
				{
					linkSrc2,
					new Record(item) {Rev = 1, Date = DateTime.Now.AddDays(-2) },
					new Record(item) {Rev = 2, Date = DateTime.Now.AddDays(-1).Date }
				};

			var item3 = new Item { Id = offset + 17, Title = "Test " + (offset + 17) };
			var linkTrg = new Record(item3) { Rev = 2, Date = DateTime.Now.AddDays(-2) };
			item3.Records = new[]
				{
					new Record(item3) {Rev = 0, Date = DateTime.Now.AddDays(-4) },
					new Record(item3) {Rev = 1, Date = DateTime.Now.AddDays(-3) },
					linkTrg
				};

			var item2 = new Item { Id = offset + 7, Title = "Test " + (offset + 7) };
			var linkSrc = new Record(item2) { Rev = 1, Date = DateTime.Now.AddDays(-3), AddedLinks = new[] { linkTrg } };
			item2.Records = new []
				{
					new Record(item2) {Rev = 0, Date = DateTime.Now.AddDays(-5) },
					linkSrc,
					new Record(item2) {Rev = 2, Date = DateTime.Now.AddDays(-3),
						AddedAttachments = 2,
						RemovedChangesets = 4
					},
					new Record(item2) {Rev = 3, Date = DateTime.Now.AddDays(-2.98) }
				};

			var item5 = new Item { Id = offset + 19, Title = "Test " + (offset + 19) };
			item5.Records = new []
				{
					new Record(item5) { Rev = 0, Date = DateTime.Now.AddDays(-4), State = "Proposed" },
					new Record(item5) { Rev = 1, Date = DateTime.Now.AddDays(-2.98), State = "Active",
						AddedAttachments = 2,
						RemovedChangesets = 4
					},
					new Record(item5) { Rev = 2, Date = DateTime.Now.AddDays(-2), State = offset % 2 == 0 ? "Resolved" : "Active", },
					new Record(item5) { Rev = 3, Date = DateTime.Now.AddDays(-1), State = "Resolved", }
				};
			
			return new[] { item, item2, item3, item4, item5 };
		}

		private Record[] CreateRecords(Item item, IEnumerable<Tuple<int, DateTime, string>> records)
		{
			var result = new List<Record>();
			foreach (var r in records)
			{
				result.Add(new Record(item)
					{
						Rev = r.Item1,
						Date = r.Item2,
						State = r.Item3
					});
			}
			return result.ToArray();
		}

		private void MenuItem_MouseUp(object sender, System.Windows.RoutedEventArgs e)
		{
			var model = (DataModel)DataContext;
			model.Items = GetData(++_offset);
		}
	}
}
