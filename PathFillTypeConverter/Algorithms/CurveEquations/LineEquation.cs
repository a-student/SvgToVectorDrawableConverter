using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms.CurveEquations
{
    class LineEquation : CurveEquation
    {
        private readonly Point _startPoint;
        private readonly Point _endPoint;

        // ReSharper disable once SuggestBaseTypeForParameter
        public LineEquation(Point startPoint, LineSegment segment)
        {
            _startPoint = startPoint;
            _endPoint = segment.EndPoint;
        }

        public override Point GetPoint(double t)
        {
            return (1 - t) * _startPoint + t * _endPoint;
        }

        public override double GetT(Point point)
        {
            var a = -_startPoint + _endPoint;
            var b = _startPoint - point;
            double tX, tY;
            EquationSolver.SolveLinear(a.X, b.X, out tX);
            EquationSolver.SolveLinear(a.Y, b.Y, out tY);
            return GetClosestT(point, tX, tY);
        }
    }
}
