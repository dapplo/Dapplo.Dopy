using System.Linq;
using Dapplo.Windows.Clipboard;

namespace Dapplo.Dopy.Shared
{
    /// <summary>
    /// Helps with the format recognisions
    /// </summary>
    public static class ClipboardFormats
    {
        /// <summary>
        /// Used to specify that the clipboard content was "created" by Dopy
        /// </summary>
        public static string Dopy { get; } = "Dapplo.Dopy";

        /// <summary>
        /// CF_UNICODETEXT
        /// </summary>
        public static string UnicodeText { get; } = "CF_UNICODETEXT";

        /// <summary>
        /// Test if the ClipboardUpdateInformation was created indirectly by Dopy, so we can excluding this from processing
        /// </summary>
        /// <param name="clipboardUpdateInformation">ClipboardUpdateInformation</param>
        /// <returns>bool if Dopy made this</returns>
        public static bool IsModifiedByDopy(this ClipboardUpdateInformation clipboardUpdateInformation)
        {
            return clipboardUpdateInformation.Formats.Contains(Dopy);
        }
    }
}
