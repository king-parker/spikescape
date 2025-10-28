namespace Spikescape.Leaderboard
{
    /// <summary>
    /// Represents the metadata associated with a player's score submission.
    /// </summary>
    [System.Serializable]
    public class ScoreMetadata
    {
        /// <summary>
        /// The name of the player submitting the score.
        /// </summary>
        public string playerName;

        /// <summary>
        /// The timestamp of when the score was submitted, represented as Unix time.
        /// </summary>
        public long timestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreMetadata"/> class.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        /// <param name="timestamp">The timestamp of the score submission.</param>
        public ScoreMetadata(string playerName, long timestamp)
        {
            this.playerName = playerName;
            this.timestamp = timestamp;
        }
    }
}