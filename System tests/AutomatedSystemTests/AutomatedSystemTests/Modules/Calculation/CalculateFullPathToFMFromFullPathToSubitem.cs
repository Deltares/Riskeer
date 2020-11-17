/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 10/11/2020
 * Time: 14:50
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
    /// Description of CalculateFullPathToFMFromFullPathToSubitem.
    /// </summary>
    [TestModule("9621D7A6-85D9-4061-87D6-529FC5356C46", ModuleType.UserCode, 1)]
    public class CalculateFullPathToFMFromFullPathToSubitem : ITestModule
    {
        
        string _fullPathToSubitemOfFM = "";
        [TestVariable("9731f22a-ca63-407b-a787-7c6c8ce0c3e9")]
        public string fullPathToSubitemOfFM
        {
            get { return _fullPathToSubitemOfFM; }
            set { _fullPathToSubitemOfFM = value; }
        }
        
        string _fullPathToFM = "";
        [TestVariable("8fc2a5db-1cbe-485a-a15c-bf89dcdf3864")]
        public string fullPathToFM
        {
            get { return _fullPathToFM; }
            set { _fullPathToFM = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateFullPathToFMFromFullPathToSubitem()
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
            int index = fullPathToSubitemOfFM.IndexOf('>', fullPathToSubitemOfFM.IndexOf('>') + 1);
            fullPathToFM = fullPathToSubitemOfFM.Substring(0,index);
        }
    }
}
