/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 29/10/2020
 * Time: 14:15
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

namespace AutomatedSystemTests.Modules.Set_Assign
{
    /// <summary>
    /// Description of SetRangeInSmartFolder.
    /// </summary>
    [TestModule("3B9354D6-0537-452C-9857-667F5C0E7124", ModuleType.UserCode, 1)]
    public class SetRangeInSmartFolder : ITestModule
    {
        
        string _nameOfSmartFolder = "";
        [TestVariable("e27c2717-8089-484f-8051-57077da63c0b")]
        public string nameOfSmartFolder
        {
        	get { return _nameOfSmartFolder; }
        	set { _nameOfSmartFolder = value; }
        }
        
        
        string _rangeToBeSetInSmartFolder = "";
        [TestVariable("cd1da467-8e84-488e-b503-621bd966f7be")]
        public string rangeToBeSetInSmartFolder
        {
        	get { return _rangeToBeSetInSmartFolder; }
        	set { _rangeToBeSetInSmartFolder = value; }
        }
        
    	/// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SetRangeInSmartFolder()
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
            TestSuite.Current.GetTestContainer(nameOfSmartFolder).DataContext.SetRange(DataRangeSet.Parse(rangeToBeSetInSmartFolder));
        }
    }
}
