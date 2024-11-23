using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CondorHalcon.Glicko.Samples.TicTacToe
{
    /// <summary>
    /// Manages the player's stats and rating.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        public Rating rating;
        public int wins;
        public int losses;
        public int draws;
        /// <summary>
        /// The player's score based on wins and draws.
        /// </summary>
        public float score => wins + 0.5f * draws;

        [Header("UI")]
        public TMP_Text scoreText;
        public TMP_Text ratingText;

        /// <summary> Updates the player's stats and rating UI. </summary>
        public void UpdateUI()
        {
            scoreText.text = score.ToString();
            ratingText.text = $"{rating.Rating1:F0} ± {rating.Deviation1:F0}\n{(rating.Delta1 >= 0 ? "+" : "")}{rating.Delta1:F0}";
        }

        #region Unity Methods
        private void Start()
        {
            UpdateUI();
        }
        internal void Reset()
        {
            rating = new Rating();
            wins = 0;
            losses = 0;
            draws = 0;
        }
        #endregion
    }
}
