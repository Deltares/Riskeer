/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 20/11/2020
 * Time: 17:42
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
    /// Description of GenerateRandomIndexInRangeOfSmartFolder.
    /// </summary>
    [TestModule("57E8241E-E1ED-4068-A5F9-CF3C3A66E673", ModuleType.UserCode, 1)]
    public class GenerateRandomIndexInRangeOfSmartFolder : ITestModule
    {
        
        
        string _nameOfSmartFolder = "";
        [TestVariable("46e1126b-a8f6-47ec-81ed-ffabf616caff")]
        public string nameOfSmartFolder
        {
            get { return _nameOfSmartFolder; }
            set { _nameOfSmartFolder = value; }
        }
        
        
        string _generatedRandomIndex = "";
        [TestVariable("fc28675a-0851-49ee-90d4-85279ce6fa30")]
        public string generatedRandomIndex
        {
            get { return _generatedRandomIndex; }
            set { _generatedRandomIndex = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GenerateRandomIndexInRangeOfSmartFolder()
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
            int maximumIndex = TestSuite.Current.GetTestContainer(nameOfSmartFolder).DataContext.Source.Rows.Count;
            generatedRandomIndex= new Random().Next(1, maximumIndex + 1).ToString();
        }
    }
}
