using System.Drawing;

namespace SharpMap.Rendering
{
    public static class LineRectangleClipHelper
    {
        //source: http://en.wikipedia.org/wiki/Cohen–Sutherland_algorithm

        const int INSIDE = 0; // 0000
        const int LEFT = 1;   // 0001
        const int RIGHT = 2;  // 0010
        const int BOTTOM = 4; // 0100
        const int TOP = 8;    // 1000
 
        // Compute the bit code for a point (x, y) using the clip rectangle
        // bounded diagonally by (xmin, ymin), and (xmax, ymax)
 
        // ASSUME THAT xmax , xmin , ymax and ymin are global constants.

        private static int ComputeOutCode(float x, float y, RectangleF clip)
        {
            int code;
 
            code = INSIDE;          // initialised as being inside of clip window
 
            if (x < clip.Left)           // to the left of clip window
                code |= LEFT;
            else if (x > clip.Right)      // to the right of clip window
                code |= RIGHT;
            if (y < clip.Bottom)           // below the clip window
                code |= BOTTOM;
            else if (y > clip.Top)      // above the clip window
                code |= TOP;
 
            return code;
        }
 
        // Cohen–Sutherland clipping algorithm clips a line from
        // P0 = (x0, y0) to P1 = (x1, y1) against a rectangle with 
        // diagonal from (xmin, ymin) to (xmax, ymax).
        public static bool CohenSutherlandLineClip(ref float x0, ref float y0, ref float x1, ref float y1, RectangleF clip)
        {
            // compute outcodes for P0, P1, and whatever point lies outside the clip rectangle
            int outcode0 = ComputeOutCode(x0, y0, clip);
            int outcode1 = ComputeOutCode(x1, y1, clip);
            bool accept = false;
 
            while (true)
            {
                if ((outcode0 | outcode1)==0) // Bitwise OR is 0. Trivially accept and get out of loop
                { 
                    accept = true;
                    break;
                }
                if ((outcode0 & outcode1) > 0) // Bitwise AND is not 0. Trivially reject and get out of loop
                { 
                    break;
                }
                // failed both tests, so calculate the line segment to clip
                // from an outside point to an intersection with clip edge
                float x, y;
 
                // At least one endpoint is outside the clip rectangle; pick it.
                int outcodeOut = outcode0 > 0 ? outcode0 : outcode1;
 
                // Now find the intersection point;
                // use formulas y = y0 + slope * (x - x0), x = x0 + (1 / slope) * (y - y0)
                if ((outcodeOut & TOP)>0) {           // point is above the clip rectangle
                    x = x0 + (x1 - x0) * (clip.Top - y0) / (y1 - y0);
                    y = clip.Top;
                } 
                else if ((outcodeOut & BOTTOM)>0) { // point is below the clip rectangle
                    x = x0 + (x1 - x0) * (clip.Bottom - y0) / (y1 - y0);
                    y = clip.Bottom;
                } 
                else if ((outcodeOut & RIGHT)>0) {  // point is to the right of clip rectangle
                    y = y0 + (y1 - y0) * (clip.Right - x0) / (x1 - x0);
                    x = clip.Right;
                }
                else // if ((outcodeOut & LEFT) > 0)
                {   // point is to the left of clip rectangle
                    y = y0 + (y1 - y0) * (clip.Left - x0) / (x1 - x0);
                    x = clip.Left;
                }
 
                //NOTE:*****************************************************************************************
 
                /* if you follow this algorithm exactly(at least for c#), then you will fall into an infinite loop 
                        in case a line crosses more than two segments. to avoid that problem, leave out the last else
                        if(outcodeOut & LEFT) and just make it else*/
 
                //**********************************************************************************************
 
                // Now we move outside point to intersection point to clip
                // and get ready for next pass.
                if (outcodeOut == outcode0) {
                    x0 = x;
                    y0 = y;
                    outcode0 = ComputeOutCode(x0, y0, clip);
                } else {
                    x1 = x;
                    y1 = y;
                    outcode1 = ComputeOutCode(x1, y1, clip);
                }
            }
            return accept;
        }
    }
}