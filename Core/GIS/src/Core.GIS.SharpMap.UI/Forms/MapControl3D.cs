using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DelftTools.Utils.Aop;
using IrrlichtNETCP;
using IrrlichtNETCP.Inheritable;
using log4net;
using Color=IrrlichtNETCP.Color;

namespace SharpMap.UI.Forms
{
    public partial class MapControl3D : UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapControl3D)); 
        
        private IrrlichtDevice device;
        private TerrainSceneNode terrain;
        private CameraSceneNode camera;

        private Thread animationThread;

        public MapControl3D()
        {
            InitializeComponent();
            animationThread = new Thread(Initialize3D);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            Focus();
            if (Visible && !animationThread.IsAlive)
            {
                animationThread.Start();
            }
        }

        public void Initialize3D()
        {
            device = new IrrlichtDevice(DriverType.Direct3D9, /* TODO: for Linux/OSX it should be OpenGL */
                new Dimension2D(800, 600), 32, false, true, true, true, GetHandle());
            device.FileSystem.WorkingDirectory = "3d";
            device.Resizeable = true;

            // setup a simple 3d scene
            SceneManager sceneManager = device.SceneManager;

            VideoDriver driver = device.VideoDriver;

            camera = sceneManager.AddCameraSceneNodeFPS(null, 100.0f, 50000.0f, false);
            camera.Position = new Vector3D(1900 * 2, 255 * 2, 3700 * 2);
            camera.Target = new Vector3D(2397 * 2, 343 * 2, 2700 * 2);
            camera.FarValue = 12000.0f;

            camera.InputReceiverEnabled = false;

            /*
            Here comes the terrain renderer scene node: We add it just like any 
            other scene node to the scene using ISceneManager::addTerrainSceneNode(). 
            The only parameter we use is a file name to the heightmap we use. A heightmap
            is simply a gray scale texture. The terrain renderer loads it and creates 
            the 3D terrain from it.
            To make the terrain look more big, we change the scale factor of it to (40, 4.4, 40).
            Because we don't have any dynamic lights in the scene, we switch off the lighting,
            and we set the file terrain-texture.jpg as texture for the terrain and 
            detailmap3.jpg as second texture, called detail map. At last, we set
            the scale values for the texture: The first texture will be repeated only one time over 
            the whole terrain, and the second one (detail map) 20 times. 
            */

            terrain = sceneManager.AddTerrainSceneNode("terrain-heightmap.bmp",
                null,                               // parent node
                -1,									// node id
                new Vector3D(0f, 0f, 0f),			// position
                new Vector3D(0f, 0f, 0f),			// rotation
                new Vector3D(40f, 4.4f, 40f),		// scale
                new Color(255, 255, 255, 255),	    // vertexColor,
                5,									// maxLOD
                TerrainPatchSize.TPS17				// patchSize
            );

            terrain.SetMaterialFlag(MaterialFlag.Lighting, false);
            terrain.SetMaterialTexture(0, driver.GetTexture("terrain-texture.jpg"));
            terrain.SetMaterialTexture(1, driver.GetTexture("detailmap3.jpg"));
            terrain.SetMaterialType(MaterialType.DetailMap);
            terrain.ScaleTexture(1.0f, 20.0f);
            //terrain->setDebugDataVisible ( true );

            /*
            To be able to do collision with the terrain, we create a triangle selector.
            If you want to know what triangle selectors do, just take a look into the 
            collision tutorial. The terrain triangle selector works together with the
            terrain. To demonstrate this, we create a collision response animator 
            and attach it to the camera, so that the camera will not be able to fly 
            through the terrain.
            */

            // create triangle selector for the terrain	
            TriangleSelector selector = sceneManager.CreateTerrainTriangleSelector(terrain, 0);
            terrain.TriangleSelector = selector;

            // create collision response animator and attach it to the camera
            Animator animator = sceneManager.CreateCollisionResponseAnimator(selector, camera, new Vector3D(60, 100, 60), new Vector3D(0, 0, 0), new Vector3D(0, 50, 0), 0);
            selector.Dispose();
            camera.AddAnimator(animator);
            animator.Dispose(); ;

            /*
            To make the user be able to switch between normal and wireframe mode, we create
            an instance of the event reciever from above and let Irrlicht know about it. In 
            addition, we add the skybox which we already used in lots of Irrlicht examples.
            */

            // create skybox
            driver.SetTextureFlag(TextureCreationFlag.CreateMipMaps, false);

            sceneManager.AddSkyBoxSceneNode(null, new Texture[] {
                driver.GetTexture("irrlicht2_up.jpg"),
                driver.GetTexture("irrlicht2_dn.jpg"),
                driver.GetTexture("irrlicht2_lf.jpg"),
                driver.GetTexture("irrlicht2_rt.jpg"),
                driver.GetTexture("irrlicht2_ft.jpg"),
                driver.GetTexture("irrlicht2_bk.jpg")
            }, -1);

            while (device.Run())
            {
                driver.BeginScene(true, true, new Color());
                sceneManager.DrawAll();
                driver.EndScene();
            }

            device.Dispose();

            return;
        }

        private delegate IntPtr GetHandleDelegate();
        private IntPtr GetHandle()
        {
            if(InvokeRequired)
            {
                GetHandleDelegate getHandleDelegate = GetHandle;
                return (IntPtr) Invoke(getHandleDelegate, null);
            }
            else
            {
                return Handle;
            }
        }

        private void MapControl3D_MouseDown(object sender, MouseEventArgs e)
        {
            camera.InputReceiverEnabled = true;
            device.CursorControl.Visible = false;
        }

        private void MapControl3D_MouseUp(object sender, MouseEventArgs e)
        {
            camera.InputReceiverEnabled = false;
            device.CursorControl.Visible = true;
        }

        private void MapControl3D_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void MapControl3D_KeyDown(object sender, KeyEventArgs e)
        {
            Vector3D oldPosition = camera.Position;
            switch (e.KeyCode)
            {
                case Keys.A:
                    camera.Position = new Vector3D(oldPosition.X + camera.MoveSpeed, oldPosition.Y, oldPosition.Z);
                    break;
                case Keys.D:
                    camera.Position = new Vector3D(oldPosition.X - camera.MoveSpeed, oldPosition.Y, oldPosition.Z);
                    break;
                case Keys.W:
                    camera.Position = new Vector3D(oldPosition.X, oldPosition.Y, oldPosition.Z - camera.MoveSpeed);
                    break;
                case Keys.S:
                    camera.Position = new Vector3D(oldPosition.X, oldPosition.Y, oldPosition.Z + camera.MoveSpeed);
                    break;
                case Keys.Space:
                    terrain.SetMaterialFlag(MaterialFlag.Wireframe, !terrain.GetMaterial(0).Wireframe);
                    terrain.SetMaterialFlag(MaterialFlag.PointCloud, false);
                    break;
                default:
                    break;
            }
        }

        private void MapControl3D_Resize(object sender, EventArgs e)
        {
        }
    }
}


