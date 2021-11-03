/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 02/11/2021
 * Time: 10:14
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
    /// Description of ConvertProbabilityToCurrentCulture.
    /// </summary>
    [TestModule("22750FB4-C0E3-4CFE-AEF2-41EF0CF191EA", ModuleType.UserCode, 1)]
    public class ConvertProbabilityToCurrentCulture : ITestModule
    {
        
        string _originalProbability = "";
        [TestVariable("6cf6f909-28aa-444f-bb66-18024b346f25")]
        public string originalProbability
        {
            get { return _originalProbability; }
            set { _originalProbability = value; }
        }
        
        
        
        string _convertedProbability = "";
        [TestVariable("4e3d9d15-9f92-43b1-8d85-9b5bf21b9d1b")]
        public string convertedProbability
        {
            get { return _convertedProbability; }
            set { _convertedProbability = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ConvertProbabilityToCurrentCulture()
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
            var denominator = Double.Parse(originalProbability.Substring(2));
            convertedProbability = "1/" + String.Format("{0:n0}", denominator);
        }
    }
}
