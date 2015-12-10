using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Utils.Drawing;
using Core.GIS.SharpMap.Extensions.Interop;
using Core.GIS.SharpMap.UI.Properties;

namespace Core.GIS.SharpMap.UI.Helpers
{
    public static class MapCursors
    {
        public static Cursor AddPoint;
        public static Cursor RemovePoint;
        public static Bitmap AddFeatureTemplateBitmap;

        private static readonly List<Cursor> CursorsToDispose;
        private static readonly List<IntPtr> IconsToDispose;

        static MapCursors()
        {
            CursorsToDispose = new List<Cursor>();
            IconsToDispose = new List<IntPtr>();
            AddPoint = CreateCursor(Resources.AddPoint, 0, 0);
            RemovePoint = CreateCursor(Resources.RemovePoint, 0, 0);
            AddFeatureTemplateBitmap = (Bitmap) Resources.AddFeatureTemplate.Clone();
        }

        public static Cursor CreateArrowOverlayCuror(Bitmap overlay, int xOffset = 12, int yOffset = 0, int width = 12, int height = 12, int xHotSpot = 0, int yHotSpot = 0)
        {
            var image = AddFeatureTemplateBitmap.AddOverlayImage(overlay, xOffset, yOffset, width, height);
            return CreateCursor(image, xHotSpot, yHotSpot);
        }

        public static Cursor CreateCursor(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            var tmpPtr = bmp.GetHicon();

            var iconInfo = new NativeWin32.IconInfo();
            NativeWin32.GetIconInfo(tmpPtr, ref iconInfo);

            iconInfo.xHotspot = xHotSpot;
            iconInfo.yHotspot = yHotSpot;
            iconInfo.fIcon = false;

            var iconPtr = NativeWin32.CreateIconIndirect(ref iconInfo);

            var cursor = new Cursor(iconPtr);

            CursorsToDispose.Add(cursor);
            IconsToDispose.Add(iconPtr);
            IconsToDispose.Add(tmpPtr);

            NativeWin32.DestroyIcon(iconInfo.hbmMask);
            NativeWin32.DestroyIcon(iconInfo.hbmColor);
            NativeWin32.DeleteObject(iconInfo.hbmMask);
            NativeWin32.DeleteObject(iconInfo.hbmColor);

            return cursor;
        }

        public static void DisposeCursors()
        {
            CursorsToDispose.ForEach(c => c.Dispose());
            IconsToDispose.ForEach(ptr => NativeWin32.DestroyIcon(ptr));

            CursorsToDispose.Clear();
            IconsToDispose.Clear();
        }
    }
}