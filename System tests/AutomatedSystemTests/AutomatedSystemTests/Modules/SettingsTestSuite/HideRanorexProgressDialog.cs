/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 30/10/2020
 * Time: 15:21
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

namespace AutomatedSystemTests.Modules.SettingsTestSuite
{
    /// <summary>
    /// Description of SwitchOffRanorexProgressDialog.
    /// </summary>
    [TestModule("C755C323-FB6E-4704-8789-DD16A021E929", ModuleType.UserCode, 1)]
    public class HideRanorexProgressDialog : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public HideRanorexProgressDialog()
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
            Ranorex.Controls.ProgressForm.Hide();
        }
    }
}
