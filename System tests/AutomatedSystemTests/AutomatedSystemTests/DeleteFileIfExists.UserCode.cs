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
    public partial class DeleteFileIfExists
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void DeleteFileIfItCanBeFound(string fileToDelete)
        {
            try 
            {
            	// Check if file exists with its full path
            	
				if (File.Exists(fileToDelete))
					{
						// If file found, delete it
						File.Delete(fileToDelete);
						Report.Log(ReportLevel.Info, "File " + fileToDelete + " has been deleted.");
				} else {
					Report.Log(ReportLevel.Info, "File " + fileToDelete + " could not be found to be deleted.");
				}
			} catch {
				Report.Log(ReportLevel.Warn, "File " + fileToDelete + " was found but coudn't be deleted.");    
			}    
        }
    }
}
