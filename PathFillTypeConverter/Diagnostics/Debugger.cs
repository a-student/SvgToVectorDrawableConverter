using System.Diagnostics;
using static System.Diagnostics.Debugger;

namespace PathFillTypeConverter.Diagnostics
{
    public static class Debugger
    {
        [Conditional("DEBUG")]
        [DebuggerHidden]
        public static void BreakIfAttached()
        {
            if (IsAttached)
            {
                Break();
            }
        }

        [DebuggerHidden]
        public static void BreakWhen(bool condition)
        {
            if (condition)
            {
                BreakIfAttached();
            }
        }
    }
}
