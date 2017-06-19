namespace Dapplo.Dopy.Shared
{
    /// <summary>
    ///     Used to make it possible to add a ConfigScreen to a well defined parent node
    /// </summary>
    public enum ConfigIds
    {
        /// <summary>
        ///     The root ID
        /// </summary>
        Root,

        /// <summary>
        ///     UI related config screens should have this as parent
        /// </summary>
        Ui,

        /// <summary>
        ///     Config screens from addons should have this as parent
        /// </summary>
        Addons
    }
}
