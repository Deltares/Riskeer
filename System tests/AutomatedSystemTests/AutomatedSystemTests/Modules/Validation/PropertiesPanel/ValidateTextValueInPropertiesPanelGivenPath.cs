/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 25/03/2022
 * Time: 17:37
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
using Ranorex_Automation_Helpers.UserCodeCollections;

namespace AutomatedSystemTests.Modules.Validation.PropertiesPanel
{
    /// <summary>
    /// Description of ValidateTextValueInPropertiesPanelGivenPath.
    /// </summary>
    [TestModule("CC8DA82F-905A-4414-9AD6-BB42BBEA84FC", ModuleType.UserCode, 1)]
    public class ValidateTextValueInPropertiesPanelGivenPath : ITestModule
    {
        
        string _pathToRowInPP = "";
        [TestVariable("9ce9e644-2868-4a3f-9a74-bc4f92445dc1")]
        public string pathToRowInPP
        {
            get { return _pathToRowInPP; }
            set { _pathToRowInPP = value; }
        }
        
        
        string _expectedText = "";
        [TestVariable("25313ec3-4407-4a1c-bfbb-328d6c0f6b23")]
        public string expectedText
        {
            get { return _expectedText; }
            set { _expectedText = value; }
        }
        
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateTextValueInPropertiesPanelGivenPath()
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
            var table = repo.RiskeerMainWindow.ContainerMultipleViews.PropertiesPanelContainer.Table.SelfInfo.CreateAdapter<Table>(true);
            var row = PropertiesPanelHelpers.GetRowInPropertiesPanelGivenPath(pathToRowInPP, table);
            string currentValue = row.Element.GetAttributeValueText("AccessibleValue");
            Validate.AreEqual(currentValue, expectedText);
        }
    }
}
