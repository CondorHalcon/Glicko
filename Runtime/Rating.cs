using System;
using UnityEngine;

namespace CondorHalcon.Glicko
{
    /// <summary>
    /// Player rating class that contains the rating, deviation and volatility values.
    /// </summary>
    [System.Serializable]
    public class Rating
    {
        /// <summary> The rating u (mu) </summary>
        [SerializeField] internal double u;
        /// <summary>  The rating deviation p (phi) </summary>
        [SerializeField] internal double p;
        /// <summary> The rating volatility s (sigma) </summary>
        [SerializeField] internal double s;
        /// <summary>
        /// The rating delta value, since last update. </summary>
        internal double delta;
        /// <summary> The pending rating value, u' </summary>
        private double uPrime;
        /// <summary>
        /// The pending deviation value, u' </summary>
        private double pPrime;
        /// <summary> The pending volatility value, s' </summary>
        private double sPrime;

        #region Constructor
        /// <summary>
        /// Creates a new rating with the specified values.
        /// </summary>
        /// <param name="rating"></param>
        /// <param name="deviation"></param>
        /// <param name="volatility"></param>
        /// <param name="ratingDelta"></param>
        public Rating(double rating = Glicko.kDefaultR, double deviation = Glicko.kDefaultRD, double volatility = Glicko.kDefaultS, double ratingDelta = 0.0)
        {
            u = (rating - Glicko.kDefaultR) / Glicko.kScale;
            p = deviation / Glicko.kScale;
            s = volatility;
            delta = ratingDelta / Glicko.kScale;
        }

        #endregion

        #region Changing Rating
        /// <summary>
        /// Updates the rating based on the specified matches.
        /// </summary>
        /// <param name="matches">Matches to calculate for.</param>
        /// <see cref="Update(Match)"/>
        /// <see cref="Apply"/>
        public void Update(Match[] matches)
        {
            double[] gTable = new double[matches.Length];
            double[] eTable = new double[matches.Length];
            double invV = 0.0;

            // Compute the g and e values for each opponent and 
            // accumulate the results into the v value
            for (int j = 0; j < matches.Length; j++)
            {
                Rating opponent = matches[j].opponent;

                double g = matches[j].opponent.G();
                double e = matches[j].opponent.E(g, this);

                gTable[j] = g;
                eTable[j] = e;
                invV += g * g * e * (1.0 - e);
            }

            // Invert the v value
            double v = 1.0 / invV;

            // Compute the delta value from the g, e, and v
            // values
            double dInner = 0.0;
            for (int j = 0; j < matches.Length; j++)
            {
                dInner += gTable[j] * (matches[j].score - eTable[j]);
            }

            // Apply the v value to the delta
            double d = v * dInner;

            // Compute new rating, deviation and volatility values
            sPrime = Math.Exp(Convergence(d, v, p, s) / 2.0);
            pPrime = 1.0 / Math.Sqrt((1.0 / (p * p + sPrime * sPrime)) + invV);
            uPrime = u + pPrime * pPrime * dInner;
        }
        /// <summary>
        /// Updates the rating based on the specified match.
        /// </summary>
        /// <param name="match">Match to calculate for</param>
        /// <see cref="Update(Match[])"/>
        /// <see cref="Apply"/>
        public void Update(Match match)
        {
            // Compute the e and g function values
            double g = match.opponent.G();
            double e = match.opponent.E(g, this);

            // Compute 1/v and v
            double invV = g * g * e * (1.0 - e);
            double v = 1.0 / invV;

            // Compute the delta value from the g, e, and v
            // values
            double dInner = g * (match.score - e);
            double d = v * dInner;

            // Compute new rating, deviation and volatility values
            sPrime = Math.Exp(Convergence(d, v, p, s) / 2.0);
            pPrime = 1.0 / Math.Sqrt((1.0 / (p * p + sPrime * sPrime)) + invV);
            uPrime = u + pPrime * pPrime * dInner;
        }

        /// <summary>
        /// Decays the rating deviation if no games were played.
        /// </summary>
        /// <see cref="Apply"/>
        public void Decay()
        {
            uPrime = u;
            pPrime = Math.Sqrt(p * p + s * s);
            sPrime = s;
        }

        /// <summary>
        /// Applies the pending rating values to the actual rating values.
        /// </summary>
        /// <see cref="Update(Match[])"/>
        /// <see cref="Update(Match)"/>
        /// <see cref="Decay"/>
        public void Apply()
        {
            delta = uPrime - u;
            u = uPrime;
            p = pPrime;
            s = sPrime;
        }
        #endregion

        #region Properties
        /// Returns the Glicko-1 rating
        public double Rating1 { get { return (u * Glicko.kScale) + Glicko.kDefaultR; } }

        /// Returns the Glicko-1 deviation
        public double Deviation1 { get { return p * Glicko.kScale; } }
        /// <summary> Returns Glicko-1 rating delta. </summary>
        public double Delta1 { get { return delta * Glicko.kScale; } }

        /// Returns the Glicko-2 rating
        public double Rating2 { get { return u; } }

        /// Returns the Glicko-2 deviation
        public double Deviation2 { get { return p; } }

        /// Returns the Glicko-2 volatility
        public double Volatility2 { get { return s; } }
        /// <summary> Returns Glicko-2 rating delta. </summary>
        public double Delta2 { get { return delta; } }
        #endregion

        #region Private Methods
        /// <summary>
        /// Computes the value of the g function for a rating
        /// </summary>
        /// <returns></returns>
        double G()
        {
            double scale = p / Math.PI;
            return 1.0 / Math.Sqrt(1.0 + 3.0 * scale * scale);
        }

        /// <summary>
        /// Computes the value of the e function in terms of a g function value and another rating 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rating"></param>
        /// <returns></returns>        
        double E(double g, Rating rating)
        {
            double exponent = -1.0 * g * (rating.u - u);
            return 1.0 / (1.0 + Math.Exp(exponent));
        }

        /// <summary>
        /// Computes the value of the f function in terms of x, delta^2, phi^2,v, a and tau^2. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="dS"></param>
        /// <param name="pS"></param>
        /// <param name="v"></param>
        /// <param name="a"></param>
        /// <param name="tS"></param>
        /// <returns></returns>        
        static double F(double x, double dS, double pS, double v, double a, double tS)
        {
            double eX = Math.Exp(x);
            double num = eX * (dS - pS - v - eX);
            double den = pS + v + eX;

            return (num/ (2.0 * den * den)) - ((x - a)/tS);
        }

        /// <summary>
        /// Performs convergence iteration on the function f
        /// </summary>
        /// <param name="d"></param>
        /// <param name="v"></param>
        /// <param name="p"></param>
        /// <param name="s"></param>
        /// <returns></returns>        
        static double Convergence(double d, double v, double p, double s)
        {
            // Initialize function values for iteration procedure
            double dS = d * d;
            double pS = p * p;
            double tS = Glicko.kSystemConst * Glicko.kSystemConst;
            double a = Math.Log(s * s);

            // Select the upper and lower iteration ranges
            double A = a;
            double B;
            double bTest = dS - pS - v;

            if (bTest > 0.0)
            {
                B = Math.Log(bTest);
            }
            else
            {
                B = a - Glicko.kSystemConst;
                while (F(B, dS, pS, v, a, tS) < 0.0)
                {
                    B -= Glicko.kSystemConst;
                }
            }

            // Perform the iteration
            double fA = F(A, dS, pS, v, a, tS);
            double fB = F(B, dS, pS, v, a, tS);
            while (Math.Abs(B - A) > Glicko.kConvergence)
            {
                double C = A + (A - B) * fA / (fB - fA);
                double fC = F(C, dS, pS, v, a, tS);

                if (fC * fB < 0.0)
                {
                    A = B;
                    fA = fB;
                }
                else
                {
                    fA /= 2.0;
                }

                B = C;
                fB = fC;
            }

            return A;
        }
        #endregion

        ///<summary>
        /// String version of the rating in Glicko-1 format
        /// </summary>
        /// <returns>Glicko-1 rating</returns>
        public override string ToString()
        {
            return $"[µ{Rating1}:φ{Deviation1}]";
        }

        /// <summary>
        /// String version of the rating in Glicko-2 format
        /// </summary>
        /// <returns>Glicko-2 rating</returns>
        public string ToString2()
        {
            return $"[µ{Rating2}:φ{Deviation2}:σ{Volatility2}]";
        }
    }
}