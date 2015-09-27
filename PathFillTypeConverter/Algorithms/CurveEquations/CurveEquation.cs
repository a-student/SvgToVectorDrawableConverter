using System;
using System.Linq;
using PathFillTypeConverter.Data;
using PathFillTypeConverter.Extensions;

namespace PathFillTypeConverter.Algorithms.CurveEquations
{
    abstract class CurveEquation
    {
        public abstract Point GetPoint(double t);

        public abstract double GetT(Point point);

        protected double GetClosestT(Point point, params double[] ts)
        {
            return ts.Where(x => x.IsFiniteNumber()).MinBy(x => Distance(GetPoint(x), point));
        }

        private static double Distance(Point p1, Point p2)
        {
            return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
        }
    }
}
