using System;
using System.Collections.Generic;
using System.Data;

namespace DelftTools.Utils.Data
{
    /// <summary>
    /// Manipulation of ADO.NET objects.
    /// </summary>
    public class SqlUtils
    {
        /// <summary>
        /// This JOIN method is equivalent to the TSQL INNER JOIN expression using equality.
        /// 
        /// This method returns all columns from both tables.
        /// Once again, column name collision is avoided by appending "_Second" to the columns affected.
        /// 
        /// There are a total of 3 signatures for this method.
        /// In summary the code works as follows:
        /// 
        /// Create new empty table
        /// Create a DataSet and add tables.
        /// Get a reference to Join columns
        /// Create a DataRelation
        /// Construct JOIN table columns
        /// Using the DataRelation add rows with matching related rows using array manipulation
        /// Return table
        /// 
        /// http://weblogs.sqlteam.com/davidm/archive/2004/01/20/748.aspx
        /// </summary>
        /// <param name="First"></param>
        /// <param name="Second"></param>
        /// <param name="firstJoinColumn"></param>
        /// <param name="secondJoinColumn"></param>
        /// <returns></returns>
        /// <param name="leftJoin">specify wether a leftjoin should be performed</param>
        public static DataTable Join(DataTable First, DataTable Second, DataColumn[] firstJoinColumn,
                                     DataColumn[] secondJoinColumn,
                                     bool leftJoin)
        {
            //Create Empty Table
            DataTable table = new DataTable("Join");

            // Use a DataSet to leverage DataRelation
            using (DataSet ds = new DataSet())
            {
                //Add Copy of Tables
                ds.Tables.AddRange(new DataTable[] {First.Copy(), Second.Copy()});

                //Identify Joining Columns from First
                DataColumn[] parentcolumns = new DataColumn[firstJoinColumn.Length];

                for (int i = 0; i < parentcolumns.Length; i++)
                {
                    parentcolumns[i] = ds.Tables[0].Columns[firstJoinColumn[i].ColumnName];
                }

                //Identify Joining Columns from Second
                DataColumn[] childcolumns = new DataColumn[secondJoinColumn.Length];

                for (int i = 0; i < childcolumns.Length; i++)
                {
                    childcolumns[i] = ds.Tables[1].Columns[secondJoinColumn[i].ColumnName];
                }

                //Create DataRelation
                DataRelation r = new DataRelation(string.Empty, parentcolumns, childcolumns, false);

                ds.Relations.Add(r);

                //Create Columns for JOIN table
                for (int i = 0; i < First.Columns.Count; i++)
                {
                    table.Columns.Add(First.Columns[i].ColumnName, First.Columns[i].DataType);
                }

                for (int i = 0; i < Second.Columns.Count; i++)
                {
                    //Beware Duplicates
                    if (!table.Columns.Contains(Second.Columns[i].ColumnName))
                    {
                        table.Columns.Add(Second.Columns[i].ColumnName, Second.Columns[i].DataType);
                    }
                    else
                    {
                        //determine unique Column name 
                        int number = 1;
                        string baseName = Second.Columns[i].ColumnName;

                        string altName = baseName;
                        while (table.Columns.Contains(altName))
                        {
                            altName = baseName + "_" + number; //append number to columnname
                            number++;
                        }

                        table.Columns.Add(altName, Second.Columns[i].DataType);
                    }
                }

                //Loop through First table
                table.BeginLoadData();

                foreach (DataRow firstrow in ds.Tables[0].Rows)
                {
                    //do not include rows that have  row state = deleted
                    if (firstrow.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }

                    //Get "joined" rows
                    DataRow[] childrows = firstrow.GetChildRows(r);

                    if (childrows != null && childrows.Length > 0)
                    {
                        object[] parentarray = firstrow.ItemArray;
                        foreach (DataRow secondrow in childrows)
                        {
                            //do not include rows that have  row state = deleted
                            if (secondrow.RowState == DataRowState.Deleted)
                            {
                                continue;
                            }

                            object[] secondarray = secondrow.ItemArray;
                            object[] joinarray = new object[parentarray.Length + secondarray.Length];
                            Array.Copy(parentarray, 0, joinarray, 0, parentarray.Length);
                            Array.Copy(secondarray, 0, joinarray, parentarray.Length, secondarray.Length);
                            table.LoadDataRow(joinarray, true);
                        }
                    }
                    else if (leftJoin) //left join
                    {
                        object[] parentarray = firstrow.ItemArray;
                        object[] joinarray = new object[parentarray.Length];
                        Array.Copy(parentarray, 0, joinarray, 0, parentarray.Length);
                        table.LoadDataRow(joinarray, true);
                    }
                }

                table.EndLoadData();
            }

            return table;
        }

        /// <summary>
        /// The INNER JOIN returns all rows from both tables where there is a match between 
        /// one or more columns
        /// </summary>
        /// <returns></returns>
        public static DataTable Join(DataTable First, DataTable Second, DataColumn[] firstJoinColumn,
                                     DataColumn[] secondJoinColumn)
        {
            return Join(First, Second, firstJoinColumn, secondJoinColumn, false);
        }

        /// <summary>
        /// The LEFT JOIN returns all the rows from the first table, even if there are no matches in the second table.
        /// </summary>
        /// <returns></returns>
        public static DataTable LeftJoin(DataTable First, DataTable Second, DataColumn firstJoinColumn,
                                         DataColumn secondJoinColumn)
        {
            return Join(First, Second, new DataColumn[] {firstJoinColumn}, new DataColumn[] {secondJoinColumn}, true);
        }

        /// <summary>
        /// The INNER JOIN returns all rows from both tables where there is a match
        /// </summary>
        /// <returns></returns>
        public static DataTable Join(DataTable First, DataTable Second, DataColumn firstJoinColumn,
                                     DataColumn secondJoinColumn)
        {
            return Join(First, Second, new DataColumn[] {firstJoinColumn}, new DataColumn[] {secondJoinColumn}, false);
        }

        /// <summary>
        /// The LEFT JOIN returns all the rows from the first table, even if there are no matches in the second table.
        /// </summary>
        /// <returns></returns>
        public static DataTable LeftJoin(DataTable First, DataTable Second, string firstJoinColumn,
                                         string secondJoinColumn)
        {
            return
                Join(First, Second, new DataColumn[] {First.Columns[firstJoinColumn]},
                     new DataColumn[] {Second.Columns[secondJoinColumn]},
                     true);
        }

        /// <summary>
        /// The INNER JOIN returns all rows from both tables where there is a match
        /// </summary>
        /// <returns></returns>
        public static DataTable Join(DataTable First, DataTable Second, string firstJoinColumn, string secondJoinColumn)
        {
            return
                Join(First, Second, new DataColumn[] {First.Columns[firstJoinColumn]},
                     new DataColumn[] {Second.Columns[secondJoinColumn]},
                     false);
        }

        /// <summary>
        /// The INNER JOIN returns all rows from both tables where there is a match
        /// </summary>
        /// <returns></returns>
        public static DataTable Join(DataTable First, DataTable Second, string[] firstJoinColumns,
                                     string[] secondJoinColumns)
        {
            DataColumn[] firstDataColumnArray = ColumnNamesToDataColumnArray(First, firstJoinColumns);
            DataColumn[] secondDataColumnArray = ColumnNamesToDataColumnArray(Second, secondJoinColumns);

            return Join(First, Second, firstDataColumnArray, secondDataColumnArray, false);
        }


        private static DataColumn[] ColumnNamesToDataColumnArray(DataTable table, IEnumerable<string> columnNames)
        {
            List<DataColumn> colums = new List<DataColumn>();
            foreach (string columnName in columnNames)
            {
                colums.Add(table.Columns[columnName]);
            }
            return colums.ToArray();
        }


        /// <summary>
        /// The LEFT JOIN returns all the rows from the first table, even if there are no matches in the second table.
        /// </summary>
        /// <returns></returns>
        public static DataTable LeftJoin(DataTable First, DataTable Second, string[] firstJoinColumns,
                                         string[] secondJoinColumns)
        {
            DataColumn[] firstDataColumnArray = ColumnNamesToDataColumnArray(First, firstJoinColumns);
            DataColumn[] secondDataColumnArray = ColumnNamesToDataColumnArray(Second, secondJoinColumns);
            return Join(First, Second, firstDataColumnArray, secondDataColumnArray, true);
        }
    }
}