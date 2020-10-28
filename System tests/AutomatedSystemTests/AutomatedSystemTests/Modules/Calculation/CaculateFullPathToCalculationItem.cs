/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 23/10/2020
 * Time: 17:57
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
    /// Description of CaculateFullPathToCalculationInput.
    /// </summary>
    [TestModule("773D0A23-8A22-4D36-AD19-252F24FD9823", ModuleType.UserCode, 1)]
    public class CaculateFullPathToCalculationItem : ITestModule
    {
        
    	
    	string _fullPath = "";
    	[TestVariable("69eee4cc-06f5-4ef5-a9c7-fb232b4039c3")]
    	public string fullPath
    	{
    		get { return _fullPath; }
    		set { _fullPath = value; }
    	}
    	
    	
    	string _fullPathToAllCalculationsNode = "";
    	[TestVariable("55df0884-b1a3-4e1e-a02f-fe3f78aa18e6")]
    	public string fullPathToAllCalculationsNode
    	{
    		get { return _fullPathToAllCalculationsNode; }
    		set { _fullPathToAllCalculationsNode = value; }
    	}
    	
    	string _relativePathToCalculation = "";
    	[TestVariable("cea87378-b630-4c95-801a-d943b01b4f9f")]
    	public string relativePathToFolderWithCalculations
    	{
    		get { return _relativePathToCalculation; }
    		set { _relativePathToCalculation = value; }
    	}
    	
    	string _relativePathToCalculationFromFolder = "";
    	[TestVariable("7a9dc1a6-33e2-4d13-b479-241657087175")]
    	public string relativePathToCalculationFromFolder
    	{
    		get { return _relativePathToCalculationFromFolder; }
    		set { _relativePathToCalculationFromFolder = value; }
    	}
    	
    	string _relativePathToItemFromCalculation = "";
    	[TestVariable("8f6b75a3-47c7-445d-b00e-ab59e786097c")]
    	public string relativePathToItemFromCalculation
    	{
    		get { return _relativePathToItemFromCalculation; }
    		set { _relativePathToItemFromCalculation = value; }
    	}
    	
    	
    	
    	/// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CaculateFullPathToCalculationItem()
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
            
            fullPath = fullPathToAllCalculationsNode + relativePathToFolderWithCalculations + relativePathToCalculationFromFolder + relativePathToItemFromCalculation;
        }
    }
}
