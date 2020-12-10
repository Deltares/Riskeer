/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 06/11/2020
 * Time: 10:15
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

namespace AutomatedSystemTests.Modules.Validation.Abstract
{
    /// <summary>
    /// Description of ValidateStringsAreEqual.
    /// </summary>
    [TestModule("AEAA38ED-2C8F-4C2C-9DBC-888F329CF081", ModuleType.UserCode, 1)]
    public class ValidateStringsAreEqual : ITestModule
    {
        
        string _string1ToCompare = "";
        [TestVariable("63a22286-cee4-4343-bf19-c93a2573e75b")]
        public string string1ToCompare
        {
            get { return _string1ToCompare; }
            set { _string1ToCompare = value; }
        }
        
        string _string2ToCompare = "";
        [TestVariable("51334c9e-35b0-4b8e-a31e-042eacfb5a8e")]
        public string string2ToCompare
        {
            get { return _string2ToCompare; }
            set { _string2ToCompare = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateStringsAreEqual()
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
            
            Report.Info("Validating if string1ToCompare = " + string1ToCompare + " and string2ToCompare = " + string2ToCompare+ " are equal.");
            Validate.AreEqual(string1ToCompare, string2ToCompare,"Validating if string1ToCompare = " + string1ToCompare + " and string2ToCompare = " + string2ToCompare+ " are equal.",false);
        }
    }
}
