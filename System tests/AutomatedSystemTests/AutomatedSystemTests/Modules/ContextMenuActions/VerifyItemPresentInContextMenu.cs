/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 10/11/2020
 * Time: 14:09
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ContextMenuActions
{
    /// <summary>
    /// Description of VerifyItemPresentInContextMenu.
    /// </summary>
    [TestModule("59075182-3627-4F07-A091-1CE36691EE66", ModuleType.UserCode, 1)]
    public class VerifyItemPresentInContextMenu : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public VerifyItemPresentInContextMenu()
        {
            // Do not delete - a parameterless constructor is required!
        }

        
        string _nameOfItem = "";
        [TestVariable("ddcaf430-e452-4fc9-819d-4a31547a7865")]
        public string nameOfItem
        {
            get { return _nameOfItem; }
            set { _nameOfItem = value; }
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
            
            Keyboard.Press("{Apps}");
            
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var contextMenu = myRepository.ContextMenu.Self;
            var itemsInContextMenu = contextMenu.Children;
            
            var numberOfItemsWithNameInContextMenu = itemsInContextMenu.Where(it=> it.Element.ToString().Contains(nameOfItem)).ToList().Count;
            
            if (numberOfItemsWithNameInContextMenu>0) {
                Report.Log(ReportLevel.Success, nameOfItem + " found in context menu.");
                Report.Screenshot(ReportLevel.Info, "User", "", contextMenu, false);
            } else {
                Report.Log(ReportLevel.Warn, nameOfItem + " not found in context menu.");
                Report.Screenshot(ReportLevel.Warn, "User", "", contextMenu, false);
            }
            
            Keyboard.Press("{Escape}");
        }
    }
}
