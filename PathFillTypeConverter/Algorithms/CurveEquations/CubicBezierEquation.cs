using PathFillTypeConverter.Data;
using static PathFillTypeConverter.Utils.MathHelper;

namespace PathFillTypeConverter.Algorithms.CurveEquations
{
    class CubicBezierEquation : CurveEquation
    {
        private readonly Point _startPoint;
        private readonly Point _controlPoint1;
        private readonly Point _controlPoint2;
        private readonly Point _endPoint;

        public CubicBezierEquation(Point startPoint, CubicBezierSegment segment)
        {
            _startPoint = startPoint;
            _controlPoint1 = segment.ControlPoint1;
            _controlPoint2 = segment.ControlPoint2;
            _endPoint = segment.EndPoint;
        }

        public override Point GetPoint(double t)
        {
            return Cube(1 - t) * _startPoint + 3 * Square(1 - t) * t * _controlPoint1 + 3 * (1 - t) * Square(t) * _controlPoint2 + Cube(t) * _endPoint;
        }

        public override double GetT(Point point)
        {
            var a = -_startPoint + 3 * _controlPoint1 - 3 * _controlPoint2 + _endPoint;
            var b = 3 * (_startPoint - 2 * _controlPoint1 + _controlPoint2);
            var c = 3 * (-_startPoint + _controlPoint1);
            var d = _startPoint - point;
            double tX1, tX2, tX3, tY1, tY2, tY3;
            EquationSolver.SolveCubic(a.X, b.X, c.X, d.X, out tX1, out tX2, out tX3);
            EquationSolver.SolveCubic(a.Y, b.Y, c.Y, d.Y, out tY1, out tY2, out tY3);
            return GetClosestT(point, tX1, tX2, tX3, tY1, tY2, tY3);
        }
    }
}
