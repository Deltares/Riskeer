/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 29/10/2021
 * Time: 08:22
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Linq;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsPropertiesPanel
{
    /// <summary>
    /// Description of CollapseRowItemsInPropertiesPanel.
    /// </summary>
    [TestModule("9FAAAB93-1756-4755-9753-200E0BF3E602", ModuleType.UserCode, 1)]
    public class CollapseRowItemsInPropertiesPanel : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CollapseRowItemsInPropertiesPanel()
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
            Adapter propertiesPanelAdapter = myRepository.RiskeerMainWindow.ContainerMultipleViews.PropertiesPanelContainer.Table.Self;
            
            IEnumerable<Row> rowsList = propertiesPanelAdapter.As<Table>().Rows;
            IEnumerable<Row> rowsMustBeCollapsed = rowsList.Where(rw=>rw.Element.GetAttributeValueText("AccessibleState").ToString().Contains("Expanded"));
            
            foreach (var rw in rowsMustBeCollapsed.Reverse()) {
                rw.Focus();
                rw.Select();
                rw.PressKeys("{Left}");
                }
        }
    }
}
