using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CurveComparer
{
    /// <summary>
    /// Designed from http://graphics.stanford.edu/courses/cs468-10-fall/LectureSlides/10_Subdivision.pdf
    /// See slide #22
    /// </summary>
    public class CornerCut : CurveAlgorithm
    {
        public PointF[] MakeCurve(PointF[] source, int iterations)
        {
            PointF[] cur_points = new PointF[source.Length];
            source.CopyTo(cur_points, 0);
            for (int i = 0; i < iterations; i++)
            {
                List<PointF> new_points = new List<PointF>();

                for (int p = 0; p < cur_points.Length - 1; p++)
                {
                    PointF a = cur_points[p];
                    PointF b = cur_points[p + 1];
                    new_points.Add(RatioOfPoints(a, b, 3f / 4f));
                    new_points.Add(RatioOfPoints(a, b, 1f / 4f));
                }

                cur_points = new_points.ToArray();
            }

            return cur_points;
        }

        private PointF RatioOfPoints(PointF a, PointF b, float ratio)
        { return new PointF(a.X * ratio + b.X * (1 - ratio), a.Y * ratio + b.Y * (1 - ratio)); }
    }
}
