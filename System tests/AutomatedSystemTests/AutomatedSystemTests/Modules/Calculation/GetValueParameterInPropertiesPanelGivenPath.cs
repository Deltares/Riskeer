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
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0.0;
            
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            Adapter propertiesPanelAdapter = myRepository.RiskeerMainWindow.PropertiesPanelContainer.Table.Self;
            Ranorex.Row row = GetRowInPropertiesPanelGivenPath(propertiesPanelAdapter, pathToElementInPropertiesPanel);
            
            valueOfElement = row.Element.GetAttributeValueText("AccessibleValue");
        }
        
                public Ranorex.Row GetRowInPropertiesPanelGivenPath(Adapter argumentAdapter, string pathItem)
        	{
        	int minimumIndex = 0;
        	var stepsPathItem = pathItem.Split('>').ToList();
        	Ranorex.Row stepRow = argumentAdapter.As<Table>().Rows.ToList()[1];
        	for (int i=0; i < stepsPathItem.Count; i++) {
        			// Find the item corresponding to the step
        			var step = stepsPathItem[i];
        			var completeList = argumentAdapter.As<Table>().Rows.ToList();
        			var searchList = completeList.GetRange(minimumIndex, completeList.Count-minimumIndex);
        			var indexStepRow = searchList.FindIndex(rw => rw.GetAttributeValue<string>("AccessibleName").Contains(step));
        			stepRow = searchList[indexStepRow];

        			// if step is intermediate
        			if (i != stepsPathItem.Count - 1)
        				{
        			         var stateStepRow = stepRow.Element.GetAttributeValueText("AccessibleState");
        			         // if intermediate step is collapsed
        			         if (stateStepRow.Contains("Collapsed")) {
        			             // Select and expand the intermediate item
        			             Report.Log(ReportLevel.Info, "was collapsed");
        			             stepRow.Focus();
        			             stepRow.Select();
        			             stepRow.PressKeys("{Right}");
        			             }
        			     } 
        			     else
        			     {
        			    // Select the final item
        			    stepRow.Focus();
        			    stepRow.Select();
        			     }
        			// Update the minimum index administration (only search forward)
        			minimumIndex += 1 + indexStepRow;
        			}
        	return stepRow;
        	}

    }
}
