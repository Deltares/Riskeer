/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 02/12/2020
 * Time: 19:27
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

namespace AutomatedSystemTests.Modules.IO
{
    /// <summary>
    /// Description of CompareBinaryFiles.
    /// </summary>
    [TestModule("CB89280B-35E8-42FB-87CD-2EA111D129EA", ModuleType.UserCode, 1)]
    public class CompareBinaryFiles : ITestModule
    {
        
        
        string _filePath1 = "";
        [TestVariable("4cfcb494-35d9-42a7-b61c-90a926f27948")]
        public string filePath1
        {
            get { return _filePath1; }
            set { _filePath1 = value; }
        }
        
        string _filePath2 = "";
        [TestVariable("ae3a6124-5ee3-4e4a-b390-80ccfd69f924")]
        public string filePath2
        {
            get { return _filePath2; }
            set { _filePath2 = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CompareBinaryFiles()
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
            Ranorex.AutomationHelpers.UserCodeCollections.FileLibrary.ValidateFilesBinaryEqual(filePath1, filePath2);
        }
    }
}
