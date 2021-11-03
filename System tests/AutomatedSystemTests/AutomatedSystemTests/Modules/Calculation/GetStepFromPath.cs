/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 16/11/2020
 * Time: 17:46
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
    /// Description of nthStepFromPath.
    /// </summary>
    [TestModule("D0D76556-8EB7-4EC6-A100-DEB33DE45819", ModuleType.UserCode, 1)]
    public class GetStepFromPath : ITestModule
    {
        
        string _fullPath = "";
        [TestVariable("79fe10d1-42c4-4cc5-929d-1adc08c1204a")]
        public string fullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; }
        }
        
        string _indexStep = "";
        [TestVariable("9265f296-64dd-4f64-a430-82614dfc36ea")]
        public string indexStep
        {
            get { return _indexStep; }
            set { _indexStep = value; }
        }
        
        
        string _stepString = "";
        [TestVariable("72c1ba24-4090-4cb8-8c20-5a5fe2f7b2da")]
        public string stepString
        {
            get { return _stepString; }
            set { _stepString = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GetStepFromPath()
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
            int indexStepInt = Int32.Parse(indexStep);
            int unsignedIndexStep = 0;
            if (indexStepInt<0) {
                fullPath = Reverse(fullPath);
                unsignedIndexStep = - indexStepInt;
            } else {
                unsignedIndexStep = indexStepInt;
            }
            var splittedString = fullPath.Split('>');
            stepString = splittedString[unsignedIndexStep-1];
            if (indexStepInt<0) {
                stepString = Reverse(stepString);
            }
        }
        
        private string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
                Array.Reverse(charArray);
                return new string(charArray);
        }
    }
}
