using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using PathFillTypeConverter.Data;
using PathFillTypeConverter.Exceptions;
using PathFillTypeConverter.Utils;

namespace PathFillTypeConverter
{
    // http://www.w3.org/TR/SVG/paths.html#PathData
    class PathParser
    {
        private Point _currentPoint;
        private List<string> _splits;

        [NotNull]
        public Path Parse([NotNull] string pathData)
        {
            _currentPoint = new Point();
            _splits = SplitPathData(pathData).ToList();
            var subpaths = new List<Subpath>();
            while (_splits.Count > 0)
            {
                subpaths.Add(ParseSubpath());
            }
            return new Path(subpaths);
        }

        private static IEnumerable<string> SplitPathData(string pathData)
        {
            foreach (var split in PathDataSplitter.SplitByCommands(pathData))
            {
                yield return split[0].ToString();
                var parameters = PathDataSplitter.SplitParameters(split.Substring(1));
                foreach (var x in parameters)
                {
                    yield return x;
                }
            }
        }

        private string _implicitCommand;

        private Subpath ParseSubpath()
        {
            _implicitCommand = null;
            var startPoint = ParseStartPoint();
            var segments = new List<SegmentBase>();
            while (_splits.Count > 0)
            {
                switch (_splits[0])
                {
                    case "M":
                    case "m":
                        goto done;
                    case "Z":
                    case "z":
                        _currentPoint = startPoint;
                        _splits.RemoveAt(0);
                        return new Subpath(startPoint, segments, true);
                }

                var segment = ParseSegment();
                if (segment == null)
                {
                    if (_implicitCommand == null)
                    {
                        throw new PathDataConverterException("Path data must begin with a command.");
                    }
                    _splits.Insert(0, _implicitCommand);
                    segment = ParseSegment();
                }
                segments.Add(segment);
            }
            done:
            return new Subpath(startPoint, segments, false);
        }

        private Point ParseStartPoint()
        {
            switch (_splits[0])
            {
                case "M":
                    _currentPoint = ParsePoint(_splits[1], _splits[2]);
                    _splits.RemoveRange(0, 3);
                    _implicitCommand = "L";
                    break;
                case "m":
                    _currentPoint += ParsePoint(_splits[1], _splits[2]);
                    _splits.RemoveRange(0, 3);
                    _implicitCommand = "l";
                    break;
            }
            return _currentPoint;
        }

        private SegmentBase ParseSegment()
        {
            var command = _splits[0];
            var implicitCommand = _implicitCommand;
            _implicitCommand = command;
            switch (command)
            {
                case "L":
                    {
                        _currentPoint = ParsePoint(_splits[1], _splits[2]);
                        _splits.RemoveRange(0, 3);
                        return new LineSegment(_currentPoint);
                    }
                case "l":
                    {
                        _currentPoint += ParsePoint(_splits[1], _splits[2]);
                        _splits.RemoveRange(0, 3);
                        return new LineSegment(_currentPoint);
                    }
                case "H":
                    {
                        _currentPoint = new Point(ParseDouble(_splits[1]), _currentPoint.Y);
                        _splits.RemoveRange(0, 2);
                        return new LineSegment(_currentPoint);
                    }
                case "h":
                    {
                        _currentPoint = new Point(_currentPoint.X + ParseDouble(_splits[1]), _currentPoint.Y);
                        _splits.RemoveRange(0, 2);
                        return new LineSegment(_currentPoint);
                    }
                case "V":
                    {
                        _currentPoint = new Point(_currentPoint.X, ParseDouble(_splits[1]));
                        _splits.RemoveRange(0, 2);
                        return new LineSegment(_currentPoint);
                    }
                case "v":
                    {
                        _currentPoint = new Point(_currentPoint.X, _currentPoint.Y + ParseDouble(_splits[1]));
                        _splits.RemoveRange(0, 2);
                        return new LineSegment(_currentPoint);
                    }
                case "C":
                    {
                        var controlPoint1 = ParsePoint(_splits[1], _splits[2]);
                        var controlPoint2 = ParsePoint(_splits[3], _splits[4]);
                        _currentPoint = ParsePoint(_splits[5], _splits[6]);
                        _splits.RemoveRange(0, 7);
                        return new CubicBezierSegment(controlPoint1, controlPoint2, _currentPoint);
                    }
                case "c":
                    {
                        var controlPoint1 = _currentPoint + ParsePoint(_splits[1], _splits[2]);
                        var controlPoint2 = _currentPoint + ParsePoint(_splits[3], _splits[4]);
                        _currentPoint += ParsePoint(_splits[5], _splits[6]);
                        _splits.RemoveRange(0, 7);
                        return new CubicBezierSegment(controlPoint1, controlPoint2, _currentPoint);
                    }
                case "S":
                case "s":
                    throw new NotImplementedException();
                case "Q":
                    {
                        var controlPoint = ParsePoint(_splits[1], _splits[2]);
                        _currentPoint = ParsePoint(_splits[3], _splits[4]);
                        _splits.RemoveRange(0, 5);
                        return new QuadraticBezierSegment(controlPoint, _currentPoint);
                    }
                case "q":
                    {
                        var controlPoint = _currentPoint + ParsePoint(_splits[1], _splits[2]);
                        _currentPoint += ParsePoint(_splits[3], _splits[4]);
                        _splits.RemoveRange(0, 5);
                        return new QuadraticBezierSegment(controlPoint, _currentPoint);
                    }
                case "T":
                case "t":
                    throw new NotImplementedException();
                case "A":
                    {
                        var radii = ParsePoint(_splits[1], _splits[2]);
                        var xAxisRotation = ParseDouble(_splits[3]);
                        var isLargeArc = _splits[4] != "0";
                        var isSweep = _splits[5] != "0";
                        _currentPoint = ParsePoint(_splits[6], _splits[7]);
                        _splits.RemoveRange(0, 8);
                        return new EllipticalArcSegment(radii, xAxisRotation, isLargeArc, isSweep, _currentPoint);
                    }
                case "a":
                    {
                        var radii = ParsePoint(_splits[1], _splits[2]);
                        var xAxisRotation = ParseDouble(_splits[3]);
                        var isLargeArc = _splits[4] != "0";
                        var isSweep = _splits[5] != "0";
                        _currentPoint += ParsePoint(_splits[6], _splits[7]);
                        _splits.RemoveRange(0, 8);
                        return new EllipticalArcSegment(radii, xAxisRotation, isLargeArc, isSweep, _currentPoint);
                    }
                default:
                    _implicitCommand = implicitCommand;
                    return null;
            }
        }

        private static Point ParsePoint(string x, string y)
        {
            return new Point(ParseDouble(x), ParseDouble(y));
        }

        private static double ParseDouble(string d)
        {
            return double.Parse(d, CultureInfo.InvariantCulture);
        }
    }
}
