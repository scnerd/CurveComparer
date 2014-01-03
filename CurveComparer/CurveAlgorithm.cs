using System;
using System.Drawing;

namespace CurveComparer
{
    interface CurveAlgorithm
    {
        PointF[] MakeCurve(PointF[] source, int iterations);
    }
}
