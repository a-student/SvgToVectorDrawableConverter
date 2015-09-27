using PathFillTypeConverter.Data;
using static System.Math;
using static PathFillTypeConverter.Utils.MathHelper;

namespace PathFillTypeConverter.Algorithms.CurveEquations
{
    // http://www.w3.org/TR/SVG/implnote.html#ArcImplementationNotes
    class EllipticalArcEquation : CurveEquation
    {
        private readonly double _cos;
        private readonly double _sin;
        private readonly double _rx;
        private readonly double _ry;
        private readonly double _cx;
        private readonly double _cy;
        private readonly double _theta1;
        private readonly double _deltaTheta;
        private readonly bool _isSweep;

        public EllipticalArcEquation(Point startPoint, EllipticalArcSegment segment)
        {
            var phi = ToRadians(segment.XAxisRotation);
            _cos = Cos(phi);
            _sin = Sin(phi);
            var mid = (startPoint - segment.EndPoint) / 2;
            var x1 = _cos * mid.X + _sin * mid.Y;
            var y1 = -_sin * mid.X + _cos * mid.Y;

            _rx = segment.Radii.X;
            _ry = segment.Radii.Y;
            var lambda = Square(x1 / _rx) + Square(y1 / _ry);
            if (lambda > 1)
            {
                lambda = Sqrt(lambda);
                _rx *= lambda;
                _ry *= lambda;
            }

            var sqrt = Sqrt(Square(_rx * _ry) / (Square(_rx * y1) + Square(x1 * _ry)) - 1);
            var sign = segment.IsLargeArc != segment.IsSweep ? 1 : -1;
            var cx1 = sign * sqrt * _rx * y1 / _ry;
            var cy1 = -sign * sqrt * _ry * x1 / _rx;
            var avg = (startPoint + segment.EndPoint) / 2;
            _cx = _cos * cx1 - _sin * cy1 + avg.X;
            _cy = _sin * cx1 + _cos * cy1 + avg.Y;
            _theta1 = ToDegrees(Atan2((y1 - cy1) / _ry, (x1 - cx1) / _rx));
            var theta2 = ToDegrees(Atan2((-y1 - cy1) / _ry, (-x1 - cx1) / _rx));
            _deltaTheta = (theta2 - _theta1) % 360;
            _isSweep = segment.IsSweep;
            if (_isSweep)
            {
                if (_deltaTheta < 0)
                {
                    _deltaTheta += 360;
                }
            }
            else
            {
                if (_deltaTheta > 0)
                {
                    _deltaTheta -= 360;
                }
            }
            _theta1 = ToRadians(_theta1);
            _deltaTheta = ToRadians(_deltaTheta);
        }

        public override Point GetPoint(double t)
        {
            var theta = _theta1 + _deltaTheta * t;
            var x = _rx * Cos(theta);
            var y = _ry * Sin(theta);
            return new Point(_cos * x - _sin * y + _cx, _sin * x + _cos * y + _cy);
        }

        public override double GetT(Point point)
        {
            var x1 = point.X - _cx;
            var y1 = point.Y - _cy;
            var cosTheta = (_cos * x1 + _sin * y1) / _rx;
            var sinTheta = (-_sin * x1 + _cos * y1) / _ry;
            var theta = Atan2(sinTheta, cosTheta);
            var deltaTheta = ToDegrees(theta - _theta1) % 360;
            if (_isSweep)
            {
                if (deltaTheta < 0)
                {
                    deltaTheta += 360;
                }
            }
            else
            {
                if (deltaTheta > 0)
                {
                    deltaTheta -= 360;
                }
            }
            deltaTheta = ToRadians(deltaTheta);
            return deltaTheta / _deltaTheta;
        }
    }
}
