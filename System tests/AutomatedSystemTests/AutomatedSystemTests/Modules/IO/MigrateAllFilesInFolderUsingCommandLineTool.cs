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
            
            string pathMigrationProgram = GetPathMigrationProgram();
            var allFilesToMigrate = Directory.GetFiles(sourceFolder, "*.risk");
            int numberFilesToMigrate = allFilesToMigrate.Length;
            int i = 1;
            foreach (var sourceFilePath in allFilesToMigrate)
                {
                Report.Info("Migrating project file (" + i.ToString() + "/" + numberFilesToMigrate.ToString() + "): " + sourceFilePath);
                string fileName = Path.GetFileName(sourceFilePath);
                string destinationFilePath = Path.Combine(targetFolder, fileName);
                string commandToRun = "/C " + pathMigrationProgram + " \"" + @sourceFilePath + "\" \"" + @destinationFilePath + "\" >migration.log";
                RunCommand(commandToRun);
                Delay.Duration(new Duration(300));
                ValidateMigratedFilesExists(destinationFilePath);
                i++;
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
        
        private void ValidateMigratedFilesExists(string pathMigratedFile)
        {
            Report.Info("Validating if migrated project file has been created...");
            if (File.Exists(pathMigratedFile)) {
                    Report.Info(pathMigratedFile + " has been found in migration destination folder.");
                } else {
                    Report.Error(pathMigratedFile + " has NOT been found in migration destination folder.");
                }
        }
        
        private string GetPathMigrationProgram()
        {
            string pathMigrationDebug =   "..\\..\\..\\..\\..\\bin\\Debug\\Migratiehulpprogramma.exe";
            string pathMigrationAgent = "bin\\Release\\Migratiehulpprogramma.exe";
            string pathMigrationProgramFound;
            if (File.Exists(pathMigrationDebug)) {
                pathMigrationProgramFound = pathMigrationDebug;
                } else if (File.Exists(pathMigrationAgent)) {
                pathMigrationProgramFound = pathMigrationAgent;
                } else {
                Report.Error("Migration program not found!!");
                return "";
                       }
                Report.Info("Migration program found at " + pathMigrationProgramFound);
                return pathMigrationProgramFound;
        }
    }
}
