using PathFillTypeConverter.Data;

namespace PathFillTypeConverter.Algorithms
{
    enum SubpathDirection
    {
        Clockwise,
        Counterclockwise
    }

    static class SubpathDirectionCalculator
    {
        public static SubpathDirection CalculateDirection(Subpath subpath)
        {
            return SubpathFillAreaCalculator.CalculateSigned(subpath) <= 0 ? SubpathDirection.Clockwise : SubpathDirection.Counterclockwise;
        }
    }
}
