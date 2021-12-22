/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 22/12/2021
 * Time: 09:14
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
    /// Description of ConvertAliasToFullName.
    /// </summary>
    [TestModule("1830CCCE-BA25-4A17-9182-E4BE6813A8DD", ModuleType.UserCode, 1)]
    public class ConvertAliasToFullName : ITestModule
    {
        
        string _Alias = "";
        [TestVariable("7bb41ca2-188f-49ee-85fe-24f10ab8ac24")]
        public string Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }
        
        
        string _FullName = "";
        [TestVariable("2fe226a6-7733-4cbe-af72-d33f051cbf12")]
        public string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ConvertAliasToFullName()
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
            FullName = Alias.ReplacePathAliases();
        }
    }
}
