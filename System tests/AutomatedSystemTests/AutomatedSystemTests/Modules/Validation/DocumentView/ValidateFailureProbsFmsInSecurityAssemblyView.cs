/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 16/05/2022
 * Time: 15:30
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
using System.Linq;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Validation.DocumentView
{
    /// <summary>
    /// Description of ValidateFailureProbsFmsInSecurityAssemblyView.
    /// </summary>
    [TestModule("4546E1AB-5EF1-40E7-9A39-721E1138E6B1", ModuleType.UserCode, 1)]
    public class ValidateFailureProbsFmsInSecurityAssemblyView : ITestModule
    {
        
        string _rowIndex = "";
        [TestVariable("e5c55790-6407-4907-bda1-2c128c6f0136")]
        public string rowIndex
        {
            get { return _rowIndex; }
            set { _rowIndex = value; }
        }
        
        
        string _nameFM = "";
        [TestVariable("333eac77-535c-428b-93ad-a1e0b56aee38")]
        public string nameFM
        {
            get { return _nameFM; }
            set { _nameFM = value; }
        }
        
        
        string _labelFM = "";
        [TestVariable("4e7ad43b-ea9b-4feb-b104-1ee6df065496")]
        public string labelFM
        {
            get { return _labelFM; }
            set { _labelFM = value; }
        }
        
        
        string _trajectAssessmentInformationString = "";
        [TestVariable("8746ea94-ac84-4c11-b481-665c6ad072a5")]
        public string trajectAssessmentInformationString
        {
            get { return _trajectAssessmentInformationString; }
            set { _trajectAssessmentInformationString = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateFailureProbsFmsInSecurityAssemblyView()
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
            var tableFailureProbsFMs = repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.SecurityAssemblyView.Table.Self;
            var trajectResultInformation = TrajectResultInformation.BuildAssessmenTrajectInformation(trajectAssessmentInformationString);
            var infoCurrentFM = trajectResultInformation.ListFMsResultInformation.Where(fmItem => fmItem.Label==labelFM).FirstOrDefault();
            var expectedFailureProbability = infoCurrentFM==null?"-":infoCurrentFM.FailureProbability.ToNoGroupSeparator();
            var row = tableFailureProbsFMs.Rows[Int32.Parse(rowIndex)-1];
            ValidateCell(row.Cells[1], nameFM.ReplacePathAliases());
            ValidateCell(row.Cells[2], labelFM);
            ValidateCell(row.Cells[3], expectedFailureProbability);
        }
        
        private void ValidateCell(Cell cellToValidate, string expectedContent)
        {
            cellToValidate.Focus();
            cellToValidate.Select();
            Validate.AreEqual(cellToValidate.Text.ToNoGroupSeparator(), expectedContent);
        }
    }
}
