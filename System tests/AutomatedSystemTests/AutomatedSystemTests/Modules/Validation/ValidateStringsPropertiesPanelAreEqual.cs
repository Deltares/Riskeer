/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 03/11/2020
 * Time: 14:42
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

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Validation
{
    /// <summary>
    /// Description of ValidateStringsPropertiesPanelAreEqual.
    /// </summary>
    [TestModule("21892C37-B86B-406B-9ADC-194CA2990FA0", ModuleType.UserCode, 1)]
    public class ValidateStringsPropertiesPanelAreEqual : ITestModule
    {
        
        string _string1PropertiesPanel = "";
        [TestVariable("cefe2b90-ac9e-4ca5-aae0-f6c8ba242211")]
        public string string1PropertiesPanel
        {
            get { return _string1PropertiesPanel; }
            set { _string1PropertiesPanel = value; }
        }
        
        string _string2PropertiesPanel = "";
        [TestVariable("a1d17877-47dc-4985-8c24-12313fb3bca5")]
        public string string2PropertiesPanel
        {
            get { return _string2PropertiesPanel; }
            set { _string2PropertiesPanel = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateStringsPropertiesPanelAreEqual()
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
            
                var list1 = string1PropertiesPanel.Substring(1,string1PropertiesPanel.Length-2).Split(new[] { "];[" }, StringSplitOptions.None);
                var list2 = string2PropertiesPanel.Substring(1,string2PropertiesPanel.Length-2).Split(new[] { "];[" }, StringSplitOptions.None);
                for (int idx = 0; idx < list1.Length; idx++) {
                    var elements1 = list1[idx].Split(';');
                    var elements2 = list2[idx].Split(';');
                    if (elements1[1]==elements2[1]) {
                        Validate.AreEqual(elements1[2], elements2[2], "Validating values for row= " + elements1[0] + ", parameter name = " + elements1[1] + ". Expected: {0} Actual: {1}");
                    } else {
                        // Will give an error showing different Accessible names
                        Validate.AreEqual(elements1[1], elements2[1], "Validating parameter names for row= " + elements1[0] + ", Accessible Name = " + elements1[1] + ". Expected: {0} Actual: {1}");
                    }
                }
        }
    }
}
