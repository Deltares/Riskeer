/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 14/11/2020
 * Time: 19:36
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
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
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Validation.DocumentView
{
    /// <summary>
    /// Description of ValidateValueInTableOfHBCDocumentView.
    /// </summary>
    [TestModule("556EE382-BFB6-405A-929C-A48C9B1D339B", ModuleType.UserCode, 1)]
    public class ValidateValueInTableOfHBCDocumentView : ITestModule
    {
        
        string _rowIndex = "";
        [TestVariable("0496fa63-b4f7-4b7a-844d-d02f0c0d4c5f")]
        public string rowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }
        
        string _columnIndex = "";
        [TestVariable("a689d753-fc9c-44c8-89b3-018bf4424da8")]
        public string columnIndex
        {
            get { return _columnIndex; }
            set { _columnIndex = value; }
        }
        
        string _comparisonValue = "";
        [TestVariable("cd5ceefa-263c-4bfc-9666-bb8867efc808")]
        public string comparisonValue
        {
            get { return _comparisonValue; }
            set { _comparisonValue = value; }
        }
        
        string _isComparisonValueExpectedToBeMet = "";
        [TestVariable("d7703330-e917-46c0-895f-bd8c7df52928")]
        public string isComparisonValueExpectedToBeMet
        {
            get { return _isComparisonValueExpectedToBeMet; }
            set { _isComparisonValueExpectedToBeMet = value; }
        }
        
        string _labelFM = "";
        [TestVariable("db7295e4-3f53-4f0b-9cfb-89e2468a69dc")]
        public string labelFM
        {
            get { return _labelFM; }
            set { _labelFM = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateValueInTableOfHBCDocumentView()
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
            
            var myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            // To create the variable
            Ranorex.Table table = myRepository.RiskeerMainWindow.ContainerMultipleViews.MessagesDataGridView.Self;
            if (labelFM=="GEBU") {
                table = myRepository.RiskeerMainWindow.DocumentViewContainer.DesignWaterLevelCalculationsViewCached.LeftSide.Table.Self;
            } else if (labelFM=="DA") {
                table = myRepository.RiskeerMainWindow.DocumentViewContainer.HydraulicBCDunes.Table.Self;
            }

            Row row = table.Rows[Int32.Parse(rowIndex)+1];
            Cell cell = row.Cells[Int32.Parse(columnIndex)];
            cell.Focus();
            cell.Select();
            string currentValue = cell.Element.GetAttributeValueText("AccessibleValue");
            
            if (comparisonValue=="-") {
                if (isComparisonValueExpectedToBeMet=="true") {
                    Validate.AreEqual(currentValue, comparisonValue);
                }
                else    {
                    Validate.IsTrue(currentValue!=comparisonValue);
                }
            } else{
                System.Globalization.CultureInfo fixedDataSourceCulture = new CultureInfo("en-US");
                fixedDataSourceCulture.NumberFormat.NumberDecimalSeparator = ".";
                fixedDataSourceCulture.NumberFormat.NumberGroupSeparator = "";
                System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
                
                double expectedValueDouble = Double.Parse(comparisonValue, fixedDataSourceCulture);
                double currentValueDouble = Double.Parse(currentValue, currentCulture);
                
                if (isComparisonValueExpectedToBeMet=="true") {
                    Validate.AreEqual(currentValueDouble, expectedValueDouble);
                }
                else    {
                    Validate.IsTrue(currentValueDouble!=expectedValueDouble);
                }
            }
        }
    }
}
