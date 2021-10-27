/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 02/11/2020
 * Time: 17:24
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

namespace AutomatedSystemTests.Modules.ActionsVisibilityItemsPropertiesPanel
{
    /// <summary>
    /// Description of ExpandAllRowItemsInPropertiesPanel.
    /// </summary>
    [TestModule("DE336D4D-6903-4666-935E-4619A8A89C40", ModuleType.UserCode, 1)]
    public class ExpandAllRowItemsInPropertiesPanel : ITestModule
    {
        
        
        string _durationPressRightKey = "";
        [TestVariable("dad1141d-7f02-4a3a-a011-eb32c0ca7b1e")]
        public string durationPressRightKey
        {
            get { return _durationPressRightKey; }
            set { _durationPressRightKey = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ExpandAllRowItemsInPropertiesPanel()
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

            var rowsMustBeExpanded = rowsList.Where(rw=>rw.Element.GetAttributeValueText("AccessibleState").ToString().Contains("Collapsed"));
            bool expNeeded = rowsMustBeExpanded.Any();
            while (expNeeded) {
                foreach (var rw in rowsMustBeExpanded) {
                    rw.Focus();
                    rw.Select();
                    rw.PressKeys("{Right}");
                }
                rowsList = myRepository.RiskeerMainWindow.ContainerMultipleViews.PropertiesPanelContainer.Table.Self.As<Table>().Rows.Except(rowsList);
                rowsMustBeExpanded = rowsList.Where(rw=>rw.Element.GetAttributeValueText("AccessibleState").ToString().Contains("Collapsed"));
                expNeeded = rowsMustBeExpanded.Any();
            }
        }
    }
}
