using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Core.Components.DotSpatial.Properties;
using DotSpatial.Controls;
using log4net;

namespace Core.Components.DotSpatial
{
    public sealed class BaseMap : Control
    {
        private ICollection<string> data;
 
        public ICollection<string> Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
                if (data != null)
                {
                    LoadData();
                }
            }
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(BaseMap));
        private Map map;

        public BaseMap()
        {
            InitializeMapView();
        }

        /// <summary>
        /// Initialize the <see cref="Map"/> for the <see cref="BaseMap"/>
        /// </summary>
        private void InitializeMapView()
        {
            data = new List<string>();
            map = new Map
            {
                Dock = DockStyle.Fill,
                FunctionMode = FunctionMode.Pan,
            };
            Controls.Add(map);
        }

        /// <summary>
        /// Loads the data from the files given in <see cref="data"/> and shows them on the <see cref="Map"/>
        /// </summary>
        private void LoadData()
        {
            try
            {
                foreach (string fileName in data)
                {
                    map.AddLayer(fileName);
                }
            }
            catch (ApplicationException applicationException)
            {
                log.Error(Resources.BaseMap_LoadData_Cannot_open_file_extension);
            }
            catch (FileNotFoundException fileException)
            {
                log.ErrorFormat(Resources.BaseMap_LoadData_File_loading_failded__The_file__0__could_not_be_found, fileException.FileName);
            }
        }
    }
}