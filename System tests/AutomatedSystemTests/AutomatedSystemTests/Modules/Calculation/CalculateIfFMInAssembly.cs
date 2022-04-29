/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 29/04/2022
 * Time: 10:13
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
    /// Description of CalculateIfFMInAssembly.
    /// </summary>
    [TestModule("FF56E032-8F65-49B8-8A21-69565853CE49", ModuleType.UserCode, 1)]
    public class CalculateIfFMInAssembly : ITestModule
    {
        
        string _distributionFMsInAssembly = "";
        [TestVariable("58e4853c-a2ca-43b4-adf7-da2ae4fd243a")]
        public string distributionFMsInAssembly
        {
            get { return _distributionFMsInAssembly; }
            set { _distributionFMsInAssembly = value; }
        }
        
        
        string _indexCurrentFM = "";
        [TestVariable("ebe78d14-ce80-4f18-af2a-0093ef427864")]
        public string indexCurrentFM
        {
            get { return _indexCurrentFM; }
            set { _indexCurrentFM = value; }
        }
        
        
        string _currentFMInAssembly = "";
        [TestVariable("88fbfa76-abd9-4070-8af1-ea0c6c6d06dc")]
        public string currentFMShouldBeInAssembly
        {
            get { return _currentFMInAssembly; }
            set { _currentFMInAssembly = value; }
        }
        
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateIfFMInAssembly()
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
            var index = distributionFMsInAssembly.Substring(Int32.Parse(indexCurrentFM), 1);
            currentFMShouldBeInAssembly = distributionFMsInAssembly.Substring(Int32.Parse(indexCurrentFM), 1)=="1"?"True":"False";
            Report.Info(distributionFMsInAssembly + " index " + indexCurrentFM + " = " + index.ToString());
            Report.Info("Expected in assembly = " + currentFMShouldBeInAssembly);
        }
    }
}
