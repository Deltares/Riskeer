/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 05/05/2022
 * Time: 15:16
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Ranorex_Automation_Helpers.UserCodeCollections;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Set_Assign.Assembly
{
    /// <summary>
    /// Description of SetValueInCellResultView.
    /// </summary>
    [TestModule("3633B360-49AF-4C09-B179-F19113C112C9", ModuleType.UserCode, 1)]
    public class SetValueInCellResultView : ITestModule
    {
        
        string _rowIndex = "";
        [TestVariable("26d19c95-6c91-42b1-ae57-08fa3b3e3431")]
        public string rowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }
        
        
        string _columnIndex = "";
        [TestVariable("68211ce0-611f-4fe7-8449-2e8952bcb4ff")]
        public string columnIndex
        {
            get { return _columnIndex; }
            set { _columnIndex = value; }
        }
        
        
        string _newCellValue = "";
        [TestVariable("7fcc6992-077f-4540-beed-6e8fd6dc0113")]
        public string newCellValue
        {
            get { return _newCellValue; }
            set { _newCellValue = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SetValueInCellResultView()
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
            
            // Initiate the variable
            var myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            
            Ranorex.Table table = myRepository.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.FM_ResultView.TableFMResultView.Self;
            Cell cell = table.Rows[Int32.Parse(rowIndex)-1].Cells[Int32.Parse(columnIndex)];
            cell.Focus();
            cell.Select();
            cell.Element.SetAttributeValue("AccessibleValue", newCellValue);
        }
    }
}
