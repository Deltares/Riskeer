/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 14/12/2020
 * Time: 12:18
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

namespace AutomatedSystemTests.Modules.Calculation
{
    /// <summary>
    /// Description of ResetVarriable.
    /// </summary>
    [TestModule("FB4C2F53-1A6B-4793-BF45-593FD01B933D", ModuleType.UserCode, 1)]
    public class ResetVariable : ITestModule
    {
        
        string _variableToReset = "";
        [TestVariable("c6798ebf-20c3-4776-9043-ff16ac64a580")]
        public string variableToReset
        {
            get { return _variableToReset; }
            set { _variableToReset = value; }
        }
        
        
        string _newValueForvariable = "";
        [TestVariable("15db2839-9a99-440e-b01a-b5fc66032d72")]
        public string newValueForvariable
        {
            get { return _newValueForvariable; }
            set { _newValueForvariable = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ResetVariable()
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
            variableToReset = newValueForvariable;
        }
    }
}
