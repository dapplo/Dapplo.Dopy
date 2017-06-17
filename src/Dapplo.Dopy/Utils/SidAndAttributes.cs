using System;
using System.Runtime.InteropServices;

namespace Dapplo.Dopy.Utils
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SidAndAttributes
    {
        public IntPtr Sid;
        public uint Attributes;
    }
}
