using System.Drawing;
using System.Net;
using System.Windows.Forms;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DeltaShell.Plugins.SharpMapGis.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Commands;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap;

namespace DeltaShell.Plugins.SharpMapGis.Tests.Commands
{
    [TestFixture]
    public class CommandTests
    {
        private readonly MockRepository mocks = new MockRepository();
        private static readonly ILog log = LogManager.GetLogger(typeof (CommandTests));

        [Test]
        [Ignore("Don't enable this test on build server until reason is found why it can't connect (hangs) probably something with security of .NET")]
        public void AddLayerFromExternalSource()
        {
            MapView mapView = GetMapView();
            var mapAddWmsLayerCommand = new MapAddWmsLayerCommand();

            Assert.AreEqual(0, mapView.Map.Layers.Count);

            string url = "http://www2.demis.nl/wms/wms.asp?wms=WorldMap&REQUEST=GetCapabilities";

            var webBrowser = new WebBrowser();
            bool connectionAvailable = false;
            connectionAvailable = ConnectionAvailable(url);

            webBrowser.DocumentCompleted += delegate
                                                {
                                                    mapAddWmsLayerCommand.AddLayerFromExternalSource(url);
                                                    Assert.AreEqual(1, mapView.Map.Layers.Count);
                                                };
            if (connectionAvailable)
            {
                webBrowser.Navigate(url);
            }
         
            Assert.AreEqual(true, connectionAvailable);
            if (!connectionAvailable)
            {
                log.DebugFormat("No internet connection");
            }
        }

        #region private methods
        private bool ConnectionAvailable(string strUrl)
        {
            try
            {
                var reqFP = (HttpWebRequest)WebRequest.Create(strUrl);
                var rspFP = (HttpWebResponse)reqFP.GetResponse();

                if (HttpStatusCode.OK == rspFP.StatusCode)
                {
                    // HTTP = 200 - Internet connection available, server online
                    rspFP.Close();
                    return true;
                }
                else
                {
                    // Other status - Server or connection not available
                    rspFP.Close();
                    return false;
                }
            }
            catch (WebException)
            {
                // Exception - connection not available
                return false;
            }
        }

        private MapView GetMapView()
        {
            var mapView = new MapView();
 
            mapView.Data = new Map(new Size(1, 1));

            //stubs
            var gui = mocks.Stub<IGui>();
            var viewManager = mocks.Stub<IViewList>();

            Expect.Call(gui.ToolWindowViews).Return(viewManager).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(viewManager).Repeat.Any();
 
            mocks.ReplayAll();

            new SharpMapGisGuiPlugin { Gui = gui }; // sets private gui field

            viewManager.ActiveView = mapView;

            return mapView;
        }
        #endregion
    }
}