/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 30/10/2020
 * Time: 15:23
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
    /// Description of ShowRanorexProgressDialog.
    /// </summary>
    [TestModule("4F56AACD-0736-4E8A-AA31-E44C09836A74", ModuleType.UserCode, 1)]
    public class ShowRanorexProgressDialog : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ShowRanorexProgressDialog()
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
        	Ranorex.Controls.ProgressForm.Show();
        }
    }
}
