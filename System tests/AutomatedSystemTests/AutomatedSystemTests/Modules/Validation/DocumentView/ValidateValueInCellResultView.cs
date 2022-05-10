/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 05/05/2022
 * Time: 11:49
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
using System.Linq;
using Ranorex_Automation_Helpers.UserCodeCollections;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Validation.DocumentView
{
    /// <summary>
    /// Description of ValidateValueInCellResultView.
    /// </summary>
    [TestModule("D8ED1182-CCC8-464F-A05F-1F3D3498DE01", ModuleType.UserCode, 1)]
    public class ValidateValueInCellResultView : ITestModule
    {
        
        
        string _rowIndex = "";
        [TestVariable("653a5740-2cef-411f-b1be-a0784c90013b")]
        public string rowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }
        
        
        string _columnIndex = "";
        [TestVariable("130652fa-36c0-40eb-8b3e-d2c1f53ae976")]
        public string columnIndex
        {
            get { return _columnIndex; }
            set { _columnIndex = value; }
        }
        
        
        
        string _referenceValue = "";
        [TestVariable("ac2c1baf-9255-4edd-9878-1725de92db82")]
        public string referenceValue
        {
            get { return _referenceValue; }
            set { _referenceValue = value; }
        }
        
        
        string _isReferenceValueExpectedToBeMet = "";
        [TestVariable("5227b799-2f99-4fee-ab07-bd24308c05bb")]
        public string isReferenceValueExpectedToBeMet
        {
            get { return _isReferenceValueExpectedToBeMet; }
            set { _isReferenceValueExpectedToBeMet = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateValueInCellResultView()
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
            Cell cell;
            int columnIndexInteger = Int32.Parse(columnIndex);
            if (columnIndexInteger<0) {
                cell = table.Rows[Int32.Parse(rowIndex)-1].Cells.Reverse().ToList()[Math.Abs(columnIndexInteger)-1];
            } else
            {
                cell = table.Rows[Int32.Parse(rowIndex)-1].Cells[Int32.Parse(columnIndex)];
            }
            cell.Focus();
            cell.Select();
            string currentValue = cell.Element.GetAttributeValueText("AccessibleValue").ToNoGroupSeparator().ToInvariantCultureDecimalSeparator();
            Validate.AreEqual(bool.Parse(isReferenceValueExpectedToBeMet), referenceValue == currentValue);
        }
    }
}
