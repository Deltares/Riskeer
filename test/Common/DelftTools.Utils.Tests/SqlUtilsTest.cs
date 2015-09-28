using System;
using System.Data;
using DelftTools.Utils.Data;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class SqlUtilsTest
    {
        private const int maleID = 1;
        private const int femaleID = 2;
        private const string genderIDColName = "ID";
        private const string personGenderIDColName = "GenderID";

        private static DataTable CreateGenderDataTable()
        {
            DataTable genderDataTable = new DataTable();
            DataColumn genderIDCol = new DataColumn(genderIDColName, typeof (int));
            genderDataTable.Columns.Add(genderIDCol);
            genderDataTable.Columns.Add(new DataColumn("Gender", typeof (string)));
            genderDataTable.Rows.Add(new object[] {maleID, "Male"});
            genderDataTable.Rows.Add(new object[] {femaleID, "Female"});
            return genderDataTable;
        }

        private static DataTable CreatePersonDataTable()
        {
            DataTable personDataTable = new DataTable();
            personDataTable.Columns.Add(new DataColumn("ID", typeof (int)));
            personDataTable.Columns.Add(new DataColumn("Name", typeof (string)));
            personDataTable.Columns.Add(new DataColumn("Date", typeof (DateTime)));
            DataColumn personGenderDataColumn = new DataColumn(personGenderIDColName, typeof (int));
            personDataTable.Columns.Add(personGenderDataColumn);
            personDataTable.Rows.Add(new object[] {1, "Charlie", DateTime.Now, maleID});
            personDataTable.Rows.Add(new object[] {2, "Candy", DateTime.Now.AddHours(2), femaleID});
            personDataTable.Rows.Add(new object[] {2, "Elsa", DateTime.Now.AddHours(2), femaleID});
            return personDataTable;
        }

        /// <summary>
        /// Join data from gender and person table.
        /// </summary>
        [Test]
        public void JoinTablesTest()
        {
            DataTable genderDataTable = CreateGenderDataTable();
            DataTable personDataTable = CreatePersonDataTable();
            DataColumn genderIDCol = genderDataTable.Columns[genderIDColName];
            DataColumn personGenderDataColumn = personDataTable.Columns[personGenderIDColName];

            //makes new table containing all fields from both tables.
            DataTable joinTable = SqlUtils.Join(genderDataTable, personDataTable, genderIDCol, personGenderDataColumn);
            Assert.AreEqual(6, joinTable.Columns.Count);
            Assert.AreEqual(Math.Max(genderDataTable.Rows.Count, personDataTable.Rows.Count), joinTable.Rows.Count);
            Assert.AreEqual(Math.Max(personDataTable.Rows.Count, genderDataTable.Rows.Count), joinTable.Rows.Count);
            Assert.AreEqual(genderDataTable.Columns[0].ColumnName, joinTable.Columns[0].ColumnName);
            Assert.AreEqual(genderDataTable.Columns[1].ColumnName, joinTable.Columns[1].ColumnName);
            Assert.AreEqual("ID_1", joinTable.Columns[2].ColumnName);
            Assert.AreEqual(personDataTable.Columns[1].ColumnName, joinTable.Columns[3].ColumnName);
            Assert.AreEqual(personDataTable.Columns[2].ColumnName, joinTable.Columns[4].ColumnName);
            Assert.AreEqual(personDataTable.Columns[3].ColumnName, joinTable.Columns[5].ColumnName);
            Assert.AreEqual(personDataTable.Rows.Count, joinTable.Rows.Count);
            Assert.AreEqual("Female", joinTable.Rows[1]["Gender"]);

            //polymorphic variants
            joinTable = SqlUtils.Join(genderDataTable, personDataTable, genderIDColName, personGenderIDColName);
            Assert.AreEqual(6, joinTable.Columns.Count);

            joinTable =
                SqlUtils.Join(genderDataTable, personDataTable, new DataColumn[] {genderIDCol},
                              new DataColumn[] {personGenderDataColumn});
            Assert.AreEqual(6, joinTable.Columns.Count);

            //remove one row from persons (rowstate=deleted)
            personDataTable.Rows[1].Delete();
            joinTable = SqlUtils.Join(genderDataTable, personDataTable, genderIDColName, personGenderIDColName);
            Assert.AreEqual(6, joinTable.Columns.Count);
            Assert.AreEqual(Math.Max(genderDataTable.Rows.Count, personDataTable.Rows.Count), joinTable.Rows.Count);
        }

        [Test]
        public void LeftJoinTablesTest()
        {
            DataTable genderDataTable = CreateGenderDataTable();
            DataTable personDataTable = CreatePersonDataTable();

            // add record to persontable that has no valid reference to valid gender
            personDataTable.Rows.Add(new object[] {3, "SomePerson", DateTime.Now.AddHours(2), null});
            DataTable joinTable =
                SqlUtils.LeftJoin(personDataTable, genderDataTable, personGenderIDColName, genderIDColName);
            Assert.AreEqual(6, joinTable.Columns.Count);
            Assert.AreEqual(Math.Max(genderDataTable.Rows.Count, personDataTable.Rows.Count), joinTable.Rows.Count);
        }
    }
}