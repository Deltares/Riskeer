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
    	
    	
    	string _string3 = "";
    	[TestVariable("22c872ab-6375-41c9-b6a0-aafb1460c90f")]
    	public string string3
    	{
    	    get { return _string3; }
    	    set { _string3 = value; }
    	}
    	
    	string _string4 = "";
    	[TestVariable("58f41f8d-e2cb-4ebe-9653-958a0586c5a5")]
    	public string string4
    	{
    	    get { return _string4; }
    	    set { _string4 = value; }
    	}
    	
    	
    	string _string5 = "";
    	[TestVariable("3e20cd2e-b0b9-4552-aa14-212a8c9693dc")]
    	public string string5
    	{
    	    get { return _string5; }
    	    set { _string5 = value; }
    	}
    	
    	string _string6 = "";
    	[TestVariable("aef0abd0-9b13-4f16-a245-f7f7be97b0d2")]
    	public string string6
    	{
    	    get { return _string6; }
    	    set { _string6 = value; }
    	}
    	
    	string _string7 = "";
    	[TestVariable("ef20b2eb-ff3a-4087-9fbe-116461e1fd42")]
    	public string string7
    	{
    	    get { return _string7; }
    	    set { _string7 = value; }
    	}
    	
    	string _string8 = "";
    	[TestVariable("654df330-d33d-4ec2-ace8-87d684f928fc")]
    	public string string8
    	{
    	    get { return _string8; }
    	    set { _string8 = value; }
    	}
    	
    	string _string9 = "";
    	[TestVariable("ee7be37b-8468-4cd6-9f9e-b1c3da26aed7")]
    	public string string9
    	{
    	    get { return _string9; }
    	    set { _string9 = value; }
    	}
    	
    	string _string10 = "";
    	[TestVariable("8062f433-11a0-400c-be02-f4508d8b0956")]
    	public string string10
    	{
    	    get { return _string10; }
    	    set { _string10 = value; }
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
            
            concatenatedString = string1 + string2 + string3 + string4 + string5 +
                string6 + string7 + string8 + string9 + string10;
            
            Report.Log(ReportLevel.Info, "Concatenated string: " + concatenatedString);
        }
    }
}
