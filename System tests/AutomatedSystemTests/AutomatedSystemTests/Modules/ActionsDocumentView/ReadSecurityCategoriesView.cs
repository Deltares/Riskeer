/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 17/05/2022
 * Time: 18:02
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
using Ranorex_Automation_Helpers.UserCodeCollections;
using Newtonsoft.Json;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ReadSecurityCategoriesView.
    /// </summary>
    [TestModule("E6D61DE3-5921-45EA-8FA4-C205BD53333B", ModuleType.UserCode, 1)]
    public class ReadSecurityCategoriesView : ITestModule
    {
        
        string _trajectAssessmentInformationString = "";
        [TestVariable("6e24e9ab-a7df-4e1a-af7d-dde7952cf206")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformationString; }
            set { _trajectAssessmentInformationString = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReadSecurityCategoriesView()
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
            var trajectAssessmentInformation = TrajectResultInformation.BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            var tableRows = AutomatedSystemTestsRepository.Instance.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.SecurityCategoriesViewTable.Rows;
            tableRows.RemoveAt(0);
            foreach (var row in tableRows) {
                var cellUpperLimit = row.Cells[4];
                cellUpperLimit.Focus();
                cellUpperLimit.Select();
                trajectAssessmentInformation.UpperLimitsSecurityBoundaries.Add(cellUpperLimit.Text.ToNoGroupSeparator());
            }
            trajectAssessmentInformationString = JsonConvert.SerializeObject(trajectAssessmentInformation, Formatting.Indented);
        }
    }
}
