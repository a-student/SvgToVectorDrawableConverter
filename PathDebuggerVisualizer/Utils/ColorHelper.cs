using System;
using System.Windows.Media;

namespace PathDebuggerVisualizer.Utils
{
    static class ColorHelper
    {
        public static Color FromHsb(float h, float s, float b)
        {
            if (0 == s)
            {
                return Color.FromRgb(Convert.ToByte(b * 255), Convert.ToByte(b * 255), Convert.ToByte(b * 255));
            }
            float fMax, fMid, fMin;
            if (0.5 < b)
            {
                fMax = b - (b * s) + s;
                fMin = b + (b * s) - s;
            }
            else
            {
                fMax = b + (b * s);
                fMin = b - (b * s);
            }
            var iSextant = (int)Math.Floor(h / 60f);
            if (300f <= h)
            {
                h -= 360f;
            }
            h /= 60f;
            h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (0 == iSextant % 2)
            {
                fMid = h * (fMax - fMin) + fMin;
            }
            else
            {
                fMid = fMin - h * (fMax - fMin);
            }
            var iMax = Convert.ToByte(fMax * 255);
            var iMid = Convert.ToByte(fMid * 255);
            var iMin = Convert.ToByte(fMin * 255);
            switch (iSextant)
            {
                case 1:
                    return Color.FromRgb(iMid, iMax, iMin);
                case 2:
                    return Color.FromRgb(iMin, iMax, iMid);
                case 3:
                    return Color.FromRgb(iMin, iMid, iMax);
                case 4:
                    return Color.FromRgb(iMid, iMin, iMax);
                case 5:
                    return Color.FromRgb(iMax, iMin, iMid);
                default:
                    return Color.FromRgb(iMax, iMid, iMin);
            }
        }
    }
}
