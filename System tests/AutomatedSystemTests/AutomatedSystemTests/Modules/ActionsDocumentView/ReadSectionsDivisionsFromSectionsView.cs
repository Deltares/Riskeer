/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 04/12/2020
 * Time: 17:48
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

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ReadSectionsDivisionsFromSectionsView.
    /// </summary>
    [TestModule("F63ED64A-DB92-435D-A38E-21A7DC8E6F54", ModuleType.UserCode, 1)]
    public class ReadSectionsDivisionsFromSectionsView : ITestModule
    {
        
        
        string _stringDataTable = "";
        [TestVariable("89baf06e-3128-406f-92f0-3478857af297")]
        public string stringDataTable
        {
            get { return _stringDataTable; }
            set { _stringDataTable = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReadSectionsDivisionsFromSectionsView()
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
            Delay.SpeedFactor = 0;
            
            var repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            
            var tableSectionsDivisions = repo.RiskeerMainWindow.DocumentViewContainerUncached.TableVakindeling;
            var rowsData = tableSectionsDivisions.Rows;
            rowsData.RemoveAt(0);
            
            string tableData = "";
            
            foreach (var row in rowsData) {
                var cellsDataInRow = row.Cells.ToList();
                tableData += "[";
                for (int i = 1; i < 4; i++) {
                    var cellCurrent = cellsDataInRow[i];
                    cellCurrent.Select();
                    tableData += cellCurrent.Element.GetAttributeValueText("AccessibleValue") + ";";
                }
                tableData = tableData.TrimEnd(';') + "]*";
            }
            stringDataTable += tableData.TrimEnd('*') + "|";
        }
    }
}
