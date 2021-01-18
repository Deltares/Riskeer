/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 27/11/2020
 * Time: 12:01
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
    /// Description of EvaluateIfSubstringIsPartOfString.
    /// </summary>
    [TestModule("2821380D-7D9D-4518-9F9D-29776BF9FAC9", ModuleType.UserCode, 1)]
    public class EvaluateIfSubstringIsPartOfString : ITestModule
    {
        
        
        string _substring = "";
        [TestVariable("08ebd391-0eb0-4215-8215-b9007ce05ccb")]
        public string substring
        {
            get { return _substring; }
            set { _substring = value; }
        }
        
        string _fullString = "";
        [TestVariable("aea74b5d-5d51-4ae1-9c93-13d270879cc3")]
        public string fullString
        {
            get { return _fullString; }
            set { _fullString = value; }
        }
        
        
        string _isSubstringPartOfFullString = "";
        [TestVariable("abea2477-2374-4dd2-986e-f39a2710d226")]
        public string isSubstringPartOfFullString
        {
            get { return _isSubstringPartOfFullString; }
            set { _isSubstringPartOfFullString = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public EvaluateIfSubstringIsPartOfString()
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
            int index = fullString.IndexOf(substring);
            if (index==-1) {
                isSubstringPartOfFullString = "false";
            } else {
                isSubstringPartOfFullString = "true";
            }
        }
    }
}
