using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Dapplo.Dopy.Utils
{
    /// <summary>
    /// Helper methods to get the login session information, which is need to detect
    /// the uniqueness of the clipboard contents (Session-ID + Sequence-number is unique)
    /// </summary>
    public static class SessionUtils
    {
        private const uint SeGroupLogonId = 0xC0000000; // from winnt.h

        /// <summary>
        /// DllImports
        /// </summary>
        private static class NativeMethods
        {
            // Using IntPtr for pSID instead of Byte[]
            [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool ConvertSidToStringSid(IntPtr pSid, out IntPtr ptrSid);

            [DllImport("kernel32.dll")]
            public static extern IntPtr LocalFree(IntPtr hMem);

            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern bool GetTokenInformation(
                IntPtr tokenHandle,
                TokenInformationClasses tokenInformationClasses,
                IntPtr tokenInformation,
                int tokenInformationLength,
                out int returnLength);
        }

        /// <summary>
        /// Get the Session-ID SID
        /// </summary>
        /// <returns>string with SessionId as SID</returns>
        public static string GetSessionId()
        {
            int tokenInfLength = 0;
            // first call gets lenght of TokenInformation
            NativeMethods.GetTokenInformation(WindowsIdentity.GetCurrent().Token, TokenInformationClasses.TokenGroups, IntPtr.Zero, tokenInfLength, out tokenInfLength);
            var tokenInformation = Marshal.AllocHGlobal(tokenInfLength);
            try
            {
                var result = NativeMethods.GetTokenInformation(WindowsIdentity.GetCurrent().Token, TokenInformationClasses.TokenGroups, tokenInformation, tokenInfLength, out tokenInfLength);
                if (!result)
                {
                    return string.Empty;
                }
                string retVal = string.Empty;
                var groups = (TokenGroups)Marshal.PtrToStructure(tokenInformation, typeof(TokenGroups));
                int sidAndAttrSize = Marshal.SizeOf(new SidAndAttributes());
                for (int i = 0; i < groups.GroupCount; i++)
                {
                    var sidAndAttributes = (SidAndAttributes)Marshal.PtrToStructure(new IntPtr(tokenInformation.ToInt64() + i * sidAndAttrSize + IntPtr.Size), typeof(SidAndAttributes));
                    if ((sidAndAttributes.Attributes & SeGroupLogonId) != SeGroupLogonId)
                    {
                        continue;
                    }

                    NativeMethods.ConvertSidToStringSid(sidAndAttributes.Sid, out var pstr);
                    try
                    {
                        retVal = Marshal.PtrToStringAuto(pstr);
                    }
                    finally
                    {
                        NativeMethods.LocalFree(pstr);
                    }
                    break;
                }
                return retVal;
            }
            finally
            {
                Marshal.FreeHGlobal(tokenInformation);
            }
        }
    }
}
