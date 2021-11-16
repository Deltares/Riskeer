/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 15/11/2021
 * Time: 12:16
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
using Ranorex_Automation_Helpers.UserCodeCollections;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Calculation
{
    /// <summary>
    /// Description of ReplaceAllAliasesInPath.
    /// </summary>
    [TestModule("A6119855-84A5-463A-BD4F-6DAA72A2F84C", ModuleType.UserCode, 1)]
    public class ReplaceAllAliasesInPath : ITestModule
    {
        
        string _path = "";
        [TestVariable("ea190b67-fc25-4cb2-8b91-0b8f513b5d7f")]
        public string path
        {
            get { return _path; }
            set { _path = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReplaceAllAliasesInPath()
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
            path = path.ReplacePathAliases();
        }
    }
}
