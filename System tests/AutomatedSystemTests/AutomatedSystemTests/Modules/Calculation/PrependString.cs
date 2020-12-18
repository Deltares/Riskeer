/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 18/12/2020
 * Time: 09:22
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
    /// Description of PrependString.
    /// </summary>
    [TestModule("1F888A85-095F-425F-8F17-E3D525506ECC", ModuleType.UserCode, 1)]
    public class PrependString : ITestModule
    {
        
        
        string _mainString = "";
        [TestVariable("4b2558be-df78-48e5-b43d-0fd066ceebe9")]
        public string mainString
        {
            get { return _mainString; }
            set { _mainString = value; }
        }
        
        
        string _stringToPrependToMainString = "";
        [TestVariable("9f891cef-5938-4926-8560-0ba9fa86a15f")]
        public string stringToPrependToMainString
        {
            get { return _stringToPrependToMainString; }
            set { _stringToPrependToMainString = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public PrependString()
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
            Report.Info("Prepending \"" + stringToPrependToMainString + "\" to \"" + mainString + "\".");
            mainString = stringToPrependToMainString + mainString;
            Report.Info("Resulting string = " + mainString);
        }
    }
}
