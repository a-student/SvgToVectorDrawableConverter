namespace SvgToVectorDrawableConverter.DataFormat.ScalableVectorGraphics
{
    class Transform
    {
        private Transform()
        { }

        public class Matrix : Transform
        {
            public double A, B, C, D, E, F;
        }

        public class Translate : Transform
        {
            public double Tx, Ty;
        }

        public class Scale : Transform
        {
            public double Sx, Sy;
        }

        public class Rotate : Transform
        {
            public double Angle, Cx, Cy;
        }
    }
}
