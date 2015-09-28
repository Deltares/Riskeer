using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf.Editors;
using DelftTools.Controls.Swf.Table;
using DelftTools.Tests.Controls.Swf.Table.TestClasses;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;
using Category = NUnit.Framework.CategoryAttribute;

namespace DelftTools.Tests.Controls.Swf.Table
{
    [TestFixture]
    public class TableView2Test
    {
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

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void FilterIntegerValues()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Column");
            Enumerable.Range(1, 5).ForEach(i => dataTable.Rows.Add(i));

            var tableView = new TableView2
            {
                Data = dataTable,
                ReadOnly = true,
                AllowAddNewRow = false
            };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.Columns[0].FilterString = "Column > 2";
                    Assert.AreEqual(3, tableView.RowCount);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void TableViewShouldUpdateColumns()
        {
            var table = new DataTable("tablename");
            var view = new TableView2 { Data = table };

            WindowsFormsTestHelper.Show(view);

            var column = new DataColumn("testName", typeof(double)) { Caption = "testCaption" };
            table.Columns.Add(column);

            Assert.AreEqual(1, view.DataGridView.Columns.Count);
            Assert.AreEqual("testName", view.Columns[0].Name);
            Assert.AreEqual("testCaption", view.Columns[0].Caption);

            column.Caption = "newTestCaption";
            view.ResetBindings();

            Assert.AreEqual("newTestCaption", view.Columns[0].Caption);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void FilterDateTimeColumn()
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var tableView = new TableView2{AllowAddNewRow = false};
            var table = new DataTable();

            table.Columns.Add("A", typeof(DateTime));
            table.Columns.Add("B", typeof(double));

            var dateTime = DateTime.Now;
            var dateTimePlus4Days = dateTime.AddDays(4);

            for (int i = 0; i < 12; i++)
            {
                var row = table.NewRow();
                row["A"] = dateTime.AddDays(i);
                row["B"] = i; 

                table.Rows.Add(row);
            }
            
            tableView.Data = table;

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.Columns[0].FilterString = string.Format("A >= #{0}# And A < #{1}#", dateTime, dateTimePlus4Days);
                    Assert.AreEqual(4, tableView.RowCount);
                });
            
            Thread.CurrentThread.CurrentCulture = oldCulture;
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetTextToTableViewWithDateTimeColumnUsCulture()
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

            var table = new DataTable();
            table.Columns.Add("A", typeof(DateTime));
            table.Columns.Add("B", typeof(double));
            
            var row = table.NewRow();
            row["A"] = DateTime.Now;
            row["B"] = 0;
            
            table.Rows.Add(row);

            var tableView = new TableView2 {AllowAddNewRow = false, Data = table};

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.SetCellValue(0, 0, "2010/11/18 01:02:03");
                    var dateTime = DateTime.Parse(tableView.GetCellValue(0, 0).ToString());
                    Assert.AreEqual(new DateTime(2010, 11, 18, 01, 02, 03), dateTime);

                    tableView.SetCellValue(0, 1, 1234.5);
                    var value = double.Parse(tableView.GetCellValue(0, 1).ToString());

                    Assert.AreEqual(1234.5, value, "Expects uncustomized en-US number formatting; " +
                                                   "If this fails, please check if local number format for en-US culture was modified.");
                });

            Thread.CurrentThread.CurrentCulture = oldCulture;
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetTextToTableViewWithDateTimeColumnNlCulture()
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            var specificCulture = CultureInfo.CreateSpecificCulture("nl-NL");
            specificCulture.NumberFormat.NumberGroupSeparator = ".";
            specificCulture.NumberFormat.NumberDecimalSeparator = ",";

            Thread.CurrentThread.CurrentCulture = specificCulture;

            var table = new DataTable();

            table.Columns.Add("A", typeof(DateTime));
            table.Columns.Add("B", typeof(double));
            var row = table.NewRow();
            row["A"] = DateTime.Now;
            row["B"] = 0;
            table.Rows.Add(row);

            var tableView = new TableView2 {Data = table};
            
            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {

                    tableView.SetCellValue(0, 0, "2010/11/18 01:02:03");
                    var dateTime = DateTime.Parse(tableView.GetCellValue(0, 0).ToString());
                    Assert.AreEqual(new DateTime(2010, 11, 18, 01, 02, 03), dateTime);

                    tableView.SetCellValue(0, 1, 1234.5);

                    var value = double.Parse(tableView.GetCellValue(0, 1).ToString());

                    Assert.AreEqual(1234.5, value);
                });

            Thread.CurrentThread.CurrentCulture = oldCulture;
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void Init()
        {
            var tableView = new TableView2();

            var table = new DataTable();

            table.Columns.Add("A", typeof(DateTime));
            table.Columns.Add("B", typeof(bool));
            for (int i = 0; i < 50; i++)
            {
                DataRow row = table.NewRow();
                row["A"] = DateTime.Now;
                row["B"] = true;
                table.Rows.Add(row);
            }

            tableView.Data = table;
            WindowsFormsTestHelper.ShowModal(tableView);
        }

/*        [Test]
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void Copy1Paste1CellEnumValue()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(FruitType));

            var row = table.NewRow();
            row["A"] = FruitType.Appel;
            table.Rows.Add(row);
            row = table.NewRow();
            row["A"] = FruitType.Banaan;
            table.Rows.Add(row);

            var tableView = new TableView2 { Data = table };
            Assert.AreEqual("Een Appel", tableView.GetCellDisplayText(0, 0));
            Assert.AreEqual("Een Banaan", tableView.GetCellDisplayText(1, 0));

            Action<Form> onShown = delegate
            {
                tableView.Focus();
                tableView.SelectRow(1); //Select Banaan
                tableView.FocusedRowIndex = 1;
                SendKeys.SendWait("^c");
                tableView.SelectRow(0);
                tableView.FocusedRowIndex = 0;
                SendKeys.SendWait("^v");

                Assert.AreEqual("Een Banaan", tableView.GetCellDisplayText(0, 0));
                Assert.AreEqual("Een Banaan", tableView.GetCellDisplayText(1, 0));
            };
            WindowsFormsTestHelper.ShowModal(tableView, onShown);
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

            var tableView = new TableView2 { Data = table };

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

            var tableView = new TableView2 { Data = table };

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

            var tableView = new TableView2 { Data = table };
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
        [Category(TestCategory.WindowsForms)]
        public void DefaultCellEditor()
        {
            var items = new List<ClassWithTwoProperties>(new[]
                                                                {
                                                                    new ClassWithTwoProperties { Property1 = "11", Property2 = "12" },
                                                                    new ClassWithTwoProperties { Property1 = "21", Property2 = "22" },
                                                                    new ClassWithTwoProperties { Property1 = "31", Property2 = "32" },
                                                                });

            var tableView = new TableView2 { Data = new BindingList<ClassWithTwoProperties>(items) };

            WindowsFormsTestHelper.ShowModal(tableView);
        }
*/

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ComboBoxTypeEditor()
        {

            // types of the item, to be shown in the combo box
            var itemTypes = new[]
                                {
                                    new ItemType{ Name = "item type1"},
                                    new ItemType{ Name = "item type2"},
                                    new ItemType{ Name = "item type3"}
                                };

            // default items
            var items = new EventedList<Item>
                            {
                                new Item { Name = "item1", Type = itemTypes[0] },
                                new Item { Name = "item2", Type = itemTypes[1] }
                            };

            var tableView = new TableView2 { Data = new BindingList<Item>(items) };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    var comboBoxTypeEditor = new ComboBoxTypeEditor { Items = itemTypes };

                    tableView.Columns[1].Editor = comboBoxTypeEditor; // inject it under the 2nd column
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms), Category(TestCategory.WorkInProgress)]
        public void ComboBoxTypeEditorWithCustomFormatter_ShouldNotOverwriteColumnFormatting_Tools7594()
        {
            // types of the item, to be shown in the combo box
            var itemTypes = new[]
                                {
                                    new ItemType{ Name = "item type1"},
                                    new ItemType{ Name = "item type2"},
                                    new ItemType{ Name = "item type3"}
                                };

            // default items
            var items = new EventedList<Item>
                            {
                                new Item { Name = "item1", Type = itemTypes[0] },
                                new Item { Name = "item2", Type = itemTypes[1] }
                            };

            var tableView = new TableView2 { Data = new BindingList<Item>(items) };

            var comboBoxTypeEditor = new ComboBoxTypeEditor
            {
                CustomFormatter = new AddBracketTestFormatter(),
                Items = itemTypes
            };
            
            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.Columns[1].CustomFormatter = new AddDashTestFormatter();
                    tableView.Columns[1].Editor = comboBoxTypeEditor; // inject it under the 2nd column
                    tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("-item type1-");
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ComboBoxTypeEditorNonMandatoryItemsWithCustomFormatter_ShouldNotOverwriteColumnFormatting_Tools7594()
        {
            // types of the item, to be shown in the combo box
            var itemTypes = new[]
                                {
                                    new ItemType{ Name = "item type1"},
                                    new ItemType{ Name = "item type2"},
                                    new ItemType{ Name = "item type3"}
                                };

            // default items
            var items = new EventedList<Item>
                            {
                                new Item { Name = "item1", Type = itemTypes[0] },
                                new Item { Name = "item2", Type = itemTypes[1] }
                            };

            var tableView = new TableView2 { Data = new BindingList<Item>(items) };

            var comboBoxTypeEditor = new ComboBoxTypeEditor
            {
                CustomFormatter = new AddBracketTestFormatter(),
                Items = itemTypes,
                ItemsMandatory = false
            };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                tableView.Columns[1].CustomFormatter = new AddDashTestFormatter();
                tableView.Columns[1].Editor = comboBoxTypeEditor; // inject it under the 2nd column
                tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("-item type1-");
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ComboBoxTypeEditorNonMandatoryWithCustomFormatter_ShouldWork_Tools7594()
        {
            // types of the item, to be shown in the combo box
            var itemTypes = new[]
                                {
                                    new ItemType{ Name = "item type1"},
                                    new ItemType{ Name = "item type2"},
                                    new ItemType{ Name = "item type3"}
                                };

            // default items
            var items = new EventedList<Item>
                            {
                                new Item { Name = "item1", Type = itemTypes[0] },
                                new Item { Name = "item2", Type = itemTypes[1] }
                            };

            var tableView = new TableView2 { Data = new BindingList<Item>(items) };

            var comboBoxTypeEditor = new ComboBoxTypeEditor
            {
                CustomFormatter = new AddBracketTestFormatter(),
                Items = itemTypes,
                ItemsMandatory = false
            };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                tableView.Columns[1].CustomFormatter = new AddDashTestFormatter();
                tableView.Columns[1].Editor = comboBoxTypeEditor; // inject it under the 2nd column

                var comboBoxColumn = (DataGridViewComboBoxColumn)tableView.DataGridView.Columns[1];
                var dictionary = (List<KeyValuePair<string, object>>)comboBoxColumn.DataSource;

                Assert.AreEqual("[item type1]",dictionary[0].Key);
                tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("-item type1-");
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms), Category(TestCategory.WorkInProgress)]
        public void ComboBoxTypeEditorNonMandatoryItemsAndWithCustomFormatter_ShouldWork_Tools7594()
        {
            // types of the item, to be shown in the combo box
            var itemTypes = new[]
                                {
                                    new ItemType{ Name = "item type1"},
                                    new ItemType{ Name = "item type2"},
                                    new ItemType{ Name = "item type3"}
                                };

            // default items
            var items = new EventedList<Item>
                            {
                                new Item { Name = "item1", Type = itemTypes[0] },
                                new Item { Name = "item2", Type = itemTypes[1] }
                            };

            var tableView = new TableView2 { Data = new BindingList<Item>(items) };

            var comboBoxTypeEditor = new ComboBoxTypeEditor
            {
                CustomFormatter = new AddBracketTestFormatter(),
                Items = itemTypes
            };

            // Test (What should be visible in the GUI):
            // * Table view should show to entries using no custom formatters at all.
            // * Editor of 2nd column should show entries using 'AddBrackerTestFormatter'.
            // * Transitioning between editing (dropdown box visible) and non-editing (no dropdown box visible) should show only custom editor for combo box is used.
            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                tableView.Columns[1].Editor = comboBoxTypeEditor; // inject it under the 2nd column
                Assert.IsNull(tableView.Columns[1].CustomFormatter); // Injection should not have affected CustomFormatter on Column itself

                var comboBoxColumn = (DataGridViewComboBoxColumn)tableView.DataGridView.Columns[1];
                var dictionary = (List<KeyValuePair<string, object>>)comboBoxColumn.DataSource;

                Assert.AreEqual("[item type1]", dictionary[0].Key);
                tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("item type1");
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ComboBoxTypeEditorWithCustomFormatter_ShouldWork_Tools7594()
        {
            // types of the item, to be shown in the combo box
            var itemTypes = new[]
                                {
                                    new ItemType{ Name = "item type1"},
                                    new ItemType{ Name = "item type2"},
                                    new ItemType{ Name = "item type3"}
                                };

            // default items
            var items = new EventedList<Item>
                            {
                                new Item { Name = "item1", Type = itemTypes[0] },
                                new Item { Name = "item2", Type = itemTypes[1] }
                            };

            var tableView = new TableView2 { Data = new BindingList<Item>(items) };

            var comboBoxTypeEditor = new ComboBoxTypeEditor
            {
                CustomFormatter = new AddBracketTestFormatter(),
                Items = itemTypes,
                ItemsMandatory = false
            };

            // Test (What should be visible in the GUI):
            // * Table view should show to entries using no custom formatters at all.
            // * Editor of 2nd column should show entries using 'AddBrackerTestFormatter'.
            // * Transitioning between editing (dropdown box visible) and non-editing (no dropdown box visible) should show only custom editor for combo box is used.
            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                tableView.Columns[1].Editor = comboBoxTypeEditor; // inject it under the 2nd column
                Assert.IsNull(tableView.Columns[1].CustomFormatter); // Injection should not have affected CustomFormatter on Column itself
                
                var comboBoxColumn = (DataGridViewComboBoxColumn)tableView.DataGridView.Columns[1];
                var dictionary = (List<KeyValuePair<string, object>>)comboBoxColumn.DataSource;

                Assert.AreEqual("[item type1]", dictionary[0].Key);
                tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("item type1");
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SelectMultipleCellsUsingApi()
        {
            var table = new DataTable();
            table.Columns.Add("column1", typeof(string));
            table.Columns.Add("column2", typeof(string));
            table.Rows.Add(new object[] { "1", "2" });
            table.Rows.Add(new object[] { "3", "4" });

            var tableView = new TableView2 { Data = table };

            var selectionTableView = new TableView2 { Data = new BindingList<TableViewCell>(tableView.SelectedCells) };

            WindowsFormsTestHelper.Show(selectionTableView);

            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        public void DefaultSelectionIsEmpty()
        {
            var table = new DataTable();
            table.Columns.Add("column1", typeof(string));
            table.Columns.Add("column2", typeof(string));
            table.Rows.Add(new object[] { "1", "2" });
            table.Rows.Add(new object[] { "3", "4" });

            var tableView = new TableView2 { Data = table };

            tableView.SelectedCells
                .Should().Be.Empty();
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowWithCustomClass()
        {
            var view = new TableView2 { Data = new BindingList<Person>() };

            WindowsFormsTestHelper.ShowModal(view);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void FillOutCustomClass()
        {
            if (GuiTestHelper.IsBuildServer)
                return;

            var persons = new BindingList<Person>();
            var tableView = new TableView2 { Data = persons };

            Action<Form> onShown = delegate
            {
                tableView.Focus();
                SendKeys.SendWait("J"); // goto row 1 column 2
                SendKeys.SendWait("{RIGHT}"); // goto row 1 column 2
                SendKeys.SendWait("3"); // also select cell below
                SendKeys.SendWait("{DOWN}"); //commit cells

                Assert.AreEqual(2, persons.Count); // 1 row with data and 1 new row
            };

            WindowsFormsTestHelper.ShowModal(tableView, onShown);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetRowCellValue()
        {
            var persons = new BindingList<Person> { new Person { Age = 22, Name = "Jan" } };
            var tableView = new TableView2 { Data = persons };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    //action! change the top left cell
                    tableView.SetCellValue(0, 0, "Kees");

                    Assert.AreEqual("Kees", persons[0].Name);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetRowCellValueIsBasedOnVisibleIndexes()
        {
            var persons = new BindingList<Person> { new Person { Age = 22, Name = "Jan" } };
            var tableView = new TableView2 { Data = persons };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    //hide the first column
                    tableView.Columns[0].Visible = false;

                    //action! change the top left cell
                    tableView.SetCellValue(0, 0, "23");

                    Assert.AreEqual(23, persons[0].Age);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetRowCellValueForCustomColumnOrder()
        {
            var tableView = new TableView();
            var persons = new List<Person>
                                {
                                    new Person {Age = 12, Name = "Hoi"},
                                    new Person {Age = 11, Name = "keers"}
                                };
            tableView.Data = persons;

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    //set the age first and name second 
                    tableView.Columns[0].DisplayIndex = 1;
                    tableView.Columns[1].DisplayIndex = 0;

                    tableView.SetCellValue(0, 1, "jantje");
                    Assert.AreEqual("jantje", persons[0].Name);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetRowCellValueDoesNotWorkOnReadOnlyTableView()
        {
            var table = new DataTable();
            table.Columns.Add("readonlycolumn", typeof(string));

            table.Rows.Add(new object[] { "1" });

            var tableView = new TableView2 { Data = table };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                tableView.ReadOnly = true;

                tableView.SetCellValue(0, 0, "2");

                Assert.AreEqual("1", tableView.GetCellValue(0, 0));
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetRowCellValueDoesNotWorkOnReadOnlyColumns()
        {
            var table = new DataTable();
            table.Columns.Add("readonlycolumn", typeof(string));

            table.Rows.Add(new object[] { "1" });

            var tableView = new TableView2 { Data = table };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                //read only first column
                tableView.Columns[0].ReadOnly = true;

                tableView.SetCellValue(0, 0, "2");

                Assert.AreEqual("1", tableView.GetCellValue(0, 0));
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetRowCellValueDoesNotWorkOnReadOnlyCell()
        {
            var table = new DataTable();
            table.Columns.Add("readonlycolumn", typeof(string));

            table.Rows.Add(new object[] { "1" });

            var tableView = new TableView2 { Data = table };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                //read only cell
                tableView.ReadOnlyCellFilter += delegate { return true; };

                tableView.SetCellValue(0, 0, "2");

                Assert.AreEqual("1", tableView.GetCellValue(0, 0));
            });
        }

/*        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetRowCellValueOnInvalidCellDoesNotCommit()
        {
            var persons = new List<Person>
                                {
                                    new Person {Age = 12, Name = "Hoi"},
                                    new Person {Age = 11, Name = "keers"}
                                };

            var tableView = new TableView2
            {
                Data = persons,
                InputValidator = (tvc, obj) => new Utils.Tuple<string, bool>("first", false)
            };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                //try to set the name on the first column...should fail

                tableView.SetCellValue(0, 0, "oeps");

                //assert the new value did not get commited
                Assert.AreEqual("Hoi", persons[0].Name);
            });
        }*/

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void RowCount()
        {
            //empty tableview should have 0 rowcount
            var persons = new BindingList<Person>();
            var tableView = new TableView2 { AllowAddNewRow = false, Data = persons };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    Assert.AreEqual(0, tableView.RowCount);
                });

            //1 person one row right?
            persons = new BindingList<Person> { new Person { Age = 22, Name = "Jan" } };
            tableView = new TableView2 { AllowAddNewRow = false, Data = persons };

            WindowsFormsTestHelper.ShowModal(tableView, f => Assert.AreEqual(1, tableView.RowCount));
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SelectCells()
        {
            //empty tableview should have 0 rowcount
            var persons = new BindingList<Person>();
            for (int i = 0; i < 10; i++)
            {
                persons.Add(new Person());
            }
            var tableView = new TableView2 { Data = persons };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                //select cells
                tableView.SelectCells(5, 0, 9, 1);

                //check the bottom cells are all selected
                Assert.AreEqual(10, tableView.SelectedCells.Count);
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ChangeSelection()
        {
            //empty tableview should have 0 rowcount
            var persons = new BindingList<Person>();
            for (int i = 0; i < 10; i++)
            {
                persons.Add(new Person());
            }
            var tableView = new TableView2 { Data = persons };
            var selectionChangedCount = 0;
            var selectionChangedRowsCount = 0;

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                tableView.SelectionChanged += delegate(object sender, TableSelectionChangedEventArgs e)
                {
                    selectionChangedCount++;
                    selectionChangedRowsCount = e.Cells.Count;
                };

                //select cells
                tableView.SelectCells(5, 0, 9, 1);

                Assert.AreEqual(1, selectionChangedCount);
                Assert.AreEqual(10, selectionChangedRowsCount);
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SortTableViewWhenSettingSortOrder()
        {
            var person = new SortableAndFilterableBindingList<Person>(new []
                {
                    new Person {Age = 12, Name = "Aaltje"}, 
                    new Person {Age = 11, Name = "Berend"}
                });

            var tableView = new TableView2 { Data = person };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                //first sort descending..the first value should be berend
                tableView.Columns[0].SortOrder = SortOrder.Descending;
                Assert.AreEqual("Berend", tableView.GetCellValue(0, 0));

                //sort ascending..the first value should be aaltje
                tableView.Columns[0].SortOrder = SortOrder.Ascending;

                Assert.AreEqual("Aaltje", tableView.GetCellValue(0, 0));
            });
            
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowRowNumbers()
        {
            var person = new List<Person>
                                {new Person {Age = 12, Name = "Aaltje"}, 
                                new Person {Age = 11, Name = "Berend"}};

            for (var i = 0; i < 10; i++)
            {
                person.Add(new Person { Age = 11, Name = "Berend" });
            }

            var tableView = new TableView2
            {
                Data = person,
                ShowRowNumbers = true
            };

            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowUnboundColumns()
        {
            var person = new List<Person>
                                {new Person {Age = 12, Name = "Aaltje"}, 
                                new Person {Age = 11, Name = "Berend"}};

            for (var i = 0; i < 10; i++)
            {
                person.Add(new Person { Age = 11, Name = "Berend" });
            }

            var tableView = new TableView2
            {
                Data = person,
                ShowRowNumbers = true
            };

            var button = new ButtonTypeEditor { ButtonClickAction = () => MessageBox.Show("DoMessageBox") };

            var comboBoxEditor = new ComboBoxTypeEditor { Items = Enum.GetNames(typeof(FruitType)) };

            tableView.UnboundColumnData = (column, row, isGetData, isSetData, value) =>
            {
                if (column == 0)
                {
                    return column;
                }

                if (column == 2)
                {
                    return 0 == (row % 2) ? 0 : column;
                }

                if (column == 6)
                {
                    return 0 == (row % 2) ? FruitType.Peer.ToString() : FruitType.Banaan.ToString();
                }

                return null;
            };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    var doubleColumn = tableView.AddUnboundColumn("double", typeof(double), 0, null);
                    var buttonColumn = tableView.AddUnboundColumn("button", typeof(int), 2, button);
                    var comboColumn = tableView.AddUnboundColumn("combo", typeof(string), -1, comboBoxEditor);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void KleurtjesTable()
        {
            var kleurtjes = Enum.GetValues(typeof(KnownColor)).Cast<KnownColor>().Select(Color.FromKnownColor).ToList();

            var tableView = new TableView2
            {
                Data = kleurtjes,
                DisplayCellFilter = celStyle =>
                {
                    if ((celStyle.RowIndex >= 0) && (!celStyle.Selected))
                    {
                        celStyle.BackColor = kleurtjes[celStyle.RowIndex];
                        return true;
                    }
                    return false;
                }
            };

            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void FilterTableOnSettingFilterText()
        {
            var person = new SortableAndFilterableBindingList<Person>(new[]
                {
                    new Person {Age = 12, Name = "Aaltje"},
                    new Person {Age = 11, Name = "Berend"}
                });

            var tableView = new TableView2 { Data = person, AllowAddNewRow = false};

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    //action! set a filter
                    tableView.Columns[0].FilterString = "Name Like 'Jaap%'";

                    //no row should be visible
                    Assert.AreEqual(0, tableView.RowCount);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowTableViewWithListEnumType()
        {
            var tableView = new TableView
                {
                    Data = new BindingList<ClassWithEnum>
                        {
                            new ClassWithEnum { Type = FruitType.Appel }
                        }
                };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.Columns[0].Editor = new ComboBoxTypeEditor { Items = Enum.GetValues(typeof(FruitType)) };
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void DeleteSelectionRemovesValueIfCellWasSelect()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12 }, 
                                new Person {Name = "Berend",Age = 11 }};
            var tableView = new TableView2 { Data = persons };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    //select the twelve
                    tableView.SelectCells(0, 1, 0, 1);

                    //action! delete the row
                    tableView.DeleteCurrentSelection();

                    //assert we 'deleted' the age
                    Assert.AreEqual(0, persons[0].Age);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void DeleteSelectionRemovesRowIfAllCellsInARowAreSelected()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12}, 
                                new Person {Name = "Berend",Age = 11 }};
            var tableView = new TableView2 { Data = persons , AllowAddNewRow = false};
            
            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
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
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void RowDeleteHandlerTest()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12}, 
                                new Person {Name = "Berend",Age = 11 }};
            var tableView = new TableView2 { Data = persons };
            tableView.RowDeleteHandler += delegate
            {
                persons.RemoveAt(0);
                return true;
            };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.SelectRow(1); // Selection should not matter.
                    tableView.DeleteCurrentSelection();

                    Assert.AreEqual(1, persons.Count);
                    Assert.AreEqual("Berend", persons[0].Name);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void DeleteSelectionRemovesRowIfRowSelectedAndRowSelectIsEnabled()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12}, 
                                new Person {Name = "Berend",Age = 11 }};
            
            var tableView = new TableView2 {Data = persons, RowSelect = true};

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    //select the top row
                    tableView.SelectRow(0);

                    //action! delete the row
                    tableView.DeleteCurrentSelection();

                    //assert we only have berend now
                    Assert.AreEqual(new[] { "Berend" }, persons.Select(p => p.Name).ToArray());                    
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void DeleteSelectionTakesAllowDeleteRowsIntoAccount()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12 }, 
                                new Person {Name = "Berend",Age = 11 }};
            var tableView = new TableView2 { Data = persons };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                //select the top row
                tableView.SelectCells(0, 0, 0, 1);

                //action! delete the row
                tableView.AllowDeleteRow = false;
                tableView.DeleteCurrentSelection();

                //assert aaltje got 'reset'
                Assert.AreEqual(new[] { "", "Berend" }, persons.Select(p => p.Name).ToArray());
                Assert.AreEqual(new[] { 0, 11 }, persons.Select(p => p.Age).ToArray());
                    
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void DeleteSelectionOnReadOnlyTableViewDoesNothing()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12 }, 
                                new Person {Name = "Berend",Age = 11 }};
            var tableView = new TableView2 { Data = persons };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    //select the top row
                    tableView.SelectCells(0, 0, 0, 1);
                    tableView.ReadOnly = true;

                    //action! try to delete something
                    tableView.DeleteCurrentSelection();

                    //assert all is well
                    Assert.AreEqual("Aaltje", persons[0].Name);
                    Assert.AreEqual(2, persons.Count);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void DeleteSelectionOnReadOnlyColumnDoesNothing()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12 }, 
                                new Person {Name = "Berend",Age = 11 }};
            var tableView = new TableView2 { Data = persons };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                //select the top row
                tableView.SelectCells(0, 1, 0, 1);
                tableView.Columns[1].ReadOnly = true;

                //action! try to delete something
                tableView.DeleteCurrentSelection();

                //assert all is well
                Assert.AreEqual(12, persons[0].Age);
                Assert.AreEqual(2, persons.Count);
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void CanDeleteCurrentSelectionTest()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12 }, 
                                new Person {Name = "Berend",Age = 11 }};
            var tableView = new TableView2 { Data = persons };
            tableView.CanDeleteCurrentSelection += () => false; // Cannot delete things

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                // Select the cell for Aaltje's age
                tableView.SelectCells(0, 1, 0, 1);
                tableView.DeleteCurrentSelection();

                Assert.AreEqual(12, persons[0].Age, "Age should not be affected.");

                tableView.CanDeleteCurrentSelection += () => true;
                tableView.DeleteCurrentSelection();

                Assert.AreEqual(0, persons[0].Age, "Age should be cleared");
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowTableViewWithoutDeleteRecordButton()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12 }, 
                                new Person {Name = "Berend",Age = 11 }};
            
            var tableView = new TableView2 {Data = persons, AllowDeleteRow = false};
            
            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ColumnReadOnly()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12 }, 
                                new Person {Name = "Berend",Age = 11 }};
            var tableView = new TableView2 { Data = persons };
            
            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.Columns[0].ReadOnly = true;
                    Assert.IsTrue(tableView.Columns[0].ReadOnly);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ColumnReadOnlyShow()
        {

            var persons = new List<Person>();
            for (int i = 0; i < 10; i++)
            {
                persons.Add(new Person { Name = string.Format("user{0}", i), Age = 10 + 1 });
            }
            var tableView = new TableView2
            {
                Data = persons,
                //use some crazy colors to demonstrate this works
                ReadOnlyCellBackColor = Color.Pink, 
                ReadOnlyCellForeColor = Color.Blue
            };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.Columns[0].ReadOnly = true;
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void CellReadOnlyShow()
        {
            var persons = new List<Person>();
            for (int i = 0; i < 10; i++)
            {
                persons.Add(new Person { Name = string.Format("user{0}", i), Age = 10 + 1 });
            }
            var tableView = new TableView2
            {
                Data = persons,
                //use some crazy colors to demonstrate this works
                ReadOnlyCellBackColor = Color.Pink,
                ReadOnlyCellForeColor = Color.Blue
            };

            tableView.ReadOnlyCellFilter += cell => (0 != (cell.RowIndex % 2));
            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void TableReadOnlyShow()
        {
            var persons = new List<Person>();
            for (int i = 0; i < 10; i++)
            {
                persons.Add(new Person
                    {
                        Name = string.Format("user{0}", i), 
                        Age = 10 + i,
                        DateOfBirth = DateTime.Now.AddYears(i),
                        DateOfDeath = DateTime.Now.AddYears(i),
                    });
            }
            var tableView = new TableView2
            {
                Data = persons,
                ReadOnly = true,
                ReadOnlyCellBackColor = Color.Pink,
                ReadOnlyCellForeColor = Color.Blue
            };
            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void AllowAddNewRow()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12 }, 
                                new Person {Name = "Berend",Age = 11 }};

            var tableView = new TableView2 { Data = new BindingList<Person>(persons) { AllowNew = true }, AllowAddNewRow = true };

            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void AllowAddNewRowShouldBeDeducedFromBindingSource()
        {
            var persons = new List<Person>
                                {new Person {Name = "Aaltje",Age = 12 }, 
                                new Person {Name = "Berend",Age = 11 }};

            var tableView = new TableView2 { Data = new BindingList<Person>(persons) { AllowNew = false } };
            
            Assert.IsFalse(tableView.AllowAddNewRow);
        }

 /*       [Test]
        [Category(TestCategory.WindowsForms)]
        public void TableWithFunctionMultipleRowEditCommitsAllRows()
        {
            IFunction function = new Function
            {
                Arguments = { new Variable<int>("x") },
                Components = { new Variable<int>("f1") }
            };

            function[1] = new[] { 1 };
            function[2] = new[] { 2 };
            function[3] = new[] { 3 };

            var tableView = new TableView2 { Data = new FunctionBindingList(function) };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                tableView.Focus();
                tableView.SelectCells(0, 1, 2, 1, true);
                tableView.SetFocus(2, 1);

                SendKeys.SendWait("9");
                SendKeys.SendWait("^{ENTER}");

                Assert.AreEqual(9, function[1], "1");
                Assert.AreEqual(9, function[2], "2");
                Assert.AreEqual(9, function[3], "3");
            });
        }*/

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void SetRowCellValueGeneratesCellChangedEvent()
        {
            var table = new DataTable();

            table.Columns.Add("column", typeof(string));
            table.Rows.Add(new object[] { "1" });

            var tableView = new TableView2 { Data = table };
            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    int callCount = 0;
                    tableView.CellChanged += (s, e) =>
                        {
                            callCount++;
                            Assert.AreEqual(0, e.Value.Column.AbsoluteIndex);
                            Assert.AreEqual(0, e.Value.RowIndex);
                        };
                    tableView.SetCellValue(0, 0, "2");
                    Assert.AreEqual(1, callCount);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void TableViewAllowsCreatingColumnsManually()
        {
            var persons = new List<Person>
                                {
                                    new Person {Name = "Aaltje", Age = 12, DateOfBirth = DateTime.Now},
                                    new Person {Name = "Berend", Age = 11, DateOfBirth = DateTime.Now}
                                };

            var tableView = new TableView2 { AutoGenerateColumns = false, Data = persons };
            
            tableView.AddColumn("Name", "Naam");
            tableView.AddColumn("DateOfBirth", "Verjaardag");

            WindowsFormsTestHelper.ShowModal(tableView);
        }


/*        [Test]
        [Category(TestCategory.Performance)]
        public void RefreshShouldHappenFastWhenFunctionDataSourceHasManyChanges()
        {
            IFunction function = new Function
            {
                Arguments = { new Variable<int>("x") },
                Components = { new Variable<int>("f") }
            };

            var values = new int[1000];
            for (var i = 0; i < values.Length; i++)
            {
                values[i] = i;
            }

            // create function binding list before to exclude it when measuring time
            var functionBindingList = new FunctionBindingList(function);

            var tableView = new TableView2 { /*Data = functionBindingList#1# };
            
            // now do the same when table view is shown
            Action<Form> onShown = delegate
                {
                    //1000 ms is not really fast but better than no check at all
                    TestHelper.AssertIsFasterThan(1000, () =>
                        {
                            function.SetValues(values, new VariableValueFilter<int>(function.Arguments[0], values));
                        });
            };

            WindowsFormsTestHelper.ShowModal(tableView, onShown);
        }*/

        [Test, Category(TestCategory.WindowsForms)]// See also: TOOLS-6573
        public void FormatStringsShouldWorkCorrectly()
        {
            var dateOfBirth = new DateTime(1980, 1, 1);
            var dateOfDeath = new DateTime(2060, 2, 3);
            var persons = new List<Person>
                                {
                                    new Person {Name = "Aaltje", Age = 12, DateOfBirth = dateOfBirth, DateOfDeath = dateOfDeath},
                                    new Person {Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990,1,1), DateOfDeath = new DateTime(2061,2,3)}
                                };

            var tableView = new TableView2 { ColumnAutoWidth = true, Data = persons };

            var dtFormat1 = "dd-MMM-yyyy";
            var dtFormat2 = "yyyy (MMM-dd)";
            
            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.GetColumnByName("Name").DisplayFormat = "Name = {0}";
                    tableView.GetColumnByName("Age").DisplayFormat = "D3";
                    tableView.GetColumnByName("DateOfBirth").DisplayFormat = dtFormat1;
                    tableView.GetColumnByName("DateOfDeath").DisplayFormat = dtFormat2;
                    tableView.GetColumnByName("Name").DisplayFormat.Should().Be.EqualTo("Name = {0}");

                    tableView.GetCellDisplayText(0, 0).Should().Be.EqualTo("Name = Aaltje");
                    tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("012");
                    tableView.GetCellDisplayText(0, 2).Should().Be.EqualTo(dateOfBirth.ToString(dtFormat1));
                    tableView.GetCellDisplayText(0, 3).Should().Be.EqualTo(dateOfDeath.ToString(dtFormat2));
                });
        }

        [Test, Category(TestCategory.WindowsForms)] // See also: TOOLS-6573
        public void FormatStringsShouldAlsoTakeCustomFormatter()
        {
            var dateOfBirth = new DateTime(1980, 1, 1);
            var dateOfDeath = new DateTime(2060, 2, 3);

            var persons = new List<Person>
                                {
                                    new Person {Name = "Aaltje", Age = 12, DateOfBirth = dateOfBirth, DateOfDeath = dateOfDeath},
                                    new Person {Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990,1,1), DateOfDeath = new DateTime(2061,2,3)}
                                };

            var tableView = new TableView2 { ColumnAutoWidth = true, Data = persons };

            var dtFormat1 = "dd-MMM-yyyy";
            var dtFormatter = new TestDateTimeColumnFormatter();

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.GetColumnByName("Name").DisplayFormat = "Name = {0}";
                    tableView.GetColumnByName("Age").DisplayFormat = "D3";
                    tableView.GetColumnByName("DateOfBirth").DisplayFormat = dtFormat1;
                    tableView.GetColumnByName("DateOfDeath").CustomFormatter = dtFormatter;

                    tableView.GetColumnByName("Name").DisplayFormat.Should().Be.EqualTo("Name = {0}");

                    tableView.GetCellDisplayText(0, 0).Should().Be.EqualTo("Name = Aaltje");
                    tableView.GetCellDisplayText(0, 1).Should().Be.EqualTo("012");
                    tableView.GetCellDisplayText(0, 2).Should().Be.EqualTo(dateOfBirth.ToString(dtFormat1));
                    tableView.GetCellDisplayText(0, 3).Should().Be.EqualTo(dtFormatter.Format("", dateOfDeath, null));

                });
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void CustomFormattingShouldWorkCorrectly()
        {

            var persons = new List<Person>
                                {
                                    new Person {Name = "Aaltje", Age = 12, DateOfBirth = new DateTime(1980,1,1)},
                                    new Person {Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990,1,1)}
                                };

            var tableView = new TableView2 { ColumnAutoWidth = true, Data = persons };
            
            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    var nameColumn = tableView.GetColumnByName("Name");

                    nameColumn.DisplayFormat = "Name = {0}";
                    nameColumn.CustomFormatter = new NameTableCellFormatter();

                    tableView.GetCellDisplayText(0, 0).Should().Be.EqualTo("Name with custom formatter : Aaltje");
                });
        }
        
        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Unable to select cells when tableView has RowSelect enabled. Use SelectRow instead.")]
        public void SelectCellsForTableViewWithRowSelectThrowsException()
        {
            //this test relates to issue 3069...demonstrating a problem paste lines when rowselect is enabled.
            //had to do with a  call to SelectCells for a table with RowSelect. Therefore tableView.gridView.GetSelectedCells() was no longer in synch with tablveView.SelectedCells
            //causing a error when pasting
            var persons = new List<Person>
                                {
                                    new Person {Name = "Aaltje", Age = 12, DateOfBirth = new DateTime(1980,1,1)},
                                    new Person {Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990,1,1)}
                                };

            var tableView = new TableView2 { ColumnAutoWidth = true, Data = persons, RowSelect = true };

            //should throw because tableview is in RowSelect modus.
            tableView.SelectCells(0, 0, 0, 1);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowWithRowSelect()
        {
            var persons = new List<Person>
                                {
                                    new Person {Name = "Aaltje", Age = 12, DateOfBirth = new DateTime(1980,1,1)},
                                    new Person {Name = "Berend", Age = 11, DateOfBirth = new DateTime(1990,1,1)}
                                };

            var tableView = new TableView2 { ColumnAutoWidth = true, Data = persons, RowSelect = true };

            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowWithCheckbox()
        {
            var bools = new List<ClassWithBool>
                                {
                                    new ClassWithBool{Enabled= true},
                                    new ClassWithBool{Enabled= false}
                                };

            var tableView = new TableView2 { Data = bools };

            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowGridViewAllowSortingColumnsIsFalseShouldSelectColumnOnHeadClick()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(int));

            for (int i = 0; i < 50; i++)
            {
                DataRow row = table.NewRow();
                row["A"] = i * 10.0;
                row["B"] = i;
                table.Rows.Add(row);
            }

            var tableView = new TableView2 { Data = table, AllowColumnSorting = false };
            
            WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void TableViewAllowSortingColumnPropertyShouldBeAddedToNewColumns()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(int));

            for (int i = 0; i < 50; i++)
            {
                DataRow row = table.NewRow();
                row["A"] = i * 10.0;
                row["B"] = i;
                table.Rows.Add(row);
            }

            var tableView = new TableView2 { Data = table, AllowColumnSorting = false };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    Assert.IsFalse(tableView.Columns[0].SortingAllowed);

                    table.Columns.Add("C", typeof(int));

                    Assert.IsFalse(tableView.Columns[2].SortingAllowed);

                    tableView.AllowColumnSorting = true;

                    Assert.IsTrue(tableView.Columns[2].SortingAllowed);

                    table.Columns.Add("D", typeof(int));

                    Assert.IsTrue(tableView.Columns[3].SortingAllowed);
                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)] //TOOLS-6647
        [Category(TestCategory.WorkInProgress)] //hangs on buildserver :-(
        public void TableViewCursorKeysWhileInCellEditModeShouldWorkLikeExcel()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(int));

            for (int i = 0; i < 5; i++)
            {
                DataRow row = table.NewRow();
                row["A"] = i * 10.0;
                row["B"] = i;
                table.Rows.Add(row);
            }

            var tableView = new TableView2 { Data = table, AllowColumnSorting = false };

            WindowsFormsTestHelper.ShowModal(tableView,
                                                f =>
                                                {
                                                    tableView.Focus();

                                                    var cell = tableView.DataGridView.CurrentCell;

                                                    Assert.AreEqual(0, cell.RowIndex);
                                                    Assert.AreEqual(0, cell.ColumnIndex);
                                                    SendKeys.SendWait("{F2}"); //enter cell edit modus
                                                    SendKeys.SendWait("{LEFT}"); //cursor to right: we want to stay in the same cell!
                                                    SendKeys.SendWait("{RIGHT}"); //cursor to right: we want to stay in the same cell!

                                                    cell = tableView.DataGridView.CurrentCell;
                                                    Assert.AreEqual(0, cell.RowIndex);
                                                    Assert.AreEqual(0, cell.ColumnIndex);
                                                });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void TableViewCellSelectionShouldUpdateSelectedColumns()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(int));

            for (int i = 0; i < 5; i++)
            {
                DataRow row = table.NewRow();
                row["A"] = i * 10.0;
                row["B"] = i;
                table.Rows.Add(row);
            }

            var tableView = new TableView2 { Data = table, AllowColumnSorting = false };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
            {
                tableView.Columns[0].Visible = false; //hide one column
                tableView.SelectCells(0, 0, 0, 0);

                Assert.AreEqual(1, tableView.SelectedColumnsIndices.Length);
                Assert.AreEqual(1, tableView.Columns[tableView.SelectedColumnsIndices[0]].AbsoluteIndex);
            });
        }

        [Test]
        [Category(TestCategory.WindowsForms)]
        public void TestAddFocusedColumnInGridViewColumnFilterChanged()
        {
            var table = new DataTable();

            table.Columns.Add("A", typeof(double));
            table.Columns.Add("B", typeof(int));

            for (int i = 0; i < 6; i++)
            {
                var row = table.NewRow();
                row["A"] = i * 10.0;
                row["B"] = i;
                table.Rows.Add(row);
            }

            var tableView = new TableView2 { Data = table, AllowColumnSorting = false };

            var filterChangedCount = 0;
            ITableViewColumn selectedColumn = null;

            tableView.ColumnFilterChanged += (s, e) =>
            {
                filterChangedCount++;
                selectedColumn = e.Value;
            };

            WindowsFormsTestHelper.ShowModal(tableView, f =>
                {
                    tableView.Columns[0].FilterString = "A = 50";

                    Assert.AreEqual(1, filterChangedCount);
                    Assert.AreEqual(tableView.Columns[0].Name, selectedColumn.Name);
                });

        }

        private class ItemType : IComparable
        {
            public string Name { get; set; }
            public override string ToString() { return Name; }

            public int CompareTo(object other)
            {
                if (!(other is ItemType))
                {
                    return 1;
                }

                if (Name == null)
                {
                    return -1;
                }

                return Name.CompareTo((string) ((ItemType)other).Name);
            }
        }

        private class Item
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

        private class TestDateTimeColumnFormatter2 : ICustomFormatter
        {
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                return "Test format";
            }
        }
    }
}
