/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 09/12/2020
 * Time: 17:24
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

namespace AutomatedSystemTests.Modules.Selection
{
    /// <summary>
    /// Description of UserCodeModule1.
    /// </summary>
    [TestModule("F30C9C45-0EF8-41B8-9FC7-309FAD59682C", ModuleType.UserCode, 1)]
    public class SelectCheckboxesForListRowsInHBCView : ITestModule
    {
        
        
        string _listRowIndecesToSelect = "";
        [TestVariable("83193dfe-bd14-4722-9469-4fdd56269f24")]
        public string listRowIndecesToSelect
        {
            get { return _listRowIndecesToSelect; }
            set { _listRowIndecesToSelect = value; }
        }
        
        
        string _calculationMustBeChecked = "";
        [TestVariable("c7e81578-ff46-480e-a453-c9cd381d1f46")]
        public string calculationMustBeChecked
        {
            get { return _calculationMustBeChecked; }
            set { _calculationMustBeChecked = value; }
        }
        
        string _illPointMustBeChecked = "";
        [TestVariable("74ee3ea5-2cb1-4ac1-bbf3-03c851b9a1d4")]
        public string illPointMustBeChecked
        {
            get { return _illPointMustBeChecked; }
            set { _illPointMustBeChecked = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SelectCheckboxesForListRowsInHBCView()
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
            Delay.SpeedFactor = 1.0;
            
            var repo =global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            
            var table = repo.RiskeerMainWindow.DocumentViewContainerUncached.DesignWaterLevelCalculationsView.LeftSide.Table.Self;
            
            var rowIndecesToProcess = listRowIndecesToSelect.Split(',').ToList();
            
            foreach (var rowIndex in rowIndecesToProcess) {
                var row = table.Rows[Int32.Parse(rowIndex)+1];
                var calcCheckboxCell = row.Children[1].As<Cell>();
                ClickIfNeeded(calcCheckboxCell, calculationMustBeChecked);
                var illPointsCheckboxCell = row.Children[2].As<Cell>();
                ClickIfNeeded(illPointsCheckboxCell, illPointMustBeChecked);
            }
        }
        
        private void ClickIfNeeded(Cell cell, string expectedCheckState)
        {
            if (CellMustBeClicked(cell, expectedCheckState)) {
                        cell.Focus();
                        cell.Select();
                        cell.Click();
                }
        }
        
        private bool CellMustBeClicked(Cell cell, string expectedCheckState)
        {
            string currentState = cell.Element.GetAttributeValueText("AccessibleState");
            string currentlyChecked = currentState.Contains("Checked").ToString().ToLower();
            return currentlyChecked!=expectedCheckState;
        }
    }
}
