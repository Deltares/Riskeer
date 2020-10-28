/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 27/10/2020
 * Time: 18:39
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

namespace AutomatedSystemTests
{
    /// <summary>
    /// Description of CalculateFullPathToResultsFromFullPathToCalculation.
    /// </summary>
    [TestModule("95B6D533-7626-4835-B2BE-85C4EC709857", ModuleType.UserCode, 1)]
    public class CalculateFullPathToResultsFromFullPathToCalculation : ITestModule
    {
        
        string _fullPathToCalculation = "";
        [TestVariable("6a9998ac-3ae1-48ee-ab1f-84d47f345c8a")]
        public string fullPathToCalculation
        {
        	get { return _fullPathToCalculation; }
        	set { _fullPathToCalculation = value; }
        }
        
        string _fullpathToResultsOfCalculation = "";
        [TestVariable("c97cb09b-34d2-46be-9162-2abbd9a96785")]
        public string fullpathToResultsOfCalculation
        {
        	get { return _fullpathToResultsOfCalculation; }
        	set { _fullpathToResultsOfCalculation = value; }
        }
        
    	
    	/// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateFullPathToResultsFromFullPathToCalculation()
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
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0.0;
            fullpathToResultsOfCalculation = fullPathToCalculation + ">Resultaat";
        }
    }
}
