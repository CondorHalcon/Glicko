namespace CondorHalcon.Glicko
{
    /// <summary>
    /// Glicko settings (defaults, constants, and scale factors).
    /// </summary>
    public static class Glicko
    {
        private static GlickoSettings settings;
        public static GlickoSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = GlickoSettings.GetOrCreateSettings();
                }
                return settings;
            }
        }

        /// <summary> The default/initial rating value </summary>   
        public static double DefaultRating => Settings.kDefaultR;
        /// <summary> The default/initial deviation value </summary>
        public static double DefaultRatingDeviation => Settings.kDefaultRD;
        /// <summary> The default/initial volatility value </summary>
        public static double DefaultVolatility => Settings.kDefaultS;
        /// <summary> The Glicko-1 to Glicko-2 scale factor </summary>
        public static double Scale => Settings.kScale;
        /// <summary> The system constant (tau) </summary>
        public static double SystemConst => Settings.kSystemConst;
        /// <summary> The convergence constant (epsilon) </summary>
        public static double Convergence => Settings.kConvergence;
    }
}