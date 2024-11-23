/// <summary>Glicko package namespace.</summary>
namespace CondorHalcon.Glicko
{
    /// <summary>
    /// Glicko constants and scale factors.
    /// </summary>
    public static class Glicko
    {
        /// <summary> The default/initial rating value </summary>   
        public const double kDefaultR = 1500.0;
        /// <summary> The default/initial deviation value </summary>
        public const double kDefaultRD = 350.0;
        /// <summary> The default/initial volatility value </summary>
        public const double kDefaultS = 0.06;
        /// <summary> The Glicko-1 to Glicko-2 scale factor </summary>
        public const double kScale = 173.7178;
        /// <summary> The system constant (tau) </summary>
        public const double kSystemConst = 0.5;
        /// <summary> The convergence constant (epsilon) </summary>
        public const double kConvergence = 0.000001;
    }
}