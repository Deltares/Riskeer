/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 04/12/2020
 * Time: 10:22
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

using Jitbit.Utils;


namespace AutomatedSystemTests.Modules.IO
{
    /// <summary>
    /// Description of ExportTableInFMResultsViewToCSVFile.
    /// </summary>
    [TestModule("EF7B61EA-A83B-469C-9970-E4C28A5C1809", ModuleType.UserCode, 1)]
    public class ExportTableInFMResultsViewToCSVFile : ITestModule
    {
        
        string _separationCharacter = "";
        [TestVariable("f7ea4d83-6da6-4c33-a02e-40cea90d13a5")]
        public string separationCharacter
        {
            get { return _separationCharacter; }
            set { _separationCharacter = value; }
        }
        
        
        string _fileName = "";
        [TestVariable("429af26b-315c-4200-bf60-9d143f9ca28a")]
        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ExportTableInFMResultsViewToCSVFile()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0.0;
            
            var repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var tableResultView = repo.RiskeerMainWindow.DocumentViewContainerUncached.FM_ResultView.Table.Self.As<Table>();
            
            var allRows = tableResultView.Rows.ToList();
            var myExport = new CsvExport(separationCharacter, false,true);
            
            var headersCells = allRows[0].Cells.ToList();
            headersCells.RemoveAt(0);
            var headersNames = headersCells.Select(hd=>hd.Element.GetAttributeValueText("AccessibleValue")).ToList();
            headersNames = headersNames.Select(name => name.Replace("\r\n", " ")).ToList();
            allRows.RemoveAt(0);
            int index = 1;
            foreach (Ranorex.Row row in allRows) {
                myExport.AddRow();
                myExport["indexRow"] = index.ToString();
                var allCells = row.Cells.ToList();
                for (int i = 1; i < allCells.Count; i++) {
                    var currentCell = allCells[i];
                    currentCell.Focus();
                    currentCell.Select();
                    string accValue = currentCell.Element.GetAttributeValueText("AccessibleValue");
                    myExport[headersNames[i-1]] = accValue;
                }
                index++;
            }
            myExport.ExportToFile(fileName);
            
            
        }
    }
}
