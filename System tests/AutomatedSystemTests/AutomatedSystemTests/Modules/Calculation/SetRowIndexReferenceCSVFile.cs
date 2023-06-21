/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 27/11/2020
 * Time: 12:29
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

namespace AutomatedSystemTests.Modules.Calculation
{
    /// <summary>
    /// Description of SetRowIndexReferenceCSVFile.
    /// </summary>
    [TestModule("73C238EC-C63E-43B3-B646-F6BFA96076B7", ModuleType.UserCode, 1)]
    public class SetRowIndexReferenceCSVFile : ITestModule
    {
        
        
        string _indecesMergedFMs = "";
        [TestVariable("8abe37d8-5faf-45be-96a5-edfef4d1bed2")]
        public string indecesMergedFMs
        {
            get { return _indecesMergedFMs; }
            set { _indecesMergedFMs = value; }
        }
        
        
        string _indexCurrentIterationInValidation = "";
        [TestVariable("8746b048-5dca-4d0a-b11d-79abd58e8436")]
        public string indexCurrentIterationInValidation
        {
            get { return _indexCurrentIterationInValidation; }
            set { _indexCurrentIterationInValidation = value; }
        }
        
        
        string _originalRowIndexReferenceCSV = "";
        [TestVariable("98332cf8-d5fc-471d-a107-de11fabee4d8")]
        public string originalRowIndexReferenceCSV
        {
            get { return _originalRowIndexReferenceCSV; }
            set { _originalRowIndexReferenceCSV = value; }
        }
        
        
        string _rowIndexReferenceCSV = "";
        [TestVariable("2c0340fb-270a-4816-bbc4-00a411012d8a")]
        public string rowIndexReferenceCSV
        {
            get { return _rowIndexReferenceCSV; }
            set { _rowIndexReferenceCSV = value; }
        }
        
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SetRowIndexReferenceCSVFile()
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
            int idxCurrentIteration = Int32.Parse(indexCurrentIterationInValidation);
            
            bool hbIteration = idxCurrentIteration>=1 && idxCurrentIteration<=13;
            bool stphIteration = idxCurrentIteration>=14 && idxCurrentIteration<=45;
            bool gekbIteration = idxCurrentIteration>=46 && idxCurrentIteration<=53;
            bool zstBskwIteration = idxCurrentIteration>=54 && idxCurrentIteration<=57;
                
            
            if (hbIteration) {
                // Hydraulic Boundary Condition (Water Level or Wave Height).
                // No modification of row index is required.
                rowIndexReferenceCSV = originalRowIndexReferenceCSV;
            } else if (stphIteration) {
                // Iteration is checking an item of Piping FM
                if (indecesMergedFMs.Contains("1")) {
                    // Piping has been merged (overwritten) by information in project B.
                    // Modification of row index is required.
                    rowIndexReferenceCSV = (Int32.Parse(originalRowIndexReferenceCSV) + 32).ToString();
                } else {
                    // Piping is original from project A
                    // No modification of row index is required.
                    rowIndexReferenceCSV = originalRowIndexReferenceCSV;
                }
            } else if (gekbIteration) {
                // Iteration is checking an item of GEKB FM
                if (indecesMergedFMs.Contains("2")) {
                    // GEKB has been merged (overwritten) by information in project B.
                    // Modification of row index is required.
                    rowIndexReferenceCSV = (Int32.Parse(originalRowIndexReferenceCSV) + 8).ToString();
                } else {
                    // GEKB is original from project A
                    // No modification of row index is required.
                    rowIndexReferenceCSV = originalRowIndexReferenceCSV;
                }
            } else if (zstBskwIteration) {
                // Iteration is checking an item of ZST or BSKW
                // No modification of row index is required.
                rowIndexReferenceCSV = originalRowIndexReferenceCSV;
            } else {
                Report.Log(ReportLevel.Error, "Index of row loop out of range: " + indexCurrentIterationInValidation);
                throw new Exception();
            }
                
        }
    }
}
