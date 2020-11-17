/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 09/11/2020
 * Time: 13:40
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
    /// Description of DeleteAllSearchFilteredFiles.
    /// </summary>
    [TestModule("BBC504B6-A274-49FD-A91C-60AE313DD965", ModuleType.UserCode, 1)]
    public class DeleteAllSearchFilteredFiles : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public DeleteAllSearchFilteredFiles()
        {
            // Do not delete - a parameterless constructor is required!
        }

        
        string _folderToDeleteFiles = "";
        [TestVariable("3c5ffbb5-af24-45ac-9e77-199557e248f9")]
        public string folderToDeleteFiles
        {
            get { return _folderToDeleteFiles; }
            set { _folderToDeleteFiles = value; }
        }
        
        
        string _searchFilter = "";
        [TestVariable("21292db3-75da-487e-9c9b-bb2ad030ab77")]
        public string searchFilter
        {
            get { return _searchFilter; }
            set { _searchFilter = value; }
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
            foreach (var filePath in Directory.GetFiles(folderToDeleteFiles, searchFilter))
                {
                System.IO.File.Delete(filePath);
                }
        }
    }
}
