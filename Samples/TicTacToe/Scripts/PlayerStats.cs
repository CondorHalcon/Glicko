using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CondorHalcon.Glicko.Samples.TicTacToe
{
    public class PlayerStats : MonoBehaviour
    {
        public Rating rating;
        public int wins;
        public int losses;
        public int draws;
        public float score => wins + 0.5f * draws;

        #region Unity Methods
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
