/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 25/11/2020
 * Time: 18:32
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
    /// Description of GenerateSomeRandomIntegersInRange.
    /// </summary>
    [TestModule("1D40FCEA-F2DB-450B-958F-73F6DA35960D", ModuleType.UserCode, 1)]
    public class GenerateSomeRandomIntegersInRange : ITestModule
    {
        
        string _minimumInteger = "";
        [TestVariable("6d6bfe56-3be5-41bd-9d5c-a5e89846ab33")]
        public string minimumInteger
        {
            get { return _minimumInteger; }
            set { _minimumInteger = value; }
        }
        
        string _maximumInteger = "";
        [TestVariable("84a54105-4b4f-44fc-860c-9c1f6f6d23fa")]
        public string maximumInteger
        {
            get { return _maximumInteger; }
            set { _maximumInteger = value; }
        }
        
        string _numberOfIntegersToGenerate = "";
        [TestVariable("eb099a5d-e7b2-4f6d-b47a-a5dfa22a12ec")]
        public string numberOfIntegersToGenerate
        {
            get { return _numberOfIntegersToGenerate; }
            set { _numberOfIntegersToGenerate = value; }
        }
        
        
        string _generatedNumbers = "";
        [TestVariable("94368669-746c-4899-95a8-e9dd390495ca")]
        public string generatedNumbers
        {
            get { return _generatedNumbers; }
            set { _generatedNumbers = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GenerateSomeRandomIntegersInRange()
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
            int min = Int32.Parse(minimumInteger);
            int max = Int32.Parse(maximumInteger);
            int numberIntegers = Int32.Parse(numberOfIntegersToGenerate);
            if (max<=min) {
                throw new Exception("Maximum must be greater than minimum.");
            }
            if (numberIntegers<1) {
                throw new Exception("Must generate at least one number.");
            }
            Random a = new Random();
            List<int> randomList = new List<int>();
            int myNumber = 0;
            while (randomList.Count<numberIntegers) {
                myNumber = a.Next(min, max+1);
                if (!randomList.Contains(myNumber))
                    randomList.Add(myNumber);
            }
            randomList.Sort();
            generatedNumbers = string.Join(",", randomList);
        }
    }
}
