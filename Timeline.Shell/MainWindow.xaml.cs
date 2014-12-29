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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YL.Timeline.Controls.Behind;
using YL.Timeline.Entities;
using YL.Timeline.Entities.RecordDetails;
using YL.Timeline.Forms;

namespace YL.Timeline.Shell
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int _offset = 0;

		private readonly Record.LoadDetailsDelegate _loader;

		public MainWindow()
		{
			InitializeComponent();
			_loader = new Record.LoadDetailsDelegate((r) =>
			{
				Thread.Sleep(2000);
				return new RevisionChanges
				{
					ChangedBy = "Test User",
					AuthorizedAs = "Test Auth",
					Fields = new[]
					{
						new Field { Name = "Id", ReferenceName= "System.Id", OriginalValue = r.Owner.Id, Value = r.Owner.Id},
						new Field { Name = "Rev", ReferenceName= "System.Rev",OriginalValue = r.Rev, Value = r.Rev},
						new Field { Name = "test", ReferenceName= "System.test", OriginalValue = "Orig", Value = "val"},
						new Field { Name = "testusr", ReferenceName= "System.testusr", OriginalValue = "Orig", Value = "val", IsChangedByUser = true},
						new Field { Name = "testeq", ReferenceName= "System.testeq", OriginalValue = "eq", Value = "eq"}
					},
					Attachments = new[]
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
							Id = 5,
							Uri = new Uri("http://localhost/changeset/id"),
							IsAdded = true,
							Comment= "TestCS"
						}
					}
				};
			});

			var controller = (ViewportController)Resources["controller"];
			controller.ShowChangeset = (id) =>
			{
				MessageBox.Show(this, id.ToString());
			};
			var items = GetData(_offset, new [] {"State"});
			DataContext = new DataModel(items);
		}

		private Item[] GetData(int offset, string[] displayFields)
		{
			var fillDisplayFields = new Action<Record>((r) => { 
				if (displayFields != null)
				{
					var fields = new List<Field>();
					foreach (var f in displayFields)
					{
						fields.Add(new Field
							{
								Name = string.Format("{0} {1} [{2}]", f, r.Owner.Id, r.Rev),
								ReferenceName = string.Format("Ref {0} {1} [{2}]", f, r.Owner.Id, r.Rev),
								OriginalValue = string.Format("Orig {0}", f),
								Value = string.Format("Val {0}", f)
							});
					}
					r.DisplayFields = fields.ToArray();
				}
			});

			var item4 = new Item { Id = offset + 18, Title = "Test " + (offset + 18) };
			var linkTrg2 = new Record(item4, _loader) { Rev = 1, Date = DateTime.Now.AddDays(-3), State = "Active" };
			item4.Records = new[]
				{
					new Record(item4, _loader) {Rev = 0, Date = DateTime.Now.AddDays(-4), State = "Proposed" },
					linkTrg2,
					new Record(item4, _loader) { Rev = 2, Date = DateTime.Now.AddDays(-2), State = "Active", },
					new Record(item4, _loader) {Rev = 3, Date = DateTime.Now.AddDays(-2.98), State = "Active", },
					new Record(item4, _loader) {Rev = 4, Date = DateTime.Now.AddDays(-1), State = "Resolved", }
				};
			
			var item = new Item { Id = offset + 5, Title = "Test " + (offset + 5) };
			var linkSrc2 = new Record(item, _loader)
			{
				Rev = 0,
				Date = DateTime.Now.AddDays(-2),
				RemovedLinks =
				offset % 2 == 0 ? null : new[] { linkTrg2 }
			};
			item.Records = new []
				{
					linkSrc2,
					new Record(item, _loader) {Rev = 1, Date = DateTime.Now.AddDays(-2) },
					new Record(item, _loader) {Rev = 2, Date = DateTime.Now.AddDays(-1).Date }
				};

			var item3 = new Item { Id = offset + 17, Title = "Test " + (offset + 17) };
			var linkTrg = new Record(item3, _loader) { Rev = 2, Date = DateTime.Now.AddDays(-2) };
			item3.Records = new[]
				{
					new Record(item3, _loader) {Rev = 0, Date = DateTime.Now.AddDays(-4) },
					new Record(item3, _loader) {Rev = 1, Date = DateTime.Now.AddDays(-3) },
					linkTrg
				};

			var item2 = new Item { Id = offset + 7, Title = "Test " + (offset + 7) };
			var linkSrc = new Record(item2, _loader) { Rev = 1, Date = DateTime.Now.AddDays(-3), AddedLinks = new[] { linkTrg } };
			item2.Records = new []
				{
					new Record(item2, _loader) {Rev = 0, Date = DateTime.Now.AddDays(-5) },
					linkSrc,
					new Record(item2, _loader) {Rev = 2, Date = DateTime.Now.AddDays(-3),
						AddedAttachments = 2,
						RemovedChangesets = 4
					},
					new Record(item2, _loader) {Rev = 3, Date = DateTime.Now.AddDays(-2.98) }
				};

			var item5 = new Item { Id = offset + 19, Title = "Test " + (offset + 19) };
			item5.Records = new []
				{
					new Record(item5, _loader) { Rev = 0, Date = DateTime.Now.AddDays(-4), State = "Proposed" },
					new Record(item5, _loader) { Rev = 1, Date = DateTime.Now.AddDays(-2.98), State = "Active",
						AddedAttachments = 2,
						RemovedChangesets = 4
					},
					new Record(item5, _loader) { Rev = 2, Date = DateTime.Now.AddDays(-2), State = offset % 2 == 0 ? "Resolved" : "Active", },
					new Record(item5, _loader) { Rev = 3, Date = DateTime.Now.AddDays(-1), State = "Resolved", }
				};

			/*var seqItem = new Item { Id = offset + 25, Title = "Test seq " + (offset + 25) };
			var seqItemRecords = new List<Record>();
			var seqCnt = 40;
			for (var i = 0; i < seqCnt; i+=4)
			{
				seqItemRecords.Add(new Record(seqItem, _loader) { Rev = i, Date = DateTime.Now.AddDays(-(seqCnt-i)), State = "Proposed" });
			}
			seqItem.Records = seqItemRecords.ToArray();*/

			var result = new[] { item, item2, item3, item4, item5 };
			foreach (var r in result.SelectMany(i => i.Records))
			{
				fillDisplayFields(r);
			}
			return result;
		}

		private Record[] CreateRecords(Item item, IEnumerable<Tuple<int, DateTime, string>> records)
		{
			var result = new List<Record>();
			foreach (var r in records)
			{
				result.Add(new Record(item, _loader)
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
			model.Items = GetData(++_offset, model.DisplayFields);
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			var model = (DataModel)DataContext;
			var service = new DataService();
			var displayFields = DisplayFieldsForm.ShowAndGetResult(service, model.DisplayFields, this);
			if (displayFields != null)
			{
				model.DisplayFields = displayFields;
				model.Items = GetData(++_offset, model.DisplayFields);
			}
		}
	}
}
