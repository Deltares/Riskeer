/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 09/11/2020
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
using Jitbit.Utils;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.IO
{
    /// <summary>
    /// Description of ExportPropertiesPanelToCsv.
    /// </summary>
    [TestModule("0024B22F-D58B-4EF0-A67D-463153071618", ModuleType.UserCode, 1)]
    public class ExportPropertiesPanelToCsv : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ExportPropertiesPanelToCsv()
        {
            // Do not delete - a parameterless constructor is required!
        }

       
       string _fileName = "";
       [TestVariable("5242fb5c-2319-400b-8d05-ca4fb3963db5")]
       public string fileName
       {
           get { return _fileName; }
           set { _fileName = value; }
       }
       
       string _separationCharacter = "";
       [TestVariable("5485f5bd-8954-4627-887f-89283106e308")]
       public string separationCharacter
       {
           get { return _separationCharacter; }
           set { _separationCharacter = value; }
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
            Delay.SpeedFactor = 0;
            
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            Adapter propertiesPanelAdapter = myRepository.RiskeerMainWindow.PropertiesPanelContainer.Table.Self;
            
            var allRows = propertiesPanelAdapter.As<Table>().Rows.ToList();
            var myExport = new CsvExport(separationCharacter, false,true);
            
            int index = 0;
            foreach (Ranorex.Row row in allRows) {
                myExport.AddRow();
                myExport["indexRow"] = index.ToString();
                myExport["AccessibleName"] = row.Element.GetAttributeValueText("AccessibleName");
                myExport["AccessibleValue"] = row.Element.GetAttributeValueText("AccessibleValue");
                index++;
            }
            myExport.ExportToFile(fileName);
        }
    }
}
