/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 05/11/2020
 * Time: 08:21
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
    /// Description of CalculateIfProjectMustBeOpened.
    /// </summary>
    [TestModule("0992AFA2-5082-4C3F-B072-0A9A6BE5977C", ModuleType.UserCode, 1)]
    public class CalculateIfProjectMustBeOpened : ITestModule
    {
        
        string _projectMustBeOpened = "";
        [TestVariable("f5d6fdf5-43e2-42d5-a343-d130722b19cb")]
        public string projectMustBeOpened
        {
            get { return _projectMustBeOpened; }
            set { _projectMustBeOpened = value; }
        }
        
        
        string _fileNameOfProjectThatShouldBeOpen = "";
        [TestVariable("72fcd65e-4903-4569-8c21-047c0d41f306")]
        public string fileNameOfProjectThatShouldBeOpen
        {
            get { return _fileNameOfProjectThatShouldBeOpen; }
            set { _fileNameOfProjectThatShouldBeOpen = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateIfProjectMustBeOpened()
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
            Delay.SpeedFactor = 1.0;
            
            var myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var rootProjectNode = myRepository.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.SelfInfo.FindAdapter<TreeItem>();
            string projectCurrentlyOpen = NameOfTreeItem(rootProjectNode);
            int idx = fileNameOfProjectThatShouldBeOpen.LastIndexOf('\\');
            string projectThatShouldBeOpen = fileNameOfProjectThatShouldBeOpen.Substring(idx+1, fileNameOfProjectThatShouldBeOpen.Length-idx-6);
            projectMustBeOpened = projectThatShouldBeOpen == projectCurrentlyOpen?"false":"true";
        }
        
        private string NameOfTreeItem(object treeItemInfo)
        {
        	return treeItemInfo.ToString().Substring(10, treeItemInfo.ToString().Length-11);
        }

    }
}
