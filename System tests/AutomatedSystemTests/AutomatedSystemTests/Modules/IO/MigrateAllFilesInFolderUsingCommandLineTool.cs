/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 16/11/2020
 * Time: 14:44
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Diagnostics;
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
    /// Description of MigrateAllFilesInFolderUsingCommandLineTool.
    /// </summary>
    [TestModule("D8E46690-D19A-45CE-9514-4E8329F75C83", ModuleType.UserCode, 1)]
    public class MigrateAllFilesInFolderUsingCommandLineTool : ITestModule
    {
        /// <summary>
        /// Path containing the projects in the previous official release version
        /// </summary>
        string _sourceFolder = "";
        [TestVariable("37a1147a-04d5-40e7-aea9-e65cdff17e35")]
        public string sourceFolder
        {
            get { return _sourceFolder; }
            set { _sourceFolder = value; }
        }
        
        /// <summary>
        /// Path containing the projects in the up-to-date version
        /// </summary>
        string _targetFolder = "";
        [TestVariable("f3b3e712-db1c-4e56-8986-c0607c00391a")]
        public string targetFolder
        {
            get { return _targetFolder; }
            set { _targetFolder = value; }
        }
        
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public MigrateAllFilesInFolderUsingCommandLineTool()
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
            
            foreach (var sourceFilePath in Directory.GetFiles(sourceFolder, "*.risk"))
                {
                string fileName = Path.GetFileName(sourceFilePath);
                string destinationFilePath = Path.Combine(targetFolder, fileName);
                
                //string commandToRun = "/C ..\\..\\..\\..\\..\\bin\\Debug\\Application\\Built-in\\Managed\\Core\\Migratiehulpprogramma.exe " + @sourceFilePath + " " + @destinationFilePath + " >borrame.log";
                string commandToRun = "/C ..\\..\\..\\..\\..\\bin\\Debug\\Migratiehulpprogramma.exe " + @sourceFilePath + " " + @destinationFilePath + " >borrame.log";
                //string commandToRun = "/C dir *.* >borrame.log";
                
                RunCommand(commandToRun);
               //var psi = new ProcessStartInfo(commandToRun) {
               //     UseShellExecute = true,
               //     CreateNoWindow = false
               //         };
                //Process.Start(psi);
                
                //System.Diagnostics.Process.Start("dir *.*");
                }
        }
        
        private void RunCommand(string arguments)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal; //.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = arguments;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
