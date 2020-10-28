/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 27/10/2020
 * Time: 17:15
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
    /// Description of CalculateFullPathToHydraulicBCcalculation.
    /// </summary>
    [TestModule("BC4780CD-BCE6-4CD8-B162-BB67F03A7885", ModuleType.UserCode, 1)]
    public class CalculateFullPathToHydraulicBCcalculation : ITestModule
    {
        
        string _pathToTraject = "";
        [TestVariable("91cc69f5-1b40-4834-8b49-05e113fd479a")]
        public string pathToTraject
        {
        	get { return _pathToTraject; }
        	set { _pathToTraject = value; }
        }
        
        
        string _pathFromTrajectToCalculation = "";
        [TestVariable("ecfec53b-d181-4fe7-8ee5-b024507fbede")]
        public string pathFromTrajectToCalculation
        {
        	get { return _pathFromTrajectToCalculation; }
        	set { _pathFromTrajectToCalculation = value; }
        }
        
        string _fullPathToCalculation = "";
        [TestVariable("3690de2e-e255-41b9-bd34-cd09b7661fae")]
        public string fullPathToCalculation
        {
        	get { return _fullPathToCalculation; }
        	set { _fullPathToCalculation = value; }
        }
        
    	
    	/// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateFullPathToHydraulicBCcalculation()
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
            fullPathToCalculation = pathToTraject + pathFromTrajectToCalculation;
            
        }
    }
}
