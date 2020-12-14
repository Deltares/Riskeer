/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 12/12/2020
 * Time: 14:48
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Newtonsoft.Json;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ReadDataScenariosView.
    /// </summary>
    [TestModule("0D889A2D-1464-4456-80E0-2EE96C49B09F", ModuleType.UserCode, 1)]
    public class ReadDataScenariosView : ITestModule
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReadDataScenariosView()
        {
            // Do not delete - a parameterless constructor is required!
        }

        
        string _jsonDataScenariosView = "";
        [TestVariable("d04c8141-7da9-4856-aea5-95ee1eb504ca")]
        public string jsonDataScenariosView
        {
            get { return _jsonDataScenariosView; }
            set { _jsonDataScenariosView = value; }
        }
        
        string _sectionIndex = "";
        [TestVariable("1dead42b-c412-4ba4-8f93-8fba0973a33d")]
        public string sectionIndex
        {
            get { return _sectionIndex; }
            set { _sectionIndex = value; }
        }
        
        string _sectionname = "";
        [TestVariable("9aef14ad-e7bf-4776-95d6-c677bf3240de")]
        public string sectionname
        {
            get { return _sectionname; }
            set { _sectionname = value; }
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
            
            var repo =global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
            var dataSectionScenariosView = BuildDataScenariosView(jsonDataScenariosView);
            

            var table = repo.RiskeerMainWindow.DocumentViewContainerUncached.ScenariosView.Table.Self;
            var rows = table.Rows;
            
            var rowHeader = rows[0];
            int indexInFinalRating = GetIndex(rowHeader, "In oordeel");
            int indexContribution = GetIndex(rowHeader, "Bijdrage aan");
            int indexName = GetIndex(rowHeader, "Naam");
            int indexFailureProbability = GetIndex(rowHeader, "Faalkans");

            var dataView = new DataSectionScenariosView();
            rows.RemoveAt(0);
            foreach (var row in rows) {
                var calcInfo = new CalculationInformationInScenariosView();
                Cell currentCell;
                
                currentCell = row.Cells[indexInFinalRating];
                currentCell.Select();
                calcInfo.InFinalRating = GetAV(currentCell)=="True";
                
                currentCell = row.Cells[indexContribution];
                currentCell.Select();
                calcInfo.Contribution = Double.Parse(GetAV(currentCell), currentCulture);
                
                currentCell = row.Cells[indexName];
                currentCell.Select();
                calcInfo.Name = GetAV(currentCell);
                
                currentCell = row.Cells[indexFailureProbability];
                currentCell.Select();
                calcInfo.FailureProbability = GetAV(currentCell);
                
                dataView.DataScenariosViewList.Add(calcInfo);
            }
            jsonDataScenariosView = JsonConvert.SerializeObject(dataView, Formatting.Indented);
        }
        
        private string GetAV(Cell cl)
        {
            return cl.Element.GetAttributeValueText("AccessibleValue");
        }
        
        private int GetIndex(Row row, string name)
        {
            return row.Cells.ToList().FindIndex(cl=>GetAV(cl).Contains(name));
        }
        
        private List<DataFMScenariosView> BuildDataScenariosView(string jsonString)
        {
            List<DataFMScenariosView> dataFMScenariosView;
            if (jsonString=="") {
                dataFMScenariosView = new List<DataFMScenariosView>();
            } else {
                var error = false;
                dataFMScenariosView = JsonConvert.DeserializeObject<List<DataFMScenariosView>>(jsonString, new JsonSerializerSettings
                {
                    Error = (s, e) =>
                    {
                        error = true;
                        e.ErrorContext.Handled = true;
                    }
                }
            );
                if (error==true) {
                    
                    Report.Log(ReportLevel.Error, "error unserializing json string for data scenarios view: " + jsonString);
                }
                
            }
            return dataFMScenariosView;
        }
        
    }
    
    public class DataFMScenariosView
    {
        public int IndexSection {get; set;}
        public string Name {get; set;}
        public DataSectionScenariosView DataSection {get; set;}
    }
    
    public class DataSectionScenariosView
    {
        public List<CalculationInformationInScenariosView> DataScenariosViewList {get; set;}
        
        public DataSectionScenariosView()
        {
            this.DataScenariosViewList = new List<CalculationInformationInScenariosView>();
        }
    }
    
    public class CalculationInformationInScenariosView
    {
        public bool InFinalRating {get; set;}
        public double Contribution {get; set;}
        public string Name {get; set;}
        public string FailureProbability {get; set;}
    }
}
