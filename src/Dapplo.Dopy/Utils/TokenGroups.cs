using System.Runtime.InteropServices;

namespace Dapplo.Dopy.Utils
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TokenGroups
    {
        public int GroupCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public SidAndAttributes[] Groups;
    };
}
