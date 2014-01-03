using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CurveComparer
{
    /// <summary>
    /// Based on Hermite Curves implemented below
    /// Duplicates last and first two points in order to create a continuous loop
    /// </summary>
    public class HermiteAutoLoop : Hermite
    {
        public override PointF[] MakeCurve(PointF[] source, int iterations)
        {
            if(source.Length == 0)
                return new PointF[0];
            if(source.Length == 1)
                return new PointF[]{source[0]};

            PointF[] mod_source = new PointF[source.Length + 3];
            Array.Copy(source, 0, mod_source, 1, source.Length);
            mod_source[0] = source[source.Length - 1];
            mod_source[mod_source.Length - 2] = source[0];
            mod_source[mod_source.Length - 1] = source[1];
            return base.MakeCurve(mod_source, iterations);
        }
    }

    /// <summary>
    /// Based on the Hermite Curves implemented below
    /// Duplicates end-points to produce a hermite curve that touches all visible vertices
    /// </summary>
    public class HermiteCopyEdges : Hermite
    {
        public override PointF[] MakeCurve(PointF[] source, int iterations)
        {
            if (source.Length == 0)
                return new PointF[0];
            if (source.Length == 1)
                return new PointF[] { source[0] };

            PointF[] mod_source = new PointF[source.Length + 2];
            Array.Copy(source, 0, mod_source, 1, source.Length);
            mod_source[mod_source.Length - 1] = source[source.Length - 1];
            mod_source[0] = source[0];
            return base.MakeCurve(mod_source, iterations);
        }
    }

    /// <summary>
    /// Designed with assistance from http://cubic.org/docs/hermite.htm
    /// Using Catmull-Rom splines approach to finding tangents
    /// </summary>
    public class Hermite : CurveAlgorithm
    {
        public virtual PointF[] MakeCurve(PointF[] source, int iterations)
        {
            if(source.Length < 3)
                return new PointF[0];
            if(source.Length == 3)
                return new PointF[]{source[1]};

            List<PointF> pts = new List<PointF>();
            PointF next_point;

            PointF?[] tangents = source.Select<PointF, PointF?>((pf,i) =>
                i == 0 ? (PointF?)null :
                i == source.Length - 1 ? (PointF?)null :
                new PointF((source[i+1].X - source[i-1].X) / 2f, (source[i+1].Y - source[i-1].Y) / 2f)
                ).ToArray();

            int steps = iterations + 1;
            for(int considered_points = 1; considered_points < source.Length - 2; considered_points++)
            {
            for (int i = 0; i <= steps; i++)
            {
                double s = (double)i / steps;
                double h1 = 2 * (s*s*s) - 3 * (s*s) + 1;
                double h2 = -2 * (s*s*s) + 3 * (s*s);
                double h3 = (s*s*s) - 2 * (s*s) + s;
                double h4 = (s*s*s) - (s*s);
                next_point = new PointF(
                    (float)(
                    h1 * source[considered_points].X +
                    h2 * source[considered_points+1].X +
                    h3 * tangents[considered_points].Value.X +
                    h4 * tangents[considered_points+1].Value.X
                    ),(float)(
                    h1 * source[considered_points].Y +
                    h2 * source[considered_points+1].Y +
                    h3 * tangents[considered_points].Value.Y +
                    h4 * tangents[considered_points+1].Value.Y
                ));

                pts.Add(next_point);
            }}

            return pts.ToArray();
        }
    }
}
