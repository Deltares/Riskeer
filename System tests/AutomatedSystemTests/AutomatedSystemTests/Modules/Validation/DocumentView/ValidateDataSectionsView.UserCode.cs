﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// Your custom recording code should go in this file.
// The designer will only add methods to this file, so your custom code won't be overwritten.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Validation.DocumentView
{
    public partial class ValidateDataSectionsView
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void ValidationOfDataInSectionsView(Adapter tableVakindelingAdapter)
        {
            var row = tableVakindelingAdapter.As<Table>().Rows[Int32.Parse(indexRow)];
            if (indexRow=="0") {
                Report.Log(ReportLevel.Info, "Validating headers of table (order of columns)");
            } else {
                Report.Log(ReportLevel.Info, "Validating row " + indexRow);
            }
            ValidateRow(row);
        }
        
        private void ValidateRow(Ranorex.Row row)
        {
            if (indexRow=="0" && fmLabel!="STPH" && fmLabel!="STBI") {
                Report.Log(ReportLevel.Info, "Validating that column Nvak is not present.");
                Validate.IsFalse(row.Cells[row.Cells.Count-1].Element.GetAttributeValueText("AccessibleName").Contains("Nvak*"));
            }
            ValidateSingleCell(row, "Validating 1st column", indexColumnName, valueColumnName);
            ValidateSingleCell(row, "Validating 2nd column", indexColumnOffsetFrom, valueColumnOffsetFrom);
            ValidateSingleCell(row, "Validating 3rd column", indexColumnOffsetTo, valueColumnOffsetTo);
            ValidateSingleCell(row, "Validating 4th column", indexColumnLength, valueColumnLength);
            if (fmLabel=="STPH") {
                Report.Log(ReportLevel.Info, "FM is " + fmLabel + " so validating 5th column.");
                ValidateSingleCell(row, "Validating 5th column", indexColumnNvak, valueColumnNvakSTPH);
            } else if (fmLabel=="STBI") {
                Report.Log(ReportLevel.Info, "FM is " + fmLabel + " so validating 5th column.");
                ValidateSingleCell(row, "Validating 5th column", indexColumnNvak, valueColumnNvakSTBI);
            }
        }
        
        private void ValidateSingleCell(Ranorex.Row row, string logMessage, string indexColumn, string expectedValue)
        {
            string actualValue  = "";
            Report.Log(ReportLevel.Info, logMessage);
            if (indexRow=="0") {
                actualValue = row.Cells[Int32.Parse(indexColumn)].Element.GetAttributeValueText("AccessibleValue");
            } else {
                var currentCell = row.Cells[Int32.Parse(indexColumn)];
                currentCell.Select();
                actualValue = currentCell.Element.GetAttributeValueText("AccessibleValue");
                if (Int32.Parse(indexColumn)>1) {
                    System.Globalization.CultureInfo fixedDataSourceCulture = new CultureInfo("en-US");
                    fixedDataSourceCulture.NumberFormat.NumberDecimalSeparator = ".";
                    fixedDataSourceCulture.NumberFormat.NumberGroupSeparator = "";
                    System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
                    actualValue = Double.Parse(actualValue, currentCulture).ToString(fixedDataSourceCulture);
                    expectedValue = Double.Parse(expectedValue, fixedDataSourceCulture).ToString(fixedDataSourceCulture);
                }
            }
            Validate.AreEqual(actualValue, expectedValue);
        }
    }
}
