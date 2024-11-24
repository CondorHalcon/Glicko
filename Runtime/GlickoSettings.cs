using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CondorHalcon.Glicko
{
    /// <summary>
    /// The Glicko settings asset.
    /// </summary>
    public class GlickoSettings : ScriptableObject
    {
        /// <summary> The path to the settings asset. </summary>
        public const string settingsPath = "Assets/Resources/GlickoSettings.asset";

        /// <summary> The default/initial rating value </summary>   
        public double kDefaultR = 1500.0;
        /// <summary> The default/initial deviation value </summary>
        public double kDefaultRD = 350.0;
        /// <summary> The default/initial volatility value </summary>
        public double kDefaultS = 0.06;
        /// <summary> The Glicko-1 to Glicko-2 scale factor </summary>
        public double kScale = 173.7178;
        /// <summary> The system constant (tau) </summary>
        public double kSystemConst = 0.5;
        /// <summary> The convergence constant (epsilon) </summary>
        public double kConvergence = 0.000001;

        /// <summary>
        /// Gets or creates the Glicko settings asset.
        /// </summary>
        /// <returns></returns>
        public static GlickoSettings GetOrCreateSettings()
        {
            GlickoSettings settings = Resources.Load<GlickoSettings>(settingsPath.Replace("Assets/Resources/", "").Replace(".asset", ""));
            if (settings == null)
            {
                settings = CreateInstance<GlickoSettings>();
#if UNITY_EDITOR
                AssetDatabase.CreateAsset(settings, settingsPath);
#endif
            }
            return settings;
        }
    }
}
