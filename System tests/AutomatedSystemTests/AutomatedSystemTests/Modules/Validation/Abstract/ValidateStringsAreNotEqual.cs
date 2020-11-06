/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 06/11/2020
 * Time: 10:25
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
    /// Description of ValidateStringsAreNotEqual.
    /// </summary>
    [TestModule("222B5582-AEA1-4E76-9EAA-CB4D3A95F0A4", ModuleType.UserCode, 1)]
    public class ValidateStringsAreNotEqual : ITestModule
    {
        
        string _string1ToCompare = "";
        [TestVariable("28aabd9b-349c-4bf1-9e2d-cb0a94710702")]
        public string string1ToCompare
        {
            get { return _string1ToCompare; }
            set { _string1ToCompare = value; }
        }
        
        string _string2ToCompare = "";
        [TestVariable("15d525eb-fc24-4761-9247-46744302861d")]
        public string string2ToCompare
        {
            get { return _string2ToCompare; }
            set { _string2ToCompare = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateStringsAreNotEqual()
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
            
            Validate.IsFalse(string1ToCompare==string2ToCompare, "Validating that string1ToCompare and string2ToCompare are NOT equal.");
        }
    }
}
