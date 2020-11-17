/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 09/11/2020
 * Time: 13:32
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.IO;
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
    /// Description of CopyAllFilesFromSource.
    /// </summary>
    [TestModule("A6CEA8B4-3109-4085-B6F6-8002979365E3", ModuleType.UserCode, 1)]
    public class CopyAllSearchFilteredFilesFromSourceToTarget : ITestModule
    {
        
        string _sourcePath = "";
        [TestVariable("0f3787cd-d429-4f28-b05b-8a6794693066")]
        public string sourcePath
        {
            get { return _sourcePath; }
            set { _sourcePath = value; }
        }
        
        
        string _targetPath = "";
        [TestVariable("4b460039-a3ab-44fd-87ab-50754ebca3ae")]
        public string targetPath
        {
            get { return _targetPath; }
            set { _targetPath = value; }
        }
        
        
        string _searchPattern = "*.*";
        [TestVariable("1bae0856-551f-4c69-ba7a-84e8f3160f52")]
        public string searchPattern
        {
            get { return _searchPattern; }
            set { _searchPattern = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CopyAllSearchFilteredFilesFromSourceToTarget()
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
            foreach (var sourceFilePath in Directory.GetFiles(sourcePath, searchPattern))
                {
                string fileName = Path.GetFileName(sourceFilePath);
                string destinationFilePath = Path.Combine(targetPath, fileName);
                System.IO.File.Copy(sourceFilePath, destinationFilePath , true);
                }
        }
    }
}
