/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 19/11/2020
 * Time: 11:29
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
    /// Description of SetRandomRowWithinRangeSmartFolder.
    /// </summary>
    [TestModule("D0D5E1A6-FBFC-4491-A44A-E731880352A4", ModuleType.UserCode, 1)]
    public class SetRandomRowWithinRangeSmartFolder : ITestModule
    {
        
        string _nameSmartFolder = "";
        [TestVariable("28356f49-3ecf-4e67-b40c-4e2c494e61ce")]
        public string nameSmartFolder
        {
            get { return _nameSmartFolder; }
            set { _nameSmartFolder = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SetRandomRowWithinRangeSmartFolder()
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
            int maxNumber = TestSuite.Current.GetTestContainer(nameSmartFolder).DataContext.Source.Rows.Count;
            Random rnd = new Random();
            string randomRowIndex = rnd.Next(1, maxNumber + 1).ToString();
            TestSuite.Current.GetTestContainer(nameSmartFolder).DataContext.SetRange(DataRangeSet.Parse(randomRowIndex));
        }
    }
}
