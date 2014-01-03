using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CurveComparer
{
    /// <summary>
    /// Based on B-Splines implemented below
    /// Duplicates last and first two points in order to create a continuous loop
    /// </summary>
    public class BSplineAutoLoop : BSpline
    {
        public override PointF[] MakeCurve(PointF[] source, int iterations)
        {
            if (source.Length == 0)
                return new PointF[0];
            if (source.Length == 1)
                return new PointF[] { source[0] };

            PointF[] mod_source = new PointF[source.Length + 3];
            Array.Copy(source, 0, mod_source, 1, source.Length);
            mod_source[0] = source[source.Length - 1];
            mod_source[mod_source.Length - 2] = source[0];
            mod_source[mod_source.Length - 1] = source[1];
            return base.MakeCurve(mod_source, iterations);
        }
    }

    /// <summary>
    /// Cubic B-Splines, using Catmull-Clark weighting (1/2-1/2 odd, 1/8-6/8-1/8 even)
    /// http://graphics.stanford.edu/courses/cs468-10-fall/LectureSlides/10_Subdivision.pdf
    /// See slide 25
    /// </summary>
    public class BSpline : CurveAlgorithm
    {
        public virtual PointF[] MakeCurve(PointF[] source, int iterations)
        {
            PointF[] cur_points = new PointF[source.Length];
            source.CopyTo(cur_points, 0);
            for (int i = 0; i < iterations; i++)
            {
                List<PointF> new_points = new List<PointF>();

                for (int s = 0; s < (cur_points.Length - 1) * 2 - 1; s++)
                {
                    if (s % 2 == 0)
                    {
                        // 1/2  1/2
                        PointF a = cur_points[s / 2];
                        PointF b = cur_points[s / 2 + 1];
                        new_points.Add(RatioOfPoints(a, b, 1f / 2f));
                    }
                    else
                    {
                        // 1/8  6/8  1/8
                        PointF a = cur_points[s / 2];
                        PointF b = cur_points[s / 2 + 1];
                        PointF c = cur_points[s / 2 + 2];
                        new_points.Add(RatioOfPoints(RatioOfPoints(a, c, 1f / 2f), b, 1f / 4f));
                    }
                }

                cur_points = new_points.ToArray();
            }

            return cur_points;
        }

        private PointF RatioOfPoints(PointF a, PointF b, float ratio)
        { return new PointF(a.X * ratio + b.X * (1 - ratio), a.Y * ratio + b.Y * (1 - ratio)); }
    }
}
