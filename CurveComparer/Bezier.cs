using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CurveComparer
{
    /// <summary>
    /// Adapted from http://en.wikipedia.org/wiki/B%C3%A9zier_curve#Higher-order_curves
    /// Approach seen seems to originate from De Casteljau's algorithm
    /// </summary>
    public class Bezier : CurveAlgorithm
    {
        public PointF[] MakeCurve(PointF[] source, int iterations)
        {
            if (source.Length == 0)
                return new PointF[0];
            if (source.Length == 1)
                return new PointF[]{source[0]};

            List<PointF> pts = new List<PointF>();
            for (int i = 0; i <= iterations + 1; i++)
            {
                float rat = (float)i / (iterations + 1);
                pts.Add(GetNextPoint(source, rat));
            }
            return pts.ToArray();
        }

        private PointF GetNextPoint(PointF[] source, float current_ratio)
        {
            if (source.Length == 2)
                return RatioOfPoints(source[0], source[1], current_ratio);
            else
            {
                // Get the points that are the current ratio along the current edges
                // Pass those as the new source into the next GetNextPoint
                PointF[] derived = new PointF[source.Length - 1];
                for (int i = 0; i < derived.Length; i++)
                {
                    derived[i] = RatioOfPoints(source[i], source[i + 1], current_ratio);
                }
                return GetNextPoint(derived, current_ratio);
            }
        }

        private PointF RatioOfPoints(PointF a, PointF b, float ratio)
        { return new PointF(a.X * ratio + b.X * (1 - ratio), a.Y * ratio + b.Y * (1 - ratio)); }
    }
}
