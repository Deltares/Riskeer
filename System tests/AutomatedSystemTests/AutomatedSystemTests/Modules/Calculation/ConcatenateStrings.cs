/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 29/10/2020
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
    /// Description of ConcatenateStrings.
    /// </summary>
    [TestModule("B15958CF-4342-4EEF-A2A3-0EC6EC4BE6DA", ModuleType.UserCode, 1)]
    public class ConcatenateStrings : ITestModule
    {
        
    	
    	string _string1 = "";
    	[TestVariable("c0f6f1c8-7180-4d58-9ab9-e28819990843")]
    	public string string1
    	{
    		get { return _string1; }
    		set { _string1 = value; }
    	}
    	
    	
    	string _string2 = "";
    	[TestVariable("5bab9acc-5817-4baf-921f-3345ba8f8ec3")]
    	public string string2
    	{
    		get { return _string2; }
    		set { _string2 = value; }
    	}
    	
    	
    	string _concatenatedString = "";
    	[TestVariable("4296aff0-7ef3-4132-ab64-c24e27efab2e")]
    	public string concatenatedString
    	{
    		get { return _concatenatedString; }
    		set { _concatenatedString = value; }
    	}
    	
    	
    	/// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ConcatenateStrings()
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
            
            concatenatedString = string1 + string2;
        }
    }
}
