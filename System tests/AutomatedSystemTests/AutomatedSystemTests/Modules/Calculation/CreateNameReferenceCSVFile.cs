/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 26/10/2020
 * Time: 16:04
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
    /// Description of CreateNameReferenceCSVFile.
    /// </summary>
    [TestModule("BC4F854B-C401-447D-976C-5E86CB4660F2", ModuleType.UserCode, 1)]
    public class CreateNameReferenceCSVFile : ITestModule
    {
        
    	
    	string _genericPath = "";
    	[TestVariable("c447385a-8ec1-44a8-a65b-dd384bf31a91")]
    	public string rootGenericPathReferenceFile
    	{
    		get { return _genericPath; }
    		set { _genericPath = value; }
    	}
    	
    	string _labelFM = "";
    	[TestVariable("6882c06f-eeed-4ba0-a79f-c7f7011c638c")]
    	public string labelFM
    	{
    		get { return _labelFM; }
    		set { _labelFM = value; }
    	}
    	
    	
    	string _pathReferenceFile = "";
    	[TestVariable("e0db401e-9eab-4545-a203-ebdbc3b26f17")]
    	public string pathReferenceFile
    	{
    		get { return _pathReferenceFile; }
    		set { _pathReferenceFile = value; }
    	}
    	
    	
    	/// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CreateNameReferenceCSVFile()
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
            
            pathReferenceFile = rootGenericPathReferenceFile + @"\referenceResults" + labelFM + ".csv";
        }
    }
}
