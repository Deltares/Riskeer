/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 16/12/2020
 * Time: 11:23
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
    /// Description of AppendString.
    /// </summary>
    [TestModule("ED043061-2B4F-4F84-B57B-5A205F5D26F9", ModuleType.UserCode, 1)]
    public class AppendString : ITestModule
    {
        
        
        string _mainString = "";
        [TestVariable("26242e38-327b-4a74-a725-5f16b6024635")]
        public string mainString
        {
            get { return _mainString; }
            set { _mainString = value; }
        }
        
        string _stringToAppendToMainString = "";
        [TestVariable("ac8b4a2f-fc21-444f-b33b-6207615639e2")]
        public string stringToAppendToMainString
        {
            get { return _stringToAppendToMainString; }
            set { _stringToAppendToMainString = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public AppendString()
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
            Report.Info("Appending \"" + mainString + "\" and \"" + stringToAppendToMainString + "\".");
            mainString += stringToAppendToMainString;
            Report.Info("Resulting string = " + mainString);
        }
    }
}
