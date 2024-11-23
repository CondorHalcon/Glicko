using System;
using UnityEngine;

namespace CondorHalcon.Glicko
{
    [System.Serializable]
    public class Rating
    {
        #region Glicko Constants
        /// The default/initial rating value
        const double kDefaultR = 1500.0;
        /// The default/initial deviation value
        const double kDefaultRD = 350.0;
        /// The default/initial volatility value
        const double kDefaultS = 0.06;
        /// The Glicko-1 to Glicko-2 scale factor
        const double kScale = 173.7178;
        /// The system constant (tau)
        const double kSystemConst = 0.5;
        /// The convergence constant (epsilon)
        const double kConvergence = 0.000001;
        #endregion

        /// The rating u (mu)
        [SerializeField] internal double u;
        /// The rating deviation p (phi)
        [SerializeField] internal double p;
        /// The rating volatility s (sigma)
        [SerializeField] internal double s;
        internal double uDelta;
        /// The pending rating value, u'
        private double uPrime;
        /// The pending deviation value, u'
        private double pPrime;
        /// The pending volatility value, s'
        private double sPrime;

        #region Constructor
        public Rating(double rating = kDefaultR, double deviation = kDefaultRD,  double volatility = kDefaultS)
        {
            u = (rating - kDefaultR) / kScale;
            p = deviation / kScale;
            s = volatility;
        }

        #endregion

        #region Changing Rating
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

        // Decay the deviation if no games were played
        public void Decay()
        {
            uPrime = u;
            pPrime = Math.Sqrt(p * p + s * s);
            sPrime = s;
        }

        // Assign the new pending values to the actual rating values
        public void Apply()
        {
            uDelta = uPrime - u;
            u = uPrime;
            p = pPrime;
            s = sPrime;
        }
        #endregion

        #region Properties
        /// Returns the Glicko-1 rating
        public double Rating1 { get { return (u * kScale) + kDefaultR; } }

        /// Returns the Glicko-1 deviation
        public double Deviation1 { get { return p * kScale; } }
        /// <summary> Returns Glicko-1 rating delta. </summary>
        public double Delta1 { get { return uDelta * kScale; } }

        /// Returns the Glicko-2 rating
        public double Rating2 { get { return u; } }

        /// Returns the Glicko-2 deviation
        public double Deviation2 { get { return p; } }

        /// Returns the Glicko-2 volatility
        public double Volatility2 { get { return s; } }
        /// <summary> Returns Glicko-2 rating delta. </summary>
        public double Delta2 { get { return uDelta; } }
        #endregion

        #region Private Methods
        /// Computes the value of the g function for a rating
        double G()
        {
            double scale = p / Math.PI;
            return 1.0 / Math.Sqrt(1.0 + 3.0 * scale * scale);
        }

        /// Computes the value of the e function in terms of a g function value
        /// and another rating
        double E(double g, Rating rating)
        {
            double exponent = -1.0 * g * (rating.u - u);
            return 1.0 / (1.0 + Math.Exp(exponent));
        }

        /// Computes the value of the f function in terms of x, delta^2, phi^2,
        /// v, a and tau^2.
        static double F(double x, double dS, double pS, double v, double a, double tS)
        {
            double eX = Math.Exp(x);
            double num = eX * (dS - pS - v - eX);
            double den = pS + v + eX;

            return (num/ (2.0 * den * den)) - ((x - a)/tS);
        }

        /// Performs convergence iteration on the function f
        static double Convergence(double d, double v, double p, double s)
        {
            // Initialize function values for iteration procedure
            double dS = d * d;
            double pS = p * p;
            double tS = kSystemConst * kSystemConst;
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
                B = a - kSystemConst;
                while (F(B, dS, pS, v, a, tS) < 0.0)
                {
                    B -= kSystemConst;
                }
            }

            // Perform the iteration
            double fA = F(A, dS, pS, v, a, tS);
            double fB = F(B, dS, pS, v, a, tS);
            while (Math.Abs(B - A) > kConvergence)
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

        /// Returns the rating in Glicko-1 fromat
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        ///  Returns Glicko 1 or 2 rating
        /// </summary>
        /// <param name="glicko1">Return glicko-1</param>
        /// <returns>string</returns>
        public string ToString(bool glicko1)
        {
            return glicko1 ? $"[µ{Rating1}:φ{Deviation1}]" : $"[µ{Rating2}:φ{Deviation2}:σ{Volatility2}]";
        }
    }
}