using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Table;
using DelftTools.TestUtils;
using DelftTools.Utils;
using DelftTools.Utils.Collections.Generic;
using DevExpress.XtraGrid;
using NUnit.Framework;
using Category = NUnit.Framework.CategoryAttribute;

namespace DelftTools.Tests.Controls.Swf.Table
{
    [TestFixture]
    public class TableViewCopyPasteTest
    {
        [Test]
        public void CopyPasteDateTimeTools8628()
        {
            using (var tableView = new TableView())
            using (var dataset = new DataSet())
            using (var dataTable = dataset.Tables.Add())
            {
                dataTable.Columns.Add("A", typeof (DateTime));
                dataTable.Columns.Add("B", typeof (double));
                dataTable.Rows.Add(new DateTime(2000, 1, 31), 10.0);
                dataTable.Rows.Add(new DateTime(2000, 2, 1), 15.0);

                tableView.Data = dataTable;

                tableView.SelectRows(new[] {0, 1}); //select all
                tableView.CopySelectionToClipboard();
                dataTable.Clear();

                tableView.SelectRow(0);
                tableView.PasteClipboardContents();

                Assert.AreEqual(2, dataTable.Rows.Count);
                Assert.AreEqual(new DateTime(2000, 1, 31), dataTable.Rows[0][0]);
                Assert.AreEqual(10.0, dataTable.Rows[0][1]);
            }
        }

        [Test]
        public void PasteLargeAmountOfDataShouldBeFast()
        {
            using (CultureUtils.SwitchToCulture("nl-NL"))
            {
                using (var dataset = new DataSet())
                using (var dataTable = dataset.Tables.Add())
                {
                    dataTable.Columns.Add("A", typeof(DateTime));
                    dataTable.Columns.Add("B", typeof(double));

                    var view = new TableView
                               {
                                   Data = dataTable
                               };

                    var file = TestHelper.GetTestFilePath("TestPasteData.txt");
                    var contents = File.ReadAllText(file);
                    Clipboard.SetText(contents);

                    TestHelper.AssertIsFasterThan(17500, view.PasteClipboardContents);

                    Assert.Greater(dataTable.Rows.Count, 5);
                }
            }
        }

        [Test]
        public void PasteDutchDateTimeShouldWorkFineTools8878()
        {
            var list = new EventedList<Utils.Tuple<DateTime, double>>();
            var bindingList = new BindingList<Utils.Tuple<DateTime, double>>(list)
            {
                AllowEdit = true,
                AllowNew = true,
                AllowRemove = true
            };

            var view = new TableView();
            view.PasteController = new TableViewArgumentBasedPasteController(view, new List<int>(new[] { 0 }))
            {
                DataIsSorted = false
            };

            view.Data = bindingList;

            Clipboard.SetText(File.ReadAllText(TestHelper.GetTestFilePath("TestPasteData_DutchDates.txt")));

            using (CultureUtils.SwitchToCulture("nl-NL"))
            {
                view.PasteClipboardContents();
            }

            var dateTimes = list.Select(t => t.First).ToList();

            Assert.AreEqual(60, dateTimes.Count);
            Assert.AreEqual(new DateTime(2001, 1, 1), dateTimes.Min());
            Assert.AreEqual(new DateTime(2001, 3, 1), dateTimes.Max());
        }

        [Test]
        public void PasteClipboardContentsCanAddRows()
        {
            var person = new List<Person> { new Person { Age = 12, Name = "Hoi" }, new Person { Age = 11, Name = "keers" } };
            //set two persons in clipboard
            const string clipBoardContents = "cees anton\t34\r\nsaifon\t66\r\nmartijn\t31\r\n";
            Clipboard.SetText(clipBoardContents);
            //setup a tableview
            var tableView = new TableView { Data = person};

            tableView.PasteClipboardContents();

            Assert.AreEqual("martijn", person[2].Name);
            Assert.AreEqual(31, person[2].Age);
        }

        [Test]
        public void PasteIntoEmptyTableView()
        {
            var persons = new List<Person>();
            //set two persons in clipboard
            const string clipBoardContents = "cees anton\t34\r\nsaifon\t66\r\nmartijn\t31\r\n";
            Clipboard.SetText(clipBoardContents);
            //setup a tableview
            var tableView = new TableView { Data = persons };

            //action!
            tableView.PasteClipboardContents();

            Assert.AreEqual("martijn", persons[2].Name);
            Assert.AreEqual(31, persons[2].Age);
            Assert.AreEqual(3, persons.Count);
        }

        [Test]
        public void PasteColumnIntoEmptyTableView()
        {
            var persons = new List<Person>();
            //set two persons in clipboard
            const string clipBoardContents = "1\r\n2\r\n3\r\n";
            Clipboard.SetText(clipBoardContents);
            //setup a tableview
            var tableView = new TableView {Data = persons};

            //action!
            tableView.PasteClipboardContents();

            Assert.AreEqual(new[] { "1", "2", "3" }, persons.Select(p => p.Name).ToArray());
            //assert the data is not pasted into the other column
            Assert.AreEqual(new[] { 0, 0, 0 }, persons.Select(p => p.Age).ToArray());
        }

        [Test]
        public void PasteClipboardContentsOverwritesExistingRows()
        {
            var person = new List<Person> { new Person { Age = 12, Name = "Hoi" }, new Person { Age = 11, Name = "keers" } };
            //set two persons in clipboard
            const string clipBoardContents = "cees anton\t34\r\nsaifon\t66\r\n";
            Clipboard.SetText(clipBoardContents);
            //setup a tableview
            var tableView = new TableView { Data = person };

            tableView.PasteClipboardContents();

            Assert.AreEqual("cees anton", person[0].Name);
            Assert.AreEqual(34, person[0].Age);
            Assert.AreEqual("saifon", person[1].Name);
            Assert.AreEqual(66, person[1].Age);
        }

        [Test]
        public void CopySelectionIntoClipBoard()
        {
            var table = new DataTable();
            table.Columns.Add("column1", typeof(string));
            table.Columns.Add("column2", typeof(string));
            table.Rows.Add("1", "2");
            table.Rows.Add("3", "4");

            var tableView = new TableView { Data = table };

            //select two rows
            tableView.SelectCells(0, 0, 1, 1);

            //action! copy selection to clipboard
            //tableView.CopySelection
            tableView.CopySelectionToClipboard();

            Assert.AreEqual("1\t2\r\n3\t4\r\n", Clipboard.GetText());
        }

        [Test]
        public void PasteIntoNewRow()
        {
            //this test relates to issue 3069...demonstrating a problem paste lines when rowselect is enabled.
            var table = new DataTable();
            table.Columns.Add("column1", typeof (string));
            table.Columns.Add("column2", typeof (string));
            table.Rows.Add("1", "2");
            table.Rows.Add("3", "4");

            var tableView = new TableView {Data = table, RowSelect = true};

            const string clipBoardContents = "5\t6\r\n";
            Clipboard.SetText(clipBoardContents);

            //first select the 1st row
            tableView.SelectRow(0);

            //add a new row
            table.Rows.Add();
            
            //focus on the left cell of the new row
            tableView.SetFocus(GridControl.NewItemRowHandle, 0);

            tableView.PasteClipboardContents();

            //check a new row was added to the table
            Assert.AreEqual(3, table.Rows.Count);

            //check that the first row equals the clip board contents
            var rowView = tableView.CurrentFocusedRowObject as DataRowView;
            var rowData = (rowView == null) ? null : rowView.Row.ItemArray;
            Assert.AreEqual(rowData, new object[] { "5", "6" });
        }
    }
}
