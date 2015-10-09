using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf.Editors;
using DelftTools.Controls.Swf.Table;
using DelftTools.Tests.Controls.Swf.Table.TestClasses;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.Table
{
    [TestFixture]
    public class TableViewPasteControllerTest
    {
        [Test]
        public void PasteFailsOnSortedColumn()
        {
            TableView tableView = GetTableViewWithTwoStringColumnsAndOneRow();

            //hook up a copy paste controller
            var tableViewCopyPasteController = new TableViewPasteController(tableView);

            //sort on the second column
            tableView.Columns[1].SortOrder = SortOrder.Ascending;

            //setup eventhandler for failed paste checking error message
            int callCount = 0;
            tableViewCopyPasteController.PasteFailed += (s, e) =>
            {
                Assert.AreEqual("Cannot paste into sorted column",
                                e.Value);
                callCount++;
            };
            //action! paste in the first column..second column is hit so should fail
            tableView.SelectCells(0, 0, 0, 0);
            tableViewCopyPasteController.PasteLines(new[]
            {
                "kaas\tmelk"
            });

            //we should have failed
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void PasteFailsInFilterGrid()
        {
            TableView tableView = GetTableViewWithTwoStringColumnsAndOneRow();

            //hook up a copy paste controller
            var tableViewCopyPasteController = new TableViewPasteController(tableView);

            //filter first column
            tableView.Columns[0].FilterString = "[Name] Like 'Jaap%'";

            //setup eventhandler for failed paste checking error message
            int callCount = 0;
            tableViewCopyPasteController.PasteFailed += (s, e) =>
            {
                Assert.AreEqual("Cannot paste into filtered tableview.",
                                e.Value);
                callCount++;
            };

            //action! paste in the first column..
            tableView.SelectCells(0, 0, 0, 0);
            tableViewCopyPasteController.PasteLines(new[]
            {
                "kaas"
            });

            //we should have failed
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void PasteDoesNotAddNewRowsToSortedTableView()
        {
            TableView tableView = GetTableViewWithTwoStringColumnsAndOneRow();

            //hook up a copy paste controller
            var tableViewCopyPasteController = new TableViewPasteController(tableView);

            //select the first cell of the second column
            tableView.SelectCells(0, 1, 0, 1);
            //sort fist column
            tableView.Columns[0].SortOrder = SortOrder.Ascending;

            //action paste a possible 3 lines
            tableViewCopyPasteController.PasteLines(new[]
            {
                "kaas\tmelk",
                "noot\tmies",
                "appel\tflap"
            });

            //assert rowcount is still one..no new rows are added on a sorted grid
            Assert.AreEqual(1, tableView.RowCount);
            //make sure we did something
            Assert.AreEqual("kaas", tableView.GetCellValue(0, 1));
        }

        [Test]
        public void PasteDataWithEmptyLinesIsMaintained()
        {
            var list = new BindingList<Person>()
            {
                new Person
                {
                    Name = "jan", Age = 25
                },
                new Person
                {
                    Name = "fon", Age = 33
                },
                new Person
                {
                    Name = "peer", Age = 25
                },
                new Person
                {
                    Name = "fo", Age = 33
                }
            };

            var tableView = new TableView
            {
                Data = list
            };

            //select all cells in first column 
            tableView.SelectCells(0, 0, 3, 0);

            //hook up a copy paste controller
            var tableViewCopyPasteController = new TableViewPasteController(tableView);
            //this should set the first column to kees,anton,kees,anton
            Clipboard.SetText("kees\r\nanton\r\n\r\npeer");

            tableViewCopyPasteController.PasteClipboardContents();
            //asert the names are now like this
            Assert.AreEqual(new[]
            {
                "kees",
                "anton",
                "",
                "peer"
            }, list.Select(p => p.Name).ToArray());
        }

        [Test]
        public void PasteFillsSelection()
        {
            var list = new BindingList<Person>()
            {
                new Person
                {
                    Name = "jan", Age = 25
                },
                new Person
                {
                    Name = "fon", Age = 33
                },
                new Person
                {
                    Name = "peer", Age = 25
                },
                new Person
                {
                    Name = "fo", Age = 33
                }
            };

            var tableView = new TableView
            {
                Data = list
            };

            //select all cells in first column 
            tableView.SelectCells(0, 0, 3, 0);

            //hook up a copy paste controller
            var tableViewCopyPasteController = new TableViewPasteController(tableView);
            //this should set the first column to kees,anton,kees,anton
            tableViewCopyPasteController.PasteLines(new[]
            {
                "kees",
                "anton"
            });

            //asert the names are now like this
            Assert.AreEqual(new[]
            {
                "kees",
                "anton",
                "kees",
                "anton"
            }, list.Select(p => p.Name).ToArray());
        }

        [Test]
        public void PasteFailsIntoNonSquareSelection()
        {
            var list = new BindingList<Person>()
            {
                new Person
                {
                    Name = "jan", Age = 25
                },
                new Person
                {
                    Name = "fon", Age = 33
                },
                new Person
                {
                    Name = "peer", Age = 25
                },
                new Person
                {
                    Name = "fo", Age = 33
                }
            };

            var tableView = new TableView
            {
                Data = list
            };
            //make a diagonal selection
            tableView.SelectedCells.Add(new TableViewCell(0, tableView.Columns.FirstOrDefault(c => c.DisplayIndex == 0)));
            tableView.SelectedCells.Add(new TableViewCell(1, tableView.Columns.FirstOrDefault(c => c.DisplayIndex == 1)));

            var tableViewCopyPasteController = new TableViewPasteController(tableView);
            int callCount = 0;
            tableViewCopyPasteController.PasteFailed += (s, e) =>
            {
                callCount++;
                Assert.AreEqual(
                    "Cannot paste into non rectangular selection",
                    e.Value);
            };
            //this should a paste failed event
            tableViewCopyPasteController.PasteLines(new[]
            {
                "kees",
                "anton"
            });

            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void CopyPasteEnumInBindingList()
        {
            var list = new BindingList<ClassWithEnum>
            {
                new ClassWithEnum
                {
                    Type = FruitType.Banaan
                },
                new ClassWithEnum
                {
                    Type = FruitType.Peer
                }
            };
            var tableView = new TableView
            {
                Data = list
            };

            var editor = new ComboBoxTypeEditor
            {
                Items = Enum.GetValues(typeof(FruitType))
            };
            tableView.Columns[0].Editor = editor;

            var tableViewCopyPasteController = new TableViewPasteController(tableView);
            //select 2nd row
            tableView.SelectCells(1, 0, 1, 0);

            //paste twee bananen en een peer ;)
            tableViewCopyPasteController.PasteLines(new[]
            {
                "Banaan",
                "Banaan",
                "Peer"
            });

            //we should have 3 bananas and a pear
            Assert.AreEqual("Banaan", "Banaan", "Banaan", "Peer", list.Select(c => c.Type).ToArray());
            //WindowsFormsTestHelper.ShowModal(tableView);
        }

        [Test]
        public void PasteTakesInvisibleColumnsIntoAccount()
        {
            //if a possible paste is too big it is important to not exceed the visiblecolcount - 1
            var list = new BindingList<Person>()
            {
                new Person
                {
                    Name = "jan", Age = 25
                },
                new Person
                {
                    Name = "fon", Age = 33
                },
                new Person
                {
                    Name = "peer", Age = 25
                },
                new Person
                {
                    Name = "fo", Age = 33
                }
            };

            var tableView = new TableView
            {
                Data = list
            };
            //hide the age column
            tableView.Columns[1].Visible = false;
            //select top left
            tableView.SelectCells(0, 0, 0, 0);
            var tableViewCopyPasteController = new TableViewPasteController(tableView);

            //paste some non sense
            tableViewCopyPasteController.PasteLines(new[]
            {
                "kees\tkan\twel"
            });

            //first person should be kees now
            Assert.AreEqual("kees", list[0].Name);
        }

        [Test]
        public void PasteIntoGridBoundToDataTable()
        {
            var tableWithTwoStrings = new DataTable();
            tableWithTwoStrings.Columns.Add("A", typeof(string));
            tableWithTwoStrings.Columns.Add("B", typeof(string));
            tableWithTwoStrings.Rows.Add("a", "b");

            var tableView = new TableView
            {
                Data = tableWithTwoStrings
            };

            Clipboard.SetText(string.Format("{0}\t{1}" + Environment.NewLine + "{0}\t{1}", "oe", "oe1"));

            tableView.PasteClipboardContents();

            //WindowsFormsTestHelper.ShowModal(tableView);

            //should overwrite existing row and add another one
            Assert.AreEqual(2, tableWithTwoStrings.Rows.Count);
            Assert.AreEqual("oe", tableWithTwoStrings.Rows[0][0]);
            Assert.AreEqual("oe1", tableWithTwoStrings.Rows[0][1]);
            Assert.AreEqual("oe", tableWithTwoStrings.Rows[1][0]);
            Assert.AreEqual("oe1", tableWithTwoStrings.Rows[1][1]);
        }

        [Test]
        public void PastingIntoReadOnlyCellsDoesNotChangeTheValues()
        {
            var tableWithTwoStrings = new DataTable();
            tableWithTwoStrings.Columns.Add("A", typeof(string));
            tableWithTwoStrings.Columns.Add("B", typeof(string));
            tableWithTwoStrings.Rows.Add("a", "b");

            var tableView = new TableView
            {
                Data = tableWithTwoStrings
            };
            //values in the first column are read only
            tableView.ReadOnlyCellFilter = delegate(TableViewCell cell) { return cell.Column.AbsoluteIndex == 0; };

            //select top row
            tableView.SelectCells(0, 0, 0, 1);

            var tableViewCopyPasteController = new TableViewPasteController(tableView);

            //paste some non sense. 
            tableViewCopyPasteController.PasteLines(new[]
            {
                "c\td"
            });

            //only column b should have changed
            Assert.AreEqual("a", tableWithTwoStrings.Rows[0][0]);
            Assert.AreEqual("d", tableWithTwoStrings.Rows[0][1]);
        }

        [Test]
        public void PasteIntoARowSelection()
        {
            var tableWithTwoStrings = new DataTable();
            tableWithTwoStrings.Columns.Add("A", typeof(string));
            tableWithTwoStrings.Columns.Add("B", typeof(string));
            tableWithTwoStrings.Rows.Add("a", "b");

            var tableView = new TableView
            {
                Data = tableWithTwoStrings
            };
            var tableViewCopyPasteController = new TableViewPasteController(tableView);

            //select top row
            tableView.SelectCells(0, 0, 0, 1);

            //paste some non sense. 
            tableViewCopyPasteController.PasteLines(new[]
            {
                "c"
            });

            //first line should now be [c c]
            Assert.AreEqual("c", tableWithTwoStrings.Rows[0][0]);
            Assert.AreEqual("c", tableWithTwoStrings.Rows[0][1]);
        }

        [Test]
        public void UseValidatorToIgnorePaste()
        {
            var tableView = new TableView();
            var tableWithTwoStrings = new DataTable();
            tableWithTwoStrings.Columns.Add("Name", typeof(string));
            tableWithTwoStrings.Columns.Add("B", typeof(string));
            tableWithTwoStrings.Rows.Add("a", "b");
            tableView.Data = tableWithTwoStrings;

            tableView.InputValidator += TableViewEditorValidator;

            var tableViewCopyPasteController = new TableViewPasteController(tableView);
            Assert.AreEqual("b", tableWithTwoStrings.Rows[0][1]);

            //select top right cell
            tableView.SelectCells(0, 1, 0, 1);

            // paste "c" and check result
            tableViewCopyPasteController.PasteLines(new[]
            {
                "c"
            });
            Assert.AreEqual("c", tableWithTwoStrings.Rows[0][1]);

            // check paste of d fails
            tableViewCopyPasteController.PasteLines(new[]
            {
                "d"
            });
            Assert.AreEqual("c", tableWithTwoStrings.Rows[0][1]);
        }

        [Test]
        public void UseValidatorToIgnoreRow()
        {
            var tableView = new TableView();
            var dataTable = new DataTable();
            dataTable.Columns.Add("Alpha", typeof(string));
            dataTable.Columns.Add("Numeric", typeof(int));
            dataTable.Rows.Add("a", 1);
            tableView.Data = dataTable;

            tableView.PasteController.PasteBehaviour = TableViewPasteBehaviourOptions.SkipRowWhenValueIsInvalid;
            tableView.InputValidator = (c, v) =>
            {
                if (c.Column.AbsoluteIndex == 0 && (v.ToString() == "b"))
                {
                    return new Utils.Tuple<string, bool>("b not allowed", false);
                }

                return new Utils.Tuple<string, bool>("", true);
            };

            var tableViewCopyPasteController = new TableViewPasteController(tableView);
            Assert.AreEqual(1, dataTable.Rows[0][1]);

            //select top left cell
            tableView.SelectCells(1, 0, 0, 0);

            // paste "c" and check result
            tableViewCopyPasteController.PasteLines(new[]
            {
                "b\t2",
                "c\t3"
            });
            Assert.AreEqual(2, dataTable.Rows.Count);
            Assert.AreEqual("c", dataTable.Rows[1][0]);
        }

        private static TableView GetTableViewWithTwoStringColumnsAndOneRow()
        {
            var tableView = new TableView();
            var tableWithTwoStrings = new DataTable();
            tableWithTwoStrings.Columns.Add("Name", typeof(string));
            tableWithTwoStrings.Columns.Add("B", typeof(string));
            tableWithTwoStrings.Rows.Add("a", "b");
            tableView.Data = tableWithTwoStrings;
            return tableView;
        }

        private static Utils.Tuple<string, bool> TableViewEditorValidator(TableViewCell tableViewCell, object value)
        {
            // do not accept "d"
            return ((string) value).Contains("d") ? new Utils.Tuple<string, bool>("", false) : new Utils.Tuple<string, bool>("", true);
        }
    }
}