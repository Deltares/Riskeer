/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 29/04/2022
 * Time: 12:00
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
using Ranorex.Core.Repository;

namespace AutomatedSystemTests.Modules.Validation.ProjectExplorer
{
    /// <summary>
    /// Description of ValidateFMAssemblyWithSubnodesInProjectExplorer.
    /// </summary>
    [TestModule("7EE45812-8EA5-491E-8295-A80D7E29EB79", ModuleType.UserCode, 1)]
    public class ValidateFMAssemblyWithSubnodesInProjectExplorer : ITestModule
    {
        
        string _expectedInAssembly = "";
        [TestVariable("aa59a0bc-0f2e-4790-ba90-42931656bd52")]
        public string expectedInAssembly
        {
            get { return _expectedInAssembly; }
            set { _expectedInAssembly = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateFMAssemblyWithSubnodesInProjectExplorer()
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
            var repo = AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.ProjectExplorerPanel.Self, false, new RecordItemIndex(0));
            var currentFMNode = repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.CurrentFocusInPE;
            var nodeChildren = currentFMNode.Children; 
            int numberOfChildren = nodeChildren.Count;
            Report.Info("Failure mechanism (FM) node contains " + numberOfChildren + " items.");
            foreach (var ch in nodeChildren) {
                var nameChild = ch.ToString().Substring(10, ch.ToString().IndexOf('}')-10);
                Report.Info(nameChild);
            }
            string currentlyInAssembly = numberOfChildren!=1 ? "True" : "False";
            Report.Info("The FM is currently in assembly: " + currentlyInAssembly + ".");
            Validate.AreEqual(currentlyInAssembly, expectedInAssembly);
        }
    }
}
