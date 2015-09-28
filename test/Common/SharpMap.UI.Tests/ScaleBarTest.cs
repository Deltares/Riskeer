using System.Drawing;
using System.Windows.Forms;
using DelftTools.TestUtils;
using NUnit.Framework;
using SharpMap.UI.Tools.Decorations;

namespace SharpMap.UI.Tests
{
    [TestFixture]
    public class ScaleBarTest
    {
        /// <summary>
        /// This image shows a short bit of text with all 9 different alignment combinations around 9 different points.
        /// The alignment point is at the intersection of the blue lines. The code is below:
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="ctrl"></param>
        private static void DrawPointText(Graphics graphics, Control ctrl)
        {
            float xDelta = ctrl.ClientRectangle.Size.Width/4f;
            float yDelta = ctrl.ClientRectangle.Size.Height/4f;
            var f = new StringFormat();

            graphics.DrawLine(Pens.Blue, xDelta, 0f, xDelta,
                              ctrl.ClientRectangle.Height);
            graphics.DrawLine(Pens.Blue, xDelta*2f, 0f, xDelta*2f,
                              ctrl.ClientRectangle.Height);
            graphics.DrawLine(Pens.Blue, xDelta*3f, 0f, xDelta*3f,
                              ctrl.ClientRectangle.Height);

            graphics.DrawLine(Pens.Blue, 0, yDelta,
                              ctrl.ClientRectangle.Width, yDelta);
            graphics.DrawLine(Pens.Blue, 0, yDelta*2f,
                              ctrl.ClientRectangle.Width, yDelta*2f);
            graphics.DrawLine(Pens.Blue, 0, yDelta*3f,
                              ctrl.ClientRectangle.Width, yDelta*3f);

            f.Alignment = StringAlignment.Near;
            f.LineAlignment = StringAlignment.Near;
            graphics.DrawString("Near, Near Line", ctrl.Font, Brushes.Black,
                                xDelta*3, yDelta, f);
            f.LineAlignment = StringAlignment.Center;
            graphics.DrawString("Near, Center Line", ctrl.Font, Brushes.Black,
                                xDelta*3, yDelta*2, f);
            f.LineAlignment = StringAlignment.Far;
            graphics.DrawString("Near, Far Line", ctrl.Font, Brushes.Black,
                                xDelta*3, yDelta*3, f);

            f.Alignment = StringAlignment.Center;
            f.LineAlignment = StringAlignment.Near;
            graphics.DrawString("Center, Near Line", ctrl.Font, Brushes.Black,
                                xDelta*2, yDelta, f);
            f.LineAlignment = StringAlignment.Center;
            graphics.DrawString("Center, Center Line", ctrl.Font, Brushes.Black,
                                xDelta*2, yDelta*2f, f);
            f.LineAlignment = StringAlignment.Far;
            graphics.DrawString("Center, Far Line", ctrl.Font, Brushes.Black,
                                xDelta*2, yDelta*3f, f);

            f.Alignment = StringAlignment.Far;
            f.LineAlignment = StringAlignment.Near;
            graphics.DrawString("Far, Near Line", ctrl.Font, Brushes.Black,
                                xDelta, yDelta, f);
            f.LineAlignment = StringAlignment.Center;
            graphics.DrawString("Far, Center Line", ctrl.Font, Brushes.Black,
                                xDelta, yDelta*2f, f);
            f.LineAlignment = StringAlignment.Far;
            graphics.DrawString("Far, Far Line", ctrl.Font, Brushes.Black,
                                xDelta, yDelta*3f, f);
        }

        /// <summary>
        /// The other nine combinations are created by aligning the text inside a bounding rectangle. 
        /// This image shows the same text aligned inside the window rectangle, using all 9 alignment combinations.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="control"></param>
        private static void DrawRectText(Graphics graphics, Control control)
        {
            var f = new StringFormat();
            var bounds = new RectangleF(
                control.ClientRectangle.X,
                control.ClientRectangle.Y,
                control.ClientRectangle.Width,
                control.ClientRectangle.Height);

            f.Alignment = StringAlignment.Near;
            f.LineAlignment = StringAlignment.Near;
            graphics.DrawString("Near, Near", control.Font, Brushes.Black,
                                bounds, f);

            f.Alignment = StringAlignment.Center;
            f.LineAlignment = StringAlignment.Near;
            graphics.DrawString("Center, Near", control.Font, Brushes.Black,
                                bounds, f);

            f.Alignment = StringAlignment.Far;
            f.LineAlignment = StringAlignment.Near;
            graphics.DrawString("Far, Near", control.Font, Brushes.Black,
                                bounds, f);

            f.Alignment = StringAlignment.Near;
            f.LineAlignment = StringAlignment.Center;
            graphics.DrawString("Near, Center", control.Font, Brushes.Black,
                                bounds, f);

            f.Alignment = StringAlignment.Center;
            f.LineAlignment = StringAlignment.Center;
            graphics.DrawString("Center, Center", control.Font, Brushes.Black,
                                bounds, f);

            f.Alignment = StringAlignment.Far;
            f.LineAlignment = StringAlignment.Center;
            graphics.DrawString("Far, Center", control.Font, Brushes.Black,
                                bounds, f);

            f.Alignment = StringAlignment.Near;
            f.LineAlignment = StringAlignment.Far;
            graphics.DrawString("Near, Far", control.Font, Brushes.Black,
                                bounds, f);

            f.Alignment = StringAlignment.Center;
            f.LineAlignment = StringAlignment.Far;
            graphics.DrawString("Center, Far", control.Font, Brushes.Black,
                                bounds, f);

            f.Alignment = StringAlignment.Far;
            f.LineAlignment = StringAlignment.Far;
            graphics.DrawString("Far, Far", control.Font, Brushes.Black,
                                bounds, f);
        }

    private static void DrawRectangles(Graphics g)
    {
      g.PageUnit=GraphicsUnit.Pixel;
      var p=new Pen(Color.Black, 3); //this pen will be 3 pixels wide
      g.DrawRectangle(p,10,10,200,100); //draw a rectangle in Pixel mode (the default)
      p.Dispose();

      g.PageUnit=GraphicsUnit.Inch;
      p=new Pen(Color.Blue,0.05f); //this pen will be 1/20th of an inch wide
      g.DrawRectangle(p,0.1f,1.5f,4f,1f); // draw a rectangle 4" by 1"
      p.Dispose();

      g.PageUnit=GraphicsUnit.Millimeter;
      p=new Pen(Color.Green,1f); //this pen will be 1 millimeter wide
      g.DrawRectangle(p,4f,80f,80f,60f); // draw a rectangle 80 by 60 mm
      p.Dispose();
    }

    [Test, Ignore("just for demonstration purposes")]
    public void DrawRectanglesForDifferentUnits()
    {
        using (var control = new UserControl())
        {
            control.Paint +=
                ((sender, e) => DrawRectangles(e.Graphics));
            WindowsFormsTestHelper.ShowModal(control);
        }
    }

        [Test, Ignore("just for demonstration purposes")]
        public void DrawTextInRectangle()
        {
            using (var control = new UserControl())
            {
                control.Paint +=
                    ((sender, e) => DrawRectText(e.Graphics, (UserControl) sender));
                WindowsFormsTestHelper.ShowModal(control);
            }
        }

        [Test, Ignore("just for demonstration purposes")]
        public void DrawTextNearPoint()
        {
            using (var control = new UserControl())
            {
                control.Paint +=
                    ((sender, e) => DrawPointText(e.Graphics, (UserControl) sender));
                WindowsFormsTestHelper.ShowModal(control);
            }
        }

        [Test]
        public void ToFormattedString()
        {
            double value = 1520000;
            Assert.AreEqual(1.5.ToString()+"E+6", ScaleBar.ToFormattedString(value, 0.000001));

            value = .0000012344;
            Assert.AreEqual(1.2.ToString() + "E-6", ScaleBar.ToFormattedString(value, 0.000001));

            value = 100.2298234048243240294234320948234324032984;
            Assert.AreEqual(100.23.ToString(), ScaleBar.ToFormattedString(value, 0.000001));


            value = 1e10;
            Assert.AreEqual("1E+10", ScaleBar.ToFormattedString(value, 0.000001));
        }
    }
}