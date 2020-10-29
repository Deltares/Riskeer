/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 29/10/2020
 * Time: 14:15
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

namespace AutomatedSystemTests.Modules.Set_Assign
{
    /// <summary>
    /// Description of SetRangeInSmartFolder.
    /// </summary>
    [TestModule("3B9354D6-0537-452C-9857-667F5C0E7124", ModuleType.UserCode, 1)]
    public class SetRangeInSmartFolder : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SetRangeInSmartFolder()
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
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 100;
            Delay.SpeedFactor = 1.0;
        }
    }
}
