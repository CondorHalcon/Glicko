namespace CondorHalcon.Glicko
{
    /// <summary>
    /// Represents a match between two players.
    /// </summary>
    public struct Match
    {
        /// <summary>
        /// The opponent in the match.
        /// </summary>
        public Rating opponent { get; private set; }
        /// <summary>
        /// The score of the match. Should be 0 for a loss, 1 for a win, and .5 for a draw.
        /// </summary>
        private float _score;
        /// <summary>
        /// The score of the match. Should be 0 for a loss, 1 for a win, and .5 for a draw.
        /// </summary>
        public float score { get { return _score <= 0 ? 0 : (_score >= 1 ? 1 : .5f); } }

        /// <summary>
        /// Creates a new match.
        /// </summary>
        /// <param name="opponent">The opponent in the match.</param>
        /// <param name="score">The score of the match.</param>
        public Match(Rating opponent, float score)
        {
            this.opponent = opponent;
            this._score = score;
        }
    }
}