using System.Runtime.InteropServices;
using UnityEngine;

namespace LockOnPluginKK
{
    static class WinCursor
    {
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pos);

        public struct Point
        {
            public int x;
            public int y;

            public static implicit operator Vector2(Point point)
            {
                return new Vector2(point.x, point.y);
            }
        }
    }
}
