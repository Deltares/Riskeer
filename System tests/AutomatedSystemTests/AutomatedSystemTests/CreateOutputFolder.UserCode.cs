﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// Your custom recording code should go in this file.
// The designer will only add methods to this file, so your custom code won't be overwritten.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests
{
    public partial class CreateOutputFolder
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void CreateFolderInForCaseOutputDrive(string caseNumber)
        {
            // Specify the directory you want to manipulate.
        //string path = @"Y:\script" + caseNumber;
        string path = Directory.GetCurrentDirectory() + @"\script" + caseNumber;
        try
        {
            // Determine whether the folder exists.
            if (Directory.Exists(path))
            {
                Report.Log(ReportLevel.Info, "Path " + path + " already exists.");
                return;
            }
            // Try to create the folder.
            DirectoryInfo di = Directory.CreateDirectory(path);
            Report.Log(ReportLevel.Info, "The folder " + path + " was created successfully at " + Directory.GetCreationTime(path) + ".");
        }
        catch (Exception e)
        {
            Report.Log(ReportLevel.Error, "Creating the folder " + path + " failed: {0}", e.ToString());
        }
        finally {}
        }

    }
}
