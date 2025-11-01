namespace Spikescape.Leaderboard
{
    /// <summary>
    /// Represents a single entry in the leaderboard.
    /// </summary>
    [System.Serializable]
    public class LeaderboardEntry
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string PlayerName;

        /// <summary>
        /// The Score achieved by the player.
        /// </summary>
        public int Score;

        /// <summary>
        /// Represents an entry in a leaderboard, containing a player's name and Score.
        /// </summary>
        /// <param name="playerName">The name of the player associated with this leaderboard entry.</param>
        /// <param name="score">The Score achieved by the player.</param>
        public LeaderboardEntry(string playerName, int score)
        {
            this.PlayerName = playerName;
            this.Score = score;
        }
    }
}