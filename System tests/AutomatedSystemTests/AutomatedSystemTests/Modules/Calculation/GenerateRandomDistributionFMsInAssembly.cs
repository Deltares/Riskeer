/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 29/04/2022
 * Time: 10:00
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
    /// Description of GenerateRandomDistributionFMsInAssembly.
    /// </summary>
    [TestModule("90CC222D-99C0-4C5B-B389-DC9D8138F67C", ModuleType.UserCode, 1)]
    public class GenerateRandomDistributionFMsInAssembly : ITestModule
    {
        
        string _numberOfFMs = "";
        [TestVariable("0c018150-e015-472e-9e7e-370bb5b5043f")]
        public string numberOfFMs
        {
            get { return _numberOfFMs; }
            set { _numberOfFMs = value; }
        }
        
        
        string _randomDistributionFMs = "";
        [TestVariable("96b698a5-aced-4f73-b70d-9a7b98d8217b")]
        public string randomDistributionFMs
        {
            get { return _randomDistributionFMs; }
            set { _randomDistributionFMs = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GenerateRandomDistributionFMsInAssembly()
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
            int noFms = Int32.Parse(numberOfFMs);
            int maxValue = (int)Math.Pow(2, noFms);
            Random rnd = new Random();
            int decimalDistribution = rnd.Next(maxValue);
            randomDistributionFMs = Convert.ToString(decimalDistribution, 2).PadLeft(noFms, '0');
            Report.Info("Random distribution of FMs in assembly: " + randomDistributionFMs);
        }
    }
}
