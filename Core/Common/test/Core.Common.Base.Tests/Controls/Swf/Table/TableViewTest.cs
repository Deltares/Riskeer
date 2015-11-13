using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base.Tests.Controls.Swf.Table.TestClasses;
using Core.Common.Controls;
using Core.Common.Controls.Swf.Editors;
using Core.Common.Controls.Swf.Table;
using Core.Common.TestUtils;
using Core.Common.Utils.Collections.Generic;
using Core.Common.Utils.Globalization;
using Core.Common.Utils.Reflection;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using NUnit.Framework;
using Rhino.Mocks;
using SharpTestsEx;

namespace Core.Common.Base.Tests.Controls.Swf.Table
{
    [TestFixture]
    public class TableViewTest
    {
        [Test]
        public void PinColumns()
        {
            using (var dataset = new DataSet())
            {
                using (var dataTable = dataset.Tables.Add())
                {
                    dataTable.Columns.Add("A", typeof(double));
                    dataTable.Columns.Add("B", typeof(double));
                    dataTable.Columns.Add("C", typeof(double));
                    dataTable.Columns.Add("D", typeof(double));
                    dataTable.Columns.Add("E", typeof(double));

                    dataTable.Rows.Add(1, 2, 3, 4, 5);
                    dataTable.Rows.Add(6, 7, 8, 9, 10);
                    dataTable.Rows.Add(11, 12, 13, 14, 15);
                    dataTable.Rows.Add(16, 17, 18, 19, 20);
                    dataTable.Rows.Add(21, 22, 23, 24, 25);

                    var tableView = new TableView
                    {
                        Data = dataTable
                    };

                    var column0 = tableView.Columns[0];
                    var column1 = tableView.Columns[1];
                    var column3 = tableView.Columns[3];
                    var column4 = tableView.Columns[4];

                    column3.ReadOnly = true;

                    var index3 = column3.DisplayIndex;
                    var index4 = column4.DisplayIndex;

                    column3.Pinned = true;
                    column4.Pinned = true;

                    column3.Pinned = false;

                    // expect original index + pinned column 4
                    Assert.AreEqual(index3 + 1, column3.DisplayIndex);

                    column4.Pinned = false;

                    // expect original indices (no pinned columns)
                    Assert.AreEqual(index3, column3.DisplayIndex);
                    Assert.AreEqual(index4, column4.DisplayIndex);

                    column0.Pinned = true;
                    column1.Pinned = true;
                    Assert.AreEqual(0, column0.DisplayIndex, "display index of first pinned column");
                    Assert.AreEqual(1, column1.DisplayIndex, "display index of second pinned column");

                    column0.Pinned = false;
                    Assert.AreEqual(1, column0.DisplayIndex, "display index of first unpinned column");
                    Assert.AreEqual(0, column1.DisplayIndex, "display index of pinned column");
                }
            }
        }

        [Test]
        public void FilterIntegerValues()
        {
            var tableView = new TableView
            {
                Data = new[]
                {
                    1,
                    2,
                    3,
                    4,
                    5
                },
                ReadOnly = true
            };

            int callCount = 0;

            tableView.ColumnFilterChanged += (s, e) => { callCount++; };

            tableView.Columns[0].FilterString = "Column > 2";

            Assert.AreEqual(1, callCount);
            Assert.AreEqual(3, tableView.RowCount);
        }

        [Test]
        public void EmptyTableViewHasNoFocusedCell()
        {
            var tableView = new TableView();
            var cell = TypeUtils.CallPrivateMethod<TableViewCell>(tableView, "GetFocusedCell");
            Assert.AreEqual(null, cell);
        }

        [Test]
        public void TableViewWithNoDataHasNoFocusedCell()
        {
            var tableView = new TableView
            {
                Data = new List<Person>()
            };
            var cell = TypeUtils.CallPrivateMethod<TableViewCell>(tableView, "GetFocusedCell");
            Assert.AreEqual(null, cell);
        }

        [Test]
        public void DefaultFocusedCellIsTopLeft()
        {
            //tableview with a single row
            var tableView = new TableView
            {
                Data = new List<Person>
                {
                    new Person
                    {
                        Age = 10, Name = "Piet"
                    }
                }
            };

            var cell = TypeUtils.CallPrivateMethod<TableViewCell>(tableView, "GetFocusedCell");

            Assert.AreEqual(0, cell.Column.DisplayIndex);
            Assert.AreEqual(0, cell.RowIndex);
        }

        [Test]
        public void TableViewShouldUseDefaultValuesWhenDeleting()
        {
            using (var dataset = new DataSet())
            {
                using (var dataTable = dataset.Tables.Add())
                {
                    dataTable.Columns.Add("A", typeof(double));
                    dataTable.Columns.Add("B", typeof(double));
                    dataTable.Columns.Add("C", typeof(double));

                    dataTable.Rows.Add(1, 2, 3);
                    dataTable.Rows.Add(4, 5, 6);
                    dataTable.Rows.Add(7, 8, 9);
                    dataTable.Rows.Add(10, 11, 12);

                    var tableView = new TableView
                    {
                        Data = dataTable
                    };

                    tableView.Columns[1].ReadOnly = false;
                    tableView.Columns[1].DefaultValue = 99.9;
                    tableView.Columns[2].ReadOnly = false;
                    tableView.Columns[2].DefaultValue = double.NaN;

                    tableView.AllowColumnSorting = false;
                    tableView.AllowColumnFiltering = false;

                    tableView.SelectCells(1, 1, 1, 2);
                    tableView.DeleteCurrentSelection();

                    var secondIndexValue = (double) tableView.GetCellValue(1, 1);
                    Assert.AreEqual(99.9, secondIndexValue, "the deleted double column value becomes the given default value.");
                    var thirdIndexValue = (double) tableView.GetCellValue(1, 2);
                    Assert.AreEqual(double.NaN, thirdIndexValue, "the deleted double column value becomes the given default value.");
                }
            }
        }

        [Test] //devexpress code modified to prevent pascal casing to be split into words
        public void TableViewShouldUpdateColums()
        {
            var table = new DataTable("tablename");
            var view = new TableView
            {
                Data = table
            };
            //view.Show();
            var column = new DataColumn("testName", typeof(double))
            {
                Caption = "testCaption"
            };
            table.Columns.Add(column);
            Assert.AreEqual(1, view.Columns.Count);
            Assert.AreEqual("testName", view.Columns[0].Name);
            Assert.AreEqual("testCaption", view.Columns[0].Caption);

            column.Caption = "newTestCaption";
            view.ResetBindings();

            Assert.AreEqual("newTestCaption", view.Columns[0].Caption);
        }

        [Test]
        public void FilterDateTimeColumn()
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var tableView = new TableView();
            var table = new DataTable();

            table.Columns.Add("A", typeof(DateTime));
            table.Columns.Add("B", typeof(double));

            var row = table.NewRow();

            var dateTime = DateTime.Now;
            var dateTimeMinus1Day = dateTime - new TimeSpan(1, 0, 0, 0);
            var dateTimePlus1Day = dateTime.AddDays(1);

            row["A"] = dateTime;
            row["B"] = 0;

            table.Rows.Add(row);
            tableView.Data = table;

            tableView.SetColumnFilter("A", string.Format("[A] >= #{0}# And [A] < #{1}#", dateTimeMinus1Day, dateTimePlus1Day));

            Assert.AreEqual(1, tableView.RowCount);

            Thread.CurrentThread.CurrentCulture = oldCulture;
        }

        [Test]
        public void SetTextToTableViewWithDateTimeColumnUsCulture()
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            var specificCulture = CultureInfo.CreateSpecificCulture("en-US");
            specificCulture.NumberFormat.NumberGroupSeparator = ",";
            specificCulture.NumberFormat.NumberDecimalSeparator = ".";

            Thread.CurrentThread.CurrentCulture = specificCulture;

            var tableView = new TableView();

            var table = new DataTable();

            table.Columns.Add("A", typeof(DateTime));
            table.Columns.Add("B", typeof(double));
            var row = table.NewRow();
            row["A"] = DateTime.Now;
            row["B"] = 0;
            table.Rows.Add(row);
            tableView.Data = table;

            tableView.SetCellValue(0, 0, "2010/11/18 01:02:03");
            var dateTime = DateTime.Parse(tableView.GetCellValue(0, 0).ToString());
            Assert.AreEqual(new DateTime(2010, 11, 18, 01, 02, 03), dateTime);

            tableView.SetCellValue(0, 1, "1,234.5");
            var value = double.Parse(tableView.GetCellValue(0, 1).ToString());

            Assert.AreEqual(1234.5, value, "Expects uncustomized en-US number formatting; " +
                                           "If this fails, please check if local number format for en-US culture was modified.");

            Thread.CurrentThread.CurrentCulture = oldCulture;
        }

        [Test]
        public void SetTextToTableViewWithDateTimeColumnNlCulture()
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            var specificCulture = CultureInfo.CreateSpecificCulture("nl-NL");
            specificCulture.NumberFormat.NumberGroupSeparator = ".";
            specificCulture.NumberFormat.NumberDecimalSeparator = ",";

            Thread.CurrentThread.CurrentCulture = specificCulture;

            var tableView = new TableView();

            var table = new DataTable();

            table.Columns.Add("A", typeof(DateTime));
            table.Columns.Add("B", typeof(double));
            var row = table.NewRow();
            row["A"] = DateTime.Now;
            row["B"] = 0;
            table.Rows.Add(row);
            tableView.Data = table;

            tableView.SetCellValue(0, 0, "2010/11/18 01:02:03");
            var dateTime = DateTime.Parse(tableView.GetCellValue(0, 0).ToString());
            Assert.AreEqual(new DateTime(2010, 11, 18, 01, 02, 03), dateTime);

            tableView.SetCellValue(0, 1, "1.234,5");

            var value = double.Parse(tableView.GetCellValue(0, 1).ToString());

            Assert.AreEqual(1234.5, value);

            Thread.CurrentThread.CurrentCulture = oldCulture;
        }

        [Test]
        public void ExportToCsv()
        {
            var tableView = new TableView();

            var table = new DataTable();

            table.Columns.Add("A", typeof(DateTime));
            table.Columns.Add("B", typeof(int));
            for (int i = 0; i < 50; i++)
            {
                DataRow row = table.NewRow();
                row["A"] = DateTime.Now;
                row["B"] = i;
                table.Rows.Add(row);
            }

            tableView.Data = table;

            var path = "ExportToCsv.csv";
            tableView.ExportAsCsv(path);

            var exportedLines = File.ReadAllLines(path);

            Assert.AreEqual(51, exportedLines.Length);
            Assert.AreEqual("A, B", exportedLines[0]);
        }

        [Test]
        public void SetDataSource()
        {
            var table = new DataTable("table1");

            var tableView = new TableView
            {
                Data = table
            };

            Assert.AreEqual(table, tableView.Data, "Data assigned to TableView must not change by itself");
        }

        [Test]
        public void CopyOriginalValuesToClipBoard()
        {
            var table = new DataTable();
            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(double));

            var double1 = 1.1111111111;
            var double2 = 2.22222222;
            var double3 = 3.33333333333333;
            var double4 = 4.44444444444;
            var double5 = 5.5555555;
            var double6 = 6.6666666666;

            DataRow row = table.NewRow();
            row["A"] = double1;
            row["B"] = double2;
            table.Rows.Add(row);

            row = table.NewRow();
            row["A"] = double3;
            row["B"] = double4;
            table.Rows.Add(row);

            row = table.NewRow();
            row["A"] = double5;
            row["B"] = double6;
            table.Rows.Add(row);

            RegionalSettingsManager.RealNumberFormat = "N2";

            var tableView = new TableView
            {
                Data = table
            };

            Assert.AreEqual(double1.ToString("N2"), tableView.GetCellDisplayText(0, 0));

            tableView.SelectCells(1, 0, 2, 1);

            Assert.AreEqual(4, tableView.SelectedCells.Count);

            tableView.CopySelectionToClipboard();

            var clipBoardText = Clipboard.GetText();

            Assert.AreEqual(RegionalSettingsManager.ConvertToString(double3, false) +
                            "\t" + RegionalSettingsManager.ConvertToString(double4, false) +
                            "\r\n" + RegionalSettingsManager.ConvertToString(double5, false) +
                            "\t" + RegionalSettingsManager.ConvertToString(double6, false) + "\r\n",
                            clipBoardText);
        }

        [Test]
        public void CopyOriginalValuesToClipBoardFromSortedTable()
        {
            var table = new DataTable();
            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(double));

            var double1 = 1.1111111111;
            var double2 = 2.22222222;
            var double3 = 3.33333333333333;
            var double4 = 4.44444444444;
            var double5 = 5.5555555;
            var double6 = 6.6666666666;

            DataRow row = table.NewRow();
            row["A"] = double1;
            row["B"] = double2;
            table.Rows.Add(row);

            row = table.NewRow();
            row["A"] = double3;
            row["B"] = double4;
            table.Rows.Add(row);

            row = table.NewRow();
            row["A"] = double5;
            row["B"] = double6;
            table.Rows.Add(row);

            RegionalSettingsManager.RealNumberFormat = "N2";

            var tableView = new TableView
            {
                Data = table
            };

            tableView.Columns[1].SortOrder = SortOrder.Descending;

            Assert.AreEqual(double5.ToString("N2"), tableView.GetCellDisplayText(0, 0));

            tableView.SelectCells(1, 0, 2, 1);

            Assert.AreEqual(4, tableView.SelectedCells.Count);

            tableView.CopySelectionToClipboard();

            var clipBoardText = Clipboard.GetText();

            Assert.AreEqual(RegionalSettingsManager.ConvertToString(double3, false) +
                            "\t" + RegionalSettingsManager.ConvertToString(double4, false) +
                            "\r\n" + RegionalSettingsManager.ConvertToString(double1, false) +
                            "\t" + RegionalSettingsManager.ConvertToString(double2, false) + "\r\n",
                            clipBoardText);
        }

        [Test]
        public void CopyOriginalValuesAndHeadersToClipBoard()
        {
            var table = new DataTable();
            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(double));

            var double1 = 1.1111111111;
            var double2 = 2.22222222;
            var double3 = 3.33333333333333;
            var double4 = 4.44444444444;
            var double5 = 5.5555555;
            var double6 = 6.6666666666;

            DataRow row = table.NewRow();
            row["A"] = double1;
            row["B"] = double2;
            table.Rows.Add(row);

            row = table.NewRow();
            row["A"] = double3;
            row["B"] = double4;
            table.Rows.Add(row);

            row = table.NewRow();
            row["A"] = double5;
            row["B"] = double6;
            table.Rows.Add(row);

            RegionalSettingsManager.RealNumberFormat = "N2";

            var tableView = new TableView
            {
                Data = table
            };
            tableView.IncludeHeadersOnCopy = true;

            Assert.AreEqual(double1.ToString("N2"), tableView.GetCellDisplayText(0, 0));

            tableView.SelectCells(0, 0, 2, 1);

            Assert.AreEqual(6, tableView.SelectedCells.Count);

            tableView.CopySelectionToClipboard();

            var clipBoardText = Clipboard.GetText();
            Assert.AreEqual("A\tB\r\n" + RegionalSettingsManager.ConvertToString(double1, false) +
                            "\t" + RegionalSettingsManager.ConvertToString(double2, false) +
                            "\r\n" + RegionalSettingsManager.ConvertToString(double3, false) +
                            "\t" + RegionalSettingsManager.ConvertToString(double4, false) +
                            "\r\n" + RegionalSettingsManager.ConvertToString(double5, false) +
                            "\t" + RegionalSettingsManager.ConvertToString(double6, false) + "\r\n",
                            clipBoardText);
        }

        [Test]
        public void ComboBoxTypeEditorNonMandatoryWithCustomFormatter_ShouldNotOverwriteColumnFormatting_Tools7594()
        {
            // types of the item, to be shown in the combo box
            var itemTypes = new[]
            {
                new ItemType
                {
                    Name = "item type1"
                },
                new ItemType
                {
                    Name = "item type2"
                },
                new ItemType
                {
                    Name = "item type3"
                }
            };

            // default items
            var items = new EventedList<Item>
            {
                new Item
                {
                    Name = "item1", Type = itemTypes[0]
                },
                new Item
                {
                    Name = "item2", Type = itemTypes[1]
                }
            };

            var tableView = new TableView
            {
                Data = new BindingList<Item>(items)
            };

            var comboBoxTypeEditor = new ComboBoxTypeEditor
            {
                CustomFormatter = new AddBracketTestFormatter(),
                Items = itemTypes,
                ItemsMandatory = false
            };

            tableView.Columns[1].CustomFormatter = new AddDashTestFormatter();
            tableView.Columns[1].Editor = comboBoxTypeEditor; // inject it under the 2nd column

            WindowsFormsTestHelper.ShowModal(tableView, f => tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("-item type1-"));
        }

        [Test]
        public void ComboBoxTypeEditorNonMandatoryWithCustomFormatter_ShouldWork_Tools7594()
        {
            // types of the item, to be shown in the combo box
            var itemTypes = new[]
            {
                new ItemType
                {
                    Name = "item type1"
                },
                new ItemType
                {
                    Name = "item type2"
                },
                new ItemType
                {
                    Name = "item type3"
                }
            };

            // default items
            var items = new EventedList<Item>
            {
                new Item
                {
                    Name = "item1", Type = itemTypes[0]
                },
                new Item
                {
                    Name = "item2", Type = itemTypes[1]
                }
            };

            var tableView = new TableView
            {
                Data = new BindingList<Item>(items)
            };

            var comboBoxTypeEditor = new ComboBoxTypeEditor
            {
                CustomFormatter = new AddBracketTestFormatter(),
                Items = itemTypes,
                ItemsMandatory = false
            };

            tableView.Columns[1].CustomFormatter = new AddDashTestFormatter();
            tableView.Columns[1].Editor = comboBoxTypeEditor; // inject it under the 2nd column

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                var dxColumn = tableView.Columns[1].FieldValue<GridColumn>("dxColumn");
                var dxEditor = (RepositoryItemComboBox) dxColumn.ColumnEdit;
                TypeUtils.GetPropertyValue(dxEditor.Items[0], "DisplayText").Should().Be.EqualTo("[item type1]");
            });
        }

        [Test]
        public void ComboBoxTypeEditorWithCustomFormatter_ShouldNotOverwriteColumnFormatting_Tools7594()
        {
            // types of the item, to be shown in the combo box
            var itemTypes = new[]
            {
                new ItemType
                {
                    Name = "item type1"
                },
                new ItemType
                {
                    Name = "item type2"
                },
                new ItemType
                {
                    Name = "item type3"
                }
            };

            // default items
            var items = new EventedList<Item>
            {
                new Item
                {
                    Name = "item1", Type = itemTypes[0]
                },
                new Item
                {
                    Name = "item2", Type = itemTypes[1]
                }
            };

            var tableView = new TableView
            {
                Data = new BindingList<Item>(items)
            };

            var comboBoxTypeEditor = new ComboBoxTypeEditor
            {
                CustomFormatter = new AddBracketTestFormatter(),
                Items = itemTypes
            };

            tableView.Columns[1].CustomFormatter = new AddDashTestFormatter();
            tableView.Columns[1].Editor = comboBoxTypeEditor; // inject it under the 2nd column

            // Test (What should be visible in the GUI):
            // * Table view should show to entries using 'AddDashTestFormatter' in 2nd column only.
            // * Editor of 2nd column should show entries using 'AddBrackerTestFormatter'.
            // * Transitioning between editing (dropdown box visible) and non-editing (no dropdown box visible) should work with the correct custom formatter.
            WindowsFormsTestHelper.ShowModal(tableView, f => tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("-item type1-"));
        }

        [Test]
        public void DefaultSelectionIsEmpty()
        {
            var table = new DataTable();
            table.Columns.Add("column1", typeof(string));
            table.Columns.Add("column2", typeof(string));
            table.Rows.Add("1", "2");
            table.Rows.Add("3", "4");

            var tableView = new TableView
            {
                Data = table
            };

            tableView.SelectedCells
                     .Should().Be.Empty();
        }

        [Test]
        public void SetRowCellValue()
        {
            var persons = new BindingList<Person>
            {
                new Person
                {
                    Age = 22, Name = "Jan"
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };

            //action! change the top left cell
            tableView.SetCellValue(0, 0, "Kees");

            Assert.AreEqual("Kees", persons[0].Name);
        }

        [Test]
        public void SetRowCellValueIsBasedOnVisibleIndexes()
        {
            var persons = new BindingList<Person>
            {
                new Person
                {
                    Age = 22, Name = "Jan"
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };

            //hide the first column
            tableView.Columns[0].Visible = false;

            //action! change the top left cell
            tableView.SetCellValue(0, 0, "23");

            Assert.AreEqual(23, persons[0].Age);
        }

        [Test]
        public void SetRowCellValueForCustomColumnOrder()
        {
            var tableView = new TableView();
            var persons = new List<Person>
            {
                new Person
                {
                    Age = 12, Name = "Hoi"
                },
                new Person
                {
                    Age = 11, Name = "keers"
                }
            };
            tableView.Data = persons;
            //set the age first and name second 
            tableView.Columns[0].DisplayIndex = 1;
            tableView.Columns[1].DisplayIndex = 0;

            tableView.SetCellValue(0, 1, "jantje");
            Assert.AreEqual("jantje", persons[0].Name);
        }

        [Test]
        public void SetRowCellValueDoesNotWorkOnReadOnlyTableView()
        {
            var table = new DataTable();
            table.Columns.Add("readonlycolumn", typeof(string));

            table.Rows.Add("1");

            var tableView = new TableView
            {
                Data = table
            };

            //read only first column
            tableView.ReadOnly = true;

            tableView.SetCellValue(0, 0, "2");

            Assert.AreEqual("1", tableView.GetCellValue(0, 0));
        }

        [Test]
        public void SetRowCellValueDoesNotWorkOnReadOnlyColumns()
        {
            var table = new DataTable();
            table.Columns.Add("readonlycolumn", typeof(string));

            table.Rows.Add("1");

            var tableView = new TableView
            {
                Data = table
            };

            //read only first column
            tableView.Columns[0].ReadOnly = true;

            tableView.SetCellValue(0, 0, "2");

            Assert.AreEqual("1", tableView.GetCellValue(0, 0));
        }

        [Test]
        public void SetRowCellValueDoesNotWorkOnReadOnlyCell()
        {
            var table = new DataTable();
            table.Columns.Add("readonlycolumn", typeof(string));

            table.Rows.Add("1");

            var tableView = new TableView
            {
                Data = table
            };

            //read only cell
            tableView.ReadOnlyCellFilter += delegate { return true; };

            tableView.SetCellValue(0, 0, "2");

            Assert.AreEqual("1", tableView.GetCellValue(0, 0));
        }

        [Test]
        public void SetRowCellValueOnInvalidCellDoesNotCommit()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Age = 12, Name = "Hoi"
                },
                new Person
                {
                    Age = 11, Name = "keers"
                }
            };
            var tableView = new TableView
            {
                Data = persons,
                InputValidator = (tvc, obj) => new Tuple<string, bool>("first", false)
            };

            //try to set the name on the first column...should fail

            tableView.SetCellValue(0, 0, "oeps");

            //assert the new value did not get commited
            Assert.AreEqual("Hoi", persons[0].Name);
        }

        [Test]
        public void RowCount()
        {
            //empty tableview should have 0 rowcount
            var persons = new BindingList<Person>();
            var tableView = new TableView
            {
                Data = persons
            };
            Assert.AreEqual(0, tableView.RowCount);

            //1 person one row right?
            persons = new BindingList<Person>
            {
                new Person
                {
                    Age = 22, Name = "Jan"
                }
            };
            tableView = new TableView
            {
                Data = persons
            };
            Assert.AreEqual(1, tableView.RowCount);
        }

        [Test]
        public void SelectCells()
        {
            //empty tableview should have 0 rowcount
            var persons = new BindingList<Person>();
            for (int i = 0; i < 10; i++)
            {
                persons.Add(new Person());
            }
            var tableView = new TableView
            {
                Data = persons
            };

            //select cells
            tableView.SelectCells(5, 0, 9, 1);

            //check the bottom cells are all selected
            Assert.AreEqual(10, tableView.SelectedCells.Count);
        }

        [Test]
        public void ChangeSelection()
        {
            //empty tableview should have 0 rowcount
            var persons = new BindingList<Person>();
            for (int i = 0; i < 10; i++)
            {
                persons.Add(new Person());
            }
            var tableView = new TableView
            {
                Data = persons
            };
            var selectionChangedCount = 0;
            var selectionChangedRowsCount = 0;

            tableView.SelectionChanged += delegate(object sender, TableSelectionChangedEventArgs e)
            {
                selectionChangedCount++;
                selectionChangedRowsCount = e.Cells.Count;
            };
            //select cells
            tableView.SelectCells(5, 0, 9, 1);

            Assert.AreEqual(1, selectionChangedCount);
            Assert.AreEqual(10, selectionChangedRowsCount);
        }

        [Test]
        public void SortTableViewWhenSettingSortOrder()
        {
            var person = new List<Person>
            {
                new Person
                {
                    Age = 12, Name = "Aaltje"
                },
                new Person
                {
                    Age = 11, Name = "Berend"
                }
            };
            var tableView = new TableView
            {
                Data = person
            };

            //first sort descending..the first value should be berend
            tableView.Columns[0].SortOrder = SortOrder.Descending;
            Assert.AreEqual("Berend", tableView.GetCellValue(0, 0));

            //sort ascending..the first value should be aaltje
            tableView.Columns[0].SortOrder = SortOrder.Ascending;

            Assert.AreEqual("Aaltje", tableView.GetCellValue(0, 0));
        }

        [Test]
        public void FilterTableOnSettingFilterText()
        {
            var person = new List<Person>
            {
                new Person
                {
                    Age = 12, Name = "Aaltje"
                },
                new Person
                {
                    Age = 11, Name = "Berend"
                }
            };
            var tableView = new TableView
            {
                Data = person
            };

            //action! set a filter
            tableView.Columns[0].FilterString = "[Name] Like 'Jaap%'";

            //no row should be visible
            Assert.AreEqual(0, tableView.RowCount);
        }

        [Test]
        public void IsSorted()
        {
            var person = new List<Person>();
            var tableView = new TableView
            {
                Data = person
            };

            //should be false first
            Assert.IsFalse(TypeUtils.CallPrivateMethod<bool>(tableView, "IsSorted"));

            //action! set a sort
            tableView.Columns[0].SortOrder = SortOrder.Descending;

            Assert.IsTrue(TypeUtils.CallPrivateMethod<bool>(tableView, "IsSorted"));
        }

        [Test]
        public void DeleteSelectionRemovesValueIfCellWasSelect()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };

            //select the twelve
            tableView.SelectCells(0, 1, 0, 1);

            //action! delete the row
            tableView.DeleteCurrentSelection();

            //assert we 'deleted' the age
            Assert.AreEqual(0, persons[0].Age);
        }

        [Test]
        public void DeleteSelectionRemovesRowIfAllCellsInARowAreSelected()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };

            tableView.SelectCells(0, 0, 0, 3); // Select the whole Aaltje row
            tableView.SelectCells(1, 0, 1, 0, false); // Append to selection only the Berends name cell

            tableView.DeleteCurrentSelection();

            // Selection does not form a complete selection of rows: cell-deletion expected
            Assert.AreEqual("", persons[0].Name);
            Assert.AreEqual(0, persons[0].Age);
            Assert.AreEqual("", persons[1].Name);
            Assert.AreEqual(11, persons[1].Age);

            tableView.SelectCells(0, 0, 1, 3); // Select all cells: row deletion
            tableView.DeleteCurrentSelection();

            Assert.AreEqual(0, persons.Count);
        }

        [Test]
        public void RowDeleteHandlerTest()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };
            tableView.RowDeleteHandler += delegate
            {
                persons.RemoveAt(0);
                return true;
            };

            tableView.SelectRow(1); // Selection should not matter.
            tableView.DeleteCurrentSelection();

            Assert.AreEqual(1, persons.Count);
            Assert.AreEqual("Berend", persons[0].Name);
        }

        [Test]
        public void DeleteSelectionRemovesRowIfRowSelectedAndRowSelectIsEnabled()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };
            tableView.RowSelect = true;

            //select the top row
            tableView.SelectRow(0);

            //action! delete the row
            tableView.DeleteCurrentSelection();

            //assert we only have berend now
            Assert.AreEqual(new[]
            {
                "Berend"
            }, persons.Select(p => p.Name).ToArray());
        }

        [Test]
        public void DeleteSelectionTakesAllowDeleteRowsIntoAccount()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };

            //select the top row
            tableView.SelectCells(0, 0, 0, 1);

            //action! delete the row
            tableView.AllowDeleteRow = false;
            tableView.DeleteCurrentSelection();

            //assert aaltje got 'reset'
            Assert.AreEqual(new[]
            {
                "",
                "Berend"
            }, persons.Select(p => p.Name).ToArray());
            Assert.AreEqual(new[]
            {
                0,
                11
            }, persons.Select(p => p.Age).ToArray());
        }

        [Test]
        public void DeleteSelectionOnReadOnlyTableViewDoesNothing()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };

            //select the top row
            tableView.SelectCells(0, 0, 0, 1);
            tableView.ReadOnly = true;

            //action! try to delete something
            tableView.DeleteCurrentSelection();

            //assert all is well
            Assert.AreEqual("Aaltje", persons[0].Name);
            Assert.AreEqual(2, persons.Count);
        }

        [Test]
        public void DeleteSelectionOnReadOnlyColumnDoesNothing()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };

            //select the top row
            tableView.SelectCells(0, 1, 0, 1);
            tableView.Columns[1].ReadOnly = true;

            //action! try to delete something
            tableView.DeleteCurrentSelection();

            //assert all is well
            Assert.AreEqual(12, persons[0].Age);
            Assert.AreEqual(2, persons.Count);
        }

        [Test]
        public void CanDeleteCurrentSelectionTest()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };
            tableView.CanDeleteCurrentSelection += () => false; // Cannot delete things

            // Select the cell for Aaltje's age
            tableView.SelectCells(0, 1, 0, 1);
            tableView.DeleteCurrentSelection();

            Assert.AreEqual(12, persons[0].Age, "Age should not be affected.");

            tableView.CanDeleteCurrentSelection += () => true;
            tableView.DeleteCurrentSelection();

            Assert.AreEqual(0, persons[0].Age, "Age should be cleared");
        }

        [Test]
        public void ColumnReadOnly()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };
            var tableView = new TableView
            {
                Data = persons
            };

            tableView.Columns[0].ReadOnly = true;
            Assert.IsTrue(tableView.Columns[0].ReadOnly);
        }

        [Test]
        public void AllowAddNewRowShouldBeDeducedFromBindingSource()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12
                },
                new Person
                {
                    Name = "Berend", Age = 11
                }
            };

            var tableView = new TableView
            {
                Data = new BindingList<Person>(persons)
                {
                    AllowNew = false
                }
            };
            //WindowsFormsTestHelper.ShowModal(tableView);
            Assert.IsFalse(tableView.AllowAddNewRow);
        }

        [Test]
        public void SetRowCellValueGeneratesCellChangedEvent()
        {
            var table = new DataTable();

            table.Columns.Add("column", typeof(string));
            table.Rows.Add("1");

            var tableView = new TableView
            {
                Data = table
            };

            int callCount = 0;
            tableView.CellChanged += (s, e) =>
            {
                callCount++;
                Assert.AreEqual(0, e.Value.Column.AbsoluteIndex);
                Assert.AreEqual(0, e.Value.RowIndex);
            };
            tableView.SetCellValue(0, 0, "2");
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void FormatStringsShouldWorkCorrectly()
        {
            var dateOfBirth = new DateTime(1980, 1, 1);
            var dateOfDeath = new DateTime(2060, 2, 3);
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12, DateOfBirth = dateOfBirth, DateOfDeath = dateOfDeath
                },
                new Person
                {
                    Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990, 1, 1), DateOfDeath = new DateTime(2061, 2, 3)
                }
            };

            var tableView = new TableView
            {
                ColumnAutoWidth = true, Data = persons
            };

            var dtFormat1 = "dd-MMM-yyyy";
            var dtFormat2 = "yyyy (MMM-dd)";

            tableView.GetColumnByName("Name").DisplayFormat = "Name = {0}";
            tableView.GetColumnByName("Age").DisplayFormat = "D3";
            tableView.GetColumnByName("DateOfBirth").DisplayFormat = dtFormat1;
            tableView.GetColumnByName("DateOfDeath").DisplayFormat = dtFormat2;

            tableView.GetColumnByName("Name").DisplayFormat.Should().Be.EqualTo("Name = {0}");

            tableView.GetCellDisplayText(0, 0).Should().Be.EqualTo("Name = Aaltje");
            tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("012");
            tableView.GetCellDisplayText(0, 2).Should().Be.EqualTo(dateOfBirth.ToString(dtFormat1));
            tableView.GetCellDisplayText(0, 3).Should().Be.EqualTo(dateOfDeath.ToString(dtFormat2));
        }

        [Test]
        public void FormatStringsShouldAlsoTakeCustomFormatter()
        {
            var dateOfBirth = new DateTime(1980, 1, 1);
            var dateOfDeath = new DateTime(2060, 2, 3);

            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12, DateOfBirth = dateOfBirth, DateOfDeath = dateOfDeath
                },
                new Person
                {
                    Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990, 1, 1), DateOfDeath = new DateTime(2061, 2, 3)
                }
            };

            var tableView = new TableView
            {
                ColumnAutoWidth = true, Data = persons
            };

            var dtFormat1 = "dd-MMM-yyyy";
            var dtFormatter = new TestDateTimeColumnFormatter();

            tableView.GetColumnByName("Name").DisplayFormat = "Name = {0}";
            tableView.GetColumnByName("Age").DisplayFormat = "D3";
            tableView.GetColumnByName("DateOfBirth").DisplayFormat = dtFormat1;
            tableView.GetColumnByName("DateOfDeath").CustomFormatter = dtFormatter;

            tableView.GetColumnByName("Name").DisplayFormat.Should().Be.EqualTo("Name = {0}");

            tableView.GetCellDisplayText(0, 0).Should().Be.EqualTo("Name = Aaltje");
            tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("012");
            tableView.GetCellDisplayText(0, 2).Should().Be.EqualTo(dateOfBirth.ToString(dtFormat1));
            tableView.GetCellDisplayText(0, 3).Should().Be.EqualTo(dtFormatter.Format("", dateOfDeath, null));
        }

        [Test]
        public void CustomFormattingShouldWorkCorrectly()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12, DateOfBirth = new DateTime(1980, 1, 1)
                },
                new Person
                {
                    Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990, 1, 1)
                }
            };

            var tableView = new TableView
            {
                ColumnAutoWidth = true, Data = persons
            };

            var nameColumn = tableView.GetColumnByName("Name");

            nameColumn.DisplayFormat = "Name = {0}";
            nameColumn.CustomFormatter = new NameTableCellFormatter();

            tableView.GetCellDisplayText(0, 0).Should().Be.EqualTo("Name with custom formatter : Aaltje");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Niet in staat om cellen te selecteren wanneer tableView RowSelect ingeschakeld is. Gebruik in plaats daarvan SelectRow.")]
        public void SelectCellsForTableViewWithRowSelectThrowsException()
        {
            //this test relates to issue 3069...demonstrating a problem paste lines when rowselect is enabled.
            //had to do with a  call to SelectCells for a table with RowSelect. Therefore tableView.gridView.GetSelectedCells() was no longer in synch with tablveView.SelectedCells
            //causing a error when pasting
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12, DateOfBirth = new DateTime(1980, 1, 1)
                },
                new Person
                {
                    Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990, 1, 1)
                }
            };

            var tableView = new TableView
            {
                ColumnAutoWidth = true, Data = persons, RowSelect = true
            };

            //should throw because tableview is in RowSelect modus.
            tableView.SelectCells(0, 0, 0, 1);
        }

        [Test]
        public void ClickingColumnHeaderWithRowSelectShouldNotThrow()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Name = "Aaltje", Age = 12, DateOfBirth = new DateTime(1980, 1, 1)
                },
                new Person
                {
                    Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990, 1, 1)
                }
            };

            var mock = new MockRepository();
            var gridView = mock.Stub<GridView>();
            var hitInfo = mock.Stub<GridHitInfo>();

            hitInfo.Column = null;
            Expect.Call(hitInfo.InColumn).Return(true);
            Expect.Call(hitInfo.InColumnPanel).Return(true);
            Expect.Call(gridView.CalcHitInfo(new Point(0, 0))).IgnoreArguments().Return(hitInfo).Repeat.Any();

            mock.ReplayAll();

            var tableView = new TableView
            {
                ColumnAutoWidth = true, Data = persons, RowSelect = true
            };

            TypeUtils.SetField(tableView, "dxGridView", gridView);
            TypeUtils.CallPrivateMethod(tableView, "DxGridViewClick", null, new EventArgs()); // Should not throw
            mock.VerifyAll();
        }

        [Test]
        public void TableViewAllowSortingColumnPropertyShouldBeAddedToNewColumns()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(int));

            for (int i = 0; i < 50; i++)
            {
                DataRow row = table.NewRow();
                row["A"] = i*10.0;
                row["B"] = i;
                table.Rows.Add(row);
            }

            var tableView = new TableView
            {
                Data = table, AllowColumnSorting = false
            };

            Assert.IsFalse(tableView.Columns[0].SortingAllowed);

            table.Columns.Add("C", typeof(int));

            Assert.IsFalse(tableView.Columns[2].SortingAllowed);

            tableView.AllowColumnSorting = true;

            Assert.IsTrue(tableView.Columns[2].SortingAllowed);

            table.Columns.Add("D", typeof(int));

            Assert.IsTrue(tableView.Columns[3].SortingAllowed);
        }

        [Test]
        public void TableViewCellSelectionShouldUpdateSelectedColumns()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(int));

            for (int i = 0; i < 5; i++)
            {
                DataRow row = table.NewRow();
                row["A"] = i*10.0;
                row["B"] = i;
                table.Rows.Add(row);
            }

            var tableView = new TableView
            {
                Data = table, AllowColumnSorting = false
            };
            tableView.Columns[0].Visible = false; //hide one column
            tableView.SelectCells(0, 0, 0, 0);

            Assert.AreEqual(1, tableView.SelectedColumnsIndices.Length);
            Assert.AreEqual(1, tableView.Columns[tableView.SelectedColumnsIndices[0]].AbsoluteIndex);
        }

        [Test]
        public void TestAddFocusedColumnInGridViewColumnFilterChanged()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(int));

            for (int i = 0; i < 5; i++)
            {
                var row = table.NewRow();
                row["A"] = i*10.0;
                row["B"] = i;
                table.Rows.Add(row);
            }

            var tableView = new TableView
            {
                Data = table, AllowColumnSorting = false
            };

            var filterChangedCount = 0;
            ITableViewColumn selectedColumn = null;

            tableView.ColumnFilterChanged += (s, e) =>
            {
                filterChangedCount++;
                selectedColumn = e.Value;
            };

            tableView.Columns[0].FilterString = "A = 50";

            Assert.AreEqual(1, filterChangedCount);
            Assert.AreEqual(tableView.Columns[0], selectedColumn);
        }

        [Test]
        public void GetRowObjectAtWithIndexOutOfRangeGivesNull()
        {
            var list = new List<string>
            {
                "AA", "BB", "CC"
            };
            var tableView = new TableView
            {
                Data = list
            };

            Assert.IsNull(tableView.GetRowObjectAt(-1));
            Assert.IsNull(tableView.GetRowObjectAt(10));
        }

        [Test]
        public void GetRowObjectAtWithSorting()
        {
            var list = new List<string>
            {
                "AA", "BB", "CC"
            };
            var tableView = new TableView
            {
                Data = list
            };

            tableView.Columns[0].SortOrder = SortOrder.Descending;
            Assert.AreEqual(list[0], tableView.GetRowObjectAt(2));
        }

        [Test]
        public void GetRowObjectAtWithoutSorting()
        {
            var list = new List<string>
            {
                "AA", "BB", "CC"
            };
            var tableView = new TableView
            {
                Data = list
            };

            Assert.AreEqual(list[2], tableView.GetRowObjectAt(2));
        }

        [Test]
        public void ClickingCheckboxColumnShouldChangeSelection()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(string));
            table.Columns.Add("B", typeof(bool));

            for (int i = 0; i < 5; i++)
            {
                var row = table.NewRow();
                row["A"] = "Option " + i;
                row["B"] = i%2 != 0;
                table.Rows.Add(row);
            }

            var tableView = new TableView
            {
                Data = table, MultiSelect = true, RowSelect = true, ReadOnly = true
            };
            var gridView = TypeUtils.GetField(tableView, "dxGridView");
            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                TypeUtils.CallPrivateMethod(gridView, "RaiseMouseDown", new MouseEventArgs(MouseButtons.Left, 1, 156, 76, 0));

                Assert.AreEqual(3, tableView.SelectedRowsIndices.First());
            });
        }

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            LogHelper.ConfigureLogging();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            LogHelper.ResetLogging();
        }

        private static DataTable CreateTableForCopyPaste()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(int));
            table.Columns.Add("B", typeof(int));

            for (int i = 0; i < 5; i++)
            {
                DataRow row = table.NewRow();
                row["A"] = i;
                row["B"] = i;
                table.Rows.Add(row);
            }
            return table;
        }

        public class ItemType
        {
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public class Item
        {
            public string Name { get; set; }
            public ItemType Type { get; set; }
        }

        private class AddBracketTestFormatter : ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                return String.Format("[{0}]", arg);
            }
        }

        private class AddDashTestFormatter : ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                return String.Format("-{0}-", arg);
            }
        }

        private class NameTableCellFormatter : ICustomFormatter
        {
            /// <summary>
            /// Converts the value of a specified object to an equivalent string representation using specified format and culture-specific formatting information.
            /// </summary>
            /// <returns>
            /// The string representation of the value of <paramref name="arg"/>, formatted as specified by <paramref name="format"/> and <paramref name="formatProvider"/>.
            /// </returns>
            /// <param name="format">A format string containing formatting specifications. 
            ///                 </param><param name="arg">An object to format. 
            ///                 </param><param name="formatProvider">An <see cref="T:System.IFormatProvider"/> object that supplies format information about the current instance. 
            ///                 </param><filterpriority>2</filterpriority>
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                return "Name with custom formatter : " + arg;
            }
        }
    }

    public class TestDateTimeColumnFormatter : ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            return "Test format";
        }
    }
}