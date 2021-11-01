/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 18/12/2020
 * Time: 18:11
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
using Ranorex_Automation_Helpers.UserCodeCollections;

namespace AutomatedSystemTests.Modules.Calculation
{
    /// <summary>
    /// Description of GetValueParameterInPropertiesPanelGivenPath.
    /// </summary>
    [TestModule("C77B1704-AEA4-41DB-90C0-A00BACB9CFFC", ModuleType.UserCode, 1)]
    public class GetValueParameterInPropertiesPanelGivenPath : ITestModule
    {
        
        
        string _pathToElementInPropertiesPanel = "";
        [TestVariable("717108e0-7b86-418f-9953-a968ec4f5bef")]
        public string pathToElementInPropertiesPanel
        {
            get { return _pathToElementInPropertiesPanel; }
            set { _pathToElementInPropertiesPanel = value; }
        }
        
        
        string _valueOfElement = "";
        [TestVariable("b8b2d988-b9c5-4a36-b47d-a548cdc8bb5e")]
        public string valueOfElement
        {
            get { return _valueOfElement; }
            set { _valueOfElement = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GetValueParameterInPropertiesPanelGivenPath()
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
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            Adapter propertiesPanelAdapter = myRepository.RiskeerMainWindow.ContainerMultipleViews.PropertiesPanelContainer.Table.Self;

            Ranorex.Row row = PropertiesPanelHelpers.GetRowInPropertiesPanelGivenPath(pathToElementInPropertiesPanel, propertiesPanelAdapter);
            valueOfElement = row.Element.GetAttributeValueText("AccessibleValue");
        }
    }
}
