/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 27/10/2020
 * Time: 18:04
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
    /// Description of CalculateFullPathToSeparateCalculation.
    /// </summary>
    [TestModule("CDDCEEDC-F299-4851-BFFC-8B505702E432", ModuleType.UserCode, 1)]
    public class CalculateFullPathToSeparateCalculation : ITestModule
    {
        
        string _pathToTraject = "";
        [TestVariable("f1d993f0-026f-43fb-992c-c5026bd0ea05")]
        public string pathToTraject
        {
        	get { return _pathToTraject; }
        	set { _pathToTraject = value; }
        }
        
        string _pathFromTrajectToSeparateCalculation = "";
        [TestVariable("b3c7e7a8-d7c3-4dc7-a148-62f0443259ef")]
        public string pathFromTrajectToSeparateCalculation
        {
        	get { return _pathFromTrajectToSeparateCalculation; }
        	set { _pathFromTrajectToSeparateCalculation = value; }
        }
        
        string _fullPathToSeparateCalculation = "";
        [TestVariable("82392e5d-6689-455c-993e-70492c6024a5")]
        public string fullPathToSeparateCalculation
        {
        	get { return _fullPathToSeparateCalculation; }
        	set { _fullPathToSeparateCalculation = value; }
        }
        
    	
    	/// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateFullPathToSeparateCalculation()
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
            
            fullPathToSeparateCalculation = pathToTraject + pathFromTrajectToSeparateCalculation; 
        }
    }
}
