using PathFillTypeConverter.Data;
using static PathFillTypeConverter.Utils.MathHelper;

namespace PathFillTypeConverter.Algorithms.CurveEquations
{
    class QuadraticBezierEquation : CurveEquation
    {
        private readonly Point _startPoint;
        private readonly Point _controlPoint;
        private readonly Point _endPoint;

        public QuadraticBezierEquation(Point startPoint, QuadraticBezierSegment segment)
        {
            _startPoint = startPoint;
            _controlPoint = segment.ControlPoint;
            _endPoint = segment.EndPoint;
        }

        public override Point GetPoint(double t)
        {
            return Square(1 - t) * _startPoint + 2 * (1 - t) * t * _controlPoint + Square(t) * _endPoint;
        }

        public override double GetT(Point point)
        {
            var a = _startPoint - 2 * _controlPoint + _endPoint;
            var b = 2 * (-_startPoint + _controlPoint);
            var c = _startPoint - point;
            double tX1, tX2, tY1, tY2;
            EquationSolver.SolveQuadratic(a.X, b.X, c.X, out tX1, out tX2);
            EquationSolver.SolveQuadratic(a.Y, b.Y, c.Y, out tY1, out tY2);
            return GetClosestT(point, tX1, tX2, tY1, tY2);
        }
    }
}
