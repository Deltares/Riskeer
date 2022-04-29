/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 29/04/2022
 * Time: 11:55
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
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
    /// Description of ValidateFMAssemblyWithContextMenu.
    /// </summary>
    [TestModule("F0965E19-512A-4374-933E-3535F674E2A1", ModuleType.UserCode, 1)]
    public class ValidateFMAssemblyWithContextMenuInProjectExplorer : ITestModule
    {
        
        string _expectedInAssembly = "";
        [TestVariable("0cf4d3ab-a50a-4183-b93c-bd59401ce7c5")]
        public string expectedInAssembly
        {
            get { return _expectedInAssembly; }
            set { _expectedInAssembly = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateFMAssemblyWithContextMenuInProjectExplorer()
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
            Keyboard.Press("{Apps}");
            Report.Screenshot(ReportLevel.Info, "User", "", repo.ContextMenu.Self, false, new RecordItemIndex(4));
            var contextMenuChildren= repo.ContextMenu.Self.Children;
            var menuItemChildren = contextMenuChildren.Where(singleChild => singleChild.Element.Role.ToString()=="MenuItem [menuitem]");
            int  numberOfChildren = menuItemChildren.ToList().Count;
            Report.Info("Failure mechanism (FM) context menu contains the following " + numberOfChildren + " items:");
            bool inAssembly = false;
            foreach (var subitem in menuItemChildren) {
                var nameSubitem = subitem.ToString().Substring(10, subitem.ToString().IndexOf('}')-10);
                string suffixInAssembly = "";
                if (nameSubitem=="Openen") {
                    inAssembly = true;
                    suffixInAssembly = " => FM is in assembly";
                }
                Report.Info(nameSubitem + suffixInAssembly);
            }
            string currentlyInAssembly = inAssembly? "True" : "False";
            Report.Info("The FM is currently in assembly: " + currentlyInAssembly + ".");
            Validate.AreEqual(currentlyInAssembly, expectedInAssembly);
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Escape}'.", new RecordItemIndex(6));
            Keyboard.Press("{Escape}");
        }
    }
}
