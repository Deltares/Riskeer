/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 30/10/2020
 * Time: 12:12
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;
using Ranorex_Automation_Helpers.UserCodeCollections;

namespace AutomatedSystemTests.Modules.Set_Assign
{
    /// <summary>
    /// Description of SetValueParameterInPropertiesPanelGivenPathTypeAndNewValue.
    /// </summary>
    [TestModule("52D4C162-4D43-4281-9B87-3CF96D0FA894", ModuleType.UserCode, 1)]
    public class SetValueParameterInPropertiesPanelGivenPathTypeAndNewValue : ITestModule
    {
        
        string _pathToElementInPropertiesPanel = "";
        [TestVariable("a6c6752c-53a8-4e28-a7d5-e369855d34cb")]
        public string pathToElementInPropertiesPanel
        {
        	get { return _pathToElementInPropertiesPanel; }
        	set { _pathToElementInPropertiesPanel = value; }
        }
        
        
        string _typeParameter = "";
        [TestVariable("f6cc0a7d-f65c-4a45-8975-405678527d6a")]
        public string typeParameter
        {
        	get { return _typeParameter; }
        	set { _typeParameter = value; }
        }
        
        
        string _newValueForParameter = "";
        [TestVariable("57b81fcc-fdb5-4138-9a1d-0f85b3b8c8b6")]
        public string newValueForParameter
        {
        	get { return _newValueForParameter; }
        	set { _newValueForParameter = value; }
        }
        

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SetValueParameterInPropertiesPanelGivenPathTypeAndNewValue()
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
            row.SetValue(newValueForParameter, typeParameter);
        }
    }
}
