/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 02/11/2020
 * Time: 18:57
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Calculation
{
    /// <summary>
    /// Description of ConvertPropertiesPanelToString.
    /// </summary>
    [TestModule("3732FDC4-E20C-44E8-981C-20E1D75C4FC1", ModuleType.UserCode, 1)]
    public class ConvertPropertiesPanelToString : ITestModule
    {
    string _stringifiedPropertiesPanel = "";
    [TestVariable("1a56d401-7ad8-4220-bea4-fe5914a1314c")]
    public string stringifiedPropertiesPanel
    {
        get { return _stringifiedPropertiesPanel; }
        set { _stringifiedPropertiesPanel = value; }
    }
    
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ConvertPropertiesPanelToString()
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
            
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            Adapter propertiesPanelAdapter = myRepository.RiskeerMainWindow.PropertiesPanelContainer.Table.Self;
            
            var allRows = propertiesPanelAdapter.As<Table>().Rows.ToList();
            stringifiedPropertiesPanel = "";
            int index = 0;
            foreach (Ranorex.Row row in allRows) {
                stringifiedPropertiesPanel += "[" + index.ToString() + ";" + row.Element.GetAttributeValueText("AccessibleName") + ";" + row.Element.GetAttributeValueText("AccessibleValue") + "];";
                index++;
            }
            stringifiedPropertiesPanel = stringifiedPropertiesPanel.Remove(stringifiedPropertiesPanel.Length - 1);
            
            Report.Log(ReportLevel.Info, stringifiedPropertiesPanel);
        }
    }
}
