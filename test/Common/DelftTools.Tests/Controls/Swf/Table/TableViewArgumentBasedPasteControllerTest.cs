using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using DelftTools.Controls.Swf.Table;
using DelftTools.TestUtils;
using DelftTools.Utils;
using DelftTools.Utils.Collections.Generic;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Table
{
    [TestFixture]
    [NUnit.Framework.Category(TestCategory.WindowsForms)]
    public class TableViewArgumentBasedPasteControllerTest
    {
        private DataTable dataTable;
        private TableViewArgumentBasedPasteController tableViewCopyPasteController;
        private TableView tableView;

        [TearDown]
        public void TearDown()
        {
            if (tableView != null)
            {
                tableView.Dispose();
                tableView = null;
            }

            if (dataTable != null)
            {
                dataTable.Dispose();
                dataTable = null;
            }
            tableViewCopyPasteController = null;
        }

        private void SetupOneArgumentTable()
        {
            tableView = new TableView();
            var dataset = new DataSet();
            dataTable = dataset.Tables.Add();
            dataTable.Columns.Add("Alpha", typeof (string));
            dataTable.Columns.Add("Numeric", typeof (int));
            dataTable.DefaultView.Sort = "Alpha";
            dataTable.Rows.Add("c", 3);
            dataTable.Rows.Add("b", 2);
            dataTable.Rows.Add("f", 1);
            dataTable.Rows.Add("e", 5);
            dataTable.Rows.Add("d", 4);
            tableView.Data = dataTable;

            tableViewCopyPasteController = new TableViewArgumentBasedPasteController(tableView, new List<int>(new[] { 0 }));
        }

        private void SetupTwoArgumentTable()
        {
            tableView = new TableView();
            var dataset = new DataSet();
            dataTable = dataset.Tables.Add();
            dataTable.Columns.Add("Branch", typeof(string));
            dataTable.Columns.Add("Chainage", typeof(double));
            dataTable.Columns.Add("Value", typeof(int));
            dataTable.DefaultView.Sort = "Branch, Chainage";
            dataTable.Rows.Add("a", 5.0, 3);
            dataTable.Rows.Add("a", 10.0, 2);
            dataTable.Rows.Add("a", 20.0, 1);
            dataTable.Rows.Add("b", 5.0, 5);
            dataTable.Rows.Add("b", 20.0, 4);
            tableView.Data = dataTable;

            tableViewCopyPasteController = new TableViewArgumentBasedPasteController(tableView, new List<int>(new[] { 0, 1 }));
        }

        [Test]
        public void ReorderingPaste()
        {
            SetupOneArgumentTable();

            tableViewCopyPasteController.PasteLines(new[] { "d\t6", "b\t9" });

            Assert.IsTrue(DataTableContains(new object[] { "c", 3 }));
            Assert.IsTrue(DataTableContains(new object[] { "b", 9 }));
            Assert.IsTrue(DataTableContains(new object[] { "f", 1 }));
            Assert.IsTrue(DataTableContains(new object[] { "e", 5 }));
            Assert.IsTrue(DataTableContains(new object[] { "d", 6 }));
        }
        
        [Test]
        public void ReorderAndAddPaste()
        {
            SetupOneArgumentTable();

            tableViewCopyPasteController.PasteLines(new[] { "d\t6", "g\t10", "b\t9", "y\t12" });

            Assert.IsTrue(DataTableContains(new object[] {"c", 3}), "c");
            Assert.IsTrue(DataTableContains(new object[] { "b", 9 }), "b");
            Assert.IsTrue(DataTableContains(new object[] { "f", 1 }), "f");
            Assert.IsTrue(DataTableContains(new object[] { "e", 5 }), "e");
            Assert.IsTrue(DataTableContains(new object[] { "d", 6 }), "d");
            Assert.IsTrue(DataTableContains(new object[] { "g", 10 }), "g");
            Assert.IsTrue(DataTableContains(new object[] { "y", 12 }), "y");
        }

        [Test]
        public void PasteOnFirstLine()
        {
            SetupOneArgumentTable();

            tableView.SelectRow(0);

            Assert.AreEqual(5, tableView.RowCount);

            tableViewCopyPasteController.PasteLines(new[] { "b\t6", "c\t10"});
            
            Assert.AreEqual(5, tableView.RowCount); //no extra row is added
        }

        [Test]
        public void TwoArgumentPasteTooFewArguments()
        {
            SetupTwoArgumentTable();

            int callCount = 0;

            tableViewCopyPasteController.PasteFailed += (s, e) =>
                {
                    Assert.AreEqual(
                        "This table contains multiple argument columns (Branch,Chainage). When pasting, please paste the full row width, or paste into non-argument columns only.",
                        e.Value);
                    callCount++;
                };

            tableView.SelectCells(0,1,1,2);
            tableViewCopyPasteController.PasteLines(new[] {"5.0\t4"});

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void TwoArgumentPasteOnlyComponents()
        {
            SetupTwoArgumentTable();

            int callCount = 0;

            tableViewCopyPasteController.PasteFailed += (s, e) =>
            {
                callCount++;
            };

            tableView.SelectCells(0, 2, 1, 2);
            tableViewCopyPasteController.PasteLines(new[] { "4" });

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void TwoArgumentPasteOnlyArguments()
        {
            SetupTwoArgumentTable();

            int callCount = 0;

            tableViewCopyPasteController.PasteFailed += (s, e) =>
            {
                Assert.AreEqual(
                    "This table contains multiple argument columns (Branch,Chainage). When pasting, please paste the full row width, or paste into non-argument columns only.",
                    e.Value);
                callCount++;
            };

            tableView.SelectCells(0, 0, 1, 2);
            tableViewCopyPasteController.PasteLines(new[] { "5.0\t4" });

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void TwoArgumentReorderAndAdd()
        {
            SetupTwoArgumentTable();

            tableViewCopyPasteController.PasteLines(new[] { 
                String.Format("a\t{0}\t8",15.0), 
                String.Format("c\t{0}\t12",5.0), 
                String.Format("b\t{0}\t13",3.0),
                String.Format("a\t{0}\t1", 20.0)});

            Assert.IsTrue(DataTableContains(new object[] { "a", 5.0, 3}));
            Assert.IsTrue(DataTableContains(new object[] { "a", 10.0, 2 }));
            Assert.IsTrue(DataTableContains(new object[] { "a", 20.0, 1 }));
            Assert.IsTrue(DataTableContains(new object[] { "a", 15.0, 8 }));
            Assert.IsTrue(DataTableContains(new object[] { "b", 5.0, 5 }));
            Assert.IsTrue(DataTableContains(new object[] { "b", 20.0, 4 }));
            Assert.IsTrue(DataTableContains(new object[] { "b", 3.0, 13 }));
            Assert.IsTrue(DataTableContains(new object[] { "c", 5.0, 12 }));
        }

        [Test]
        public void PasteExistingArgumentInAssendingTable()
        {
            using (CultureUtils.SwitchToInvariantCulture())
            using (var tableView = new TableView())
            using (var dataset = new DataSet())
            using (var dataTable = dataset.Tables.Add())
            {
                dataTable.Columns.Add("A", typeof (double));
                dataTable.Columns.Add("B", typeof (double));
                dataTable.Rows.Add(0.0, 10.0);
                dataTable.Rows.Add(10.0, 10.0);

                dataTable.DefaultView.Sort = "A ASC";

                tableView.Data = dataTable;

                var tableViewArgumentBasedPasteController = new TableViewArgumentBasedPasteController(tableView,new List<int>(new[] {0}));
                tableViewArgumentBasedPasteController.PasteLines(new[] {"0.0\t5.0"});
                Assert.AreEqual(2, dataTable.Rows.Count);
                Assert.AreEqual(5.0, (double) dataTable.Rows[0][1], 0.0001);
            }
        }

        [Test]
        public void PasteExistingArgumentInAssendingTableWithoutEqualtyMargin()
        {
            using (CultureUtils.SwitchToInvariantCulture())
            using (var tableView = new TableView())
            using (var dataset = new DataSet())
            using (var dataTable = dataset.Tables.Add())
            {
                dataTable.Columns.Add("A", typeof(double));
                dataTable.Columns.Add("B", typeof(double));
                dataTable.Rows.Add(0.0, 10.0);
                dataTable.Rows.Add(10.0, 10.0);

                dataTable.DefaultView.Sort = "A ASC";

                tableView.Data = dataTable;

                var tableViewArgumentBasedPasteController = new TableViewArgumentBasedPasteController(tableView, new List<int>(new[] { 0 }));
                tableViewArgumentBasedPasteController.PasteLines(new[] { "0.0000000001\t5.0" });
                Assert.AreEqual(2, dataTable.Rows.Count);
                Assert.AreEqual(5.0, (double)dataTable.Rows[0][1], 0.0001);
            }
        }

        [Test]
        public void PasteNewAndExistingArgumentsInTableTools8131()
        {
            using (CultureUtils.SwitchToInvariantCulture())
            using (var tableView = new TableView())
            using (var dataset = new DataSet())
            using (var dataTable = dataset.Tables.Add())
            {
                dataTable.Columns.Add("A", typeof (double));
                dataTable.Constraints.Add(new UniqueConstraint(dataTable.Columns[0]));
                dataTable.Columns.Add("B", typeof (double));
                dataTable.Rows.Add(0.0, 10.0);
                dataTable.Rows.Add(-10.0, 10.0);
                dataTable.DefaultView.Sort = "A DESC";

                tableView.Data = dataTable;

                var tableViewArgumentBasedPasteController = new TableViewArgumentBasedPasteController(tableView,
                                                                                                      new List<int>(
                                                                                                          new[] {0}));
                tableView.SelectRow(0);

                tableViewArgumentBasedPasteController.PasteLines(new[] {"0.0\t10.0", "-5.0\t11.0", "-10.0\t12.0"});
                Assert.AreEqual(3, dataTable.Rows.Count);
                Assert.AreEqual(-5.0, (double) dataTable.DefaultView[1][0], 0.0001);
            }
        }

        [Test]
        public void PasteNewAndExistingArgumentsInTableWithoutDataSet()
        {
            using (CultureUtils.SwitchToInvariantCulture())
            using (var tableView = new TableView())
            using (var dataTable = new DataTable())
            {
                dataTable.Columns.Add("A", typeof (double));
                dataTable.Constraints.Add(new UniqueConstraint(dataTable.Columns[0]));
                dataTable.Columns.Add("B", typeof (double));
                dataTable.Rows.Add(0.0, 10.0);
                dataTable.Rows.Add(-10.0, 10.0);
                dataTable.DefaultView.Sort = "A DESC";

                tableView.Data = dataTable;

                var tableViewArgumentBasedPasteController = new TableViewArgumentBasedPasteController(tableView,
                                                                                                      new List<int>(
                                                                                                          new[] {0}));
                tableView.SelectRow(0);

                tableViewArgumentBasedPasteController.PasteLines(new[] {"0.0\t10.0", "-5.0\t11.0", "-10.0\t12.0"});
                Assert.AreEqual(3, dataTable.Rows.Count);
                Assert.AreEqual(-5.0, (double) dataTable.DefaultView[1][0], 0.0001);
            }
        }

        [Test]
        public void PasteExistingArgumentInDescendingTable()
        {
            using (CultureUtils.SwitchToInvariantCulture())
            using (var tableView = new TableView())
            using (var dataset = new DataSet())
            using (var dataTable = dataset.Tables.Add())
            {
                dataTable.Columns.Add("A", typeof (double));
                dataTable.Columns.Add("B", typeof (double));
                dataTable.Rows.Add(0.0, 10.0);
                dataTable.Rows.Add(-10.0, 10.0);

                dataTable.DefaultView.Sort = "A DESC";

                tableView.Data = dataTable;

                var tableViewArgumentBasedPasteController = new TableViewArgumentBasedPasteController(tableView,
                                                                                                      new List<int>(
                                                                                                          new[] {0}));

                tableViewArgumentBasedPasteController.PasteLines(new[] {"0.0\t5.0"});
                Assert.AreEqual(2, dataTable.Rows.Count);
                Assert.AreEqual(5.0, (double) dataTable.Rows[0][1], 0.0001);
            }
        }

        private bool DataTableContains(object[] objects)
        {
            foreach(var row in dataTable.Rows.OfType<DataRow>())
            {
                if (row.ItemArray.SequenceEqual(objects))
                {
                    return true;
                }
            }
            return false;
        }

        [Test]
        public void PasteValuesIntoNonDataTableUnsorted()
        {
            using (var tableView = new TableView())
            {
                var list = new EventedList<Utils.Tuple<double, double>>();
                list.AddRange(new[]
                    {
                        new Utils.Tuple<double, double>(1.0, 2.0),
                        new Utils.Tuple<double, double>(5.0, 6.0),
                        new Utils.Tuple<double, double>(3.0, 4.0),
                        new Utils.Tuple<double, double>(7.0, 8.0)
                    });

                var bindingList = new BindingList<Utils.Tuple<double, double>>(list)
                    {
                        AllowEdit = true,
                        AllowNew = true,
                        AllowRemove = true
                    };
                tableView.Data = bindingList;

                tableView.AllowAddNewRow = true;
                tableView.AllowColumnSorting = false;
                tableView.AllowColumnFiltering = false;

                var pasteController = new TableViewArgumentBasedPasteController(tableView, new List<int>(new[] {0}))
                    {
                        DataIsSorted = false
                    };

                pasteController.PasteLines(new[] { "2\t9", "11\t2", "3\t3", "5\t12", "-1\t17", "17\t12" });

                Assert.AreEqual(8, list.Count);
                Assert.AreEqual(new Utils.Tuple<double, double>(1.0, 2.0), list[0]);
                Assert.AreEqual(new Utils.Tuple<double, double>(5.0, 12.0), list[1]);
                Assert.AreEqual(new Utils.Tuple<double, double>(3.0, 3.0), list[2]);
                Assert.AreEqual(new Utils.Tuple<double, double>(7.0, 8.0), list[3]);
                Assert.AreEqual(new Utils.Tuple<double, double>(2.0, 9.0), list[4]);
                Assert.AreEqual(new Utils.Tuple<double, double>(11.0, 2.0), list[5]);
                Assert.AreEqual(new Utils.Tuple<double, double>(-1.0, 17.0), list[6]);
                Assert.AreEqual(new Utils.Tuple<double, double>(17.0, 12.0), list[7]);
            }
        }
    }
}
