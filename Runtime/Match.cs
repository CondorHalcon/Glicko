namespace CondorHalcon.Glicko
{
    public struct Match
    {
        public Rating opponent { get; private set; }
        private float _score;
        public float score { get { return _score <= 0 ? 0 : (_score >= 1 ? 1 : .5f); } }

        public Match(Rating opponent, float score)
        {
            this.opponent = opponent;
            this._score = score;
        }
    }
}