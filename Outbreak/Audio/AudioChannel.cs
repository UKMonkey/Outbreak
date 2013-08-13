namespace Outbreak.Audio
{
    public enum AudioChannel : byte
    {
        /// <summary>
        /// don't play anything on this - it's just for volumes
        /// </summary>
        Master = 0,

        /// <summary>
        /// GUI sound effects
        /// </summary>
        Interface = 1,

        /// <summary>
        /// just music
        /// </summary>
        Music = 2,

        /// <summary>
        /// sound effects caused by players - eg taunts
        /// </summary>
        Entity = 3,

        /// <summary>
        /// rain / bullets etc
        /// </summary>
        Ambient = 4,
    }
}