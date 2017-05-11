namespace Dapplo.Dopy.Entities
{
    /// <summary>
    /// Informs of a clip object that was added
    /// </summary>
    public class ClipAddedMessage
    {
        /// <summary>
        /// The Clip object that was added
        /// </summary>
        public Clip NewClip { get; }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="newClip"></param>
        public ClipAddedMessage(Clip newClip)
        {
            NewClip = newClip;
        }
    }
}
