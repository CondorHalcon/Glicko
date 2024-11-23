using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CondorHalcon.Glicko.Samples.TicTacToe
{
    /// <summary>
    /// Manages the game state and player turns.
    /// </summary>
    public class GameController : MonoBehaviour
    {
        [Header("Players")]
        public PlayerStats playerX;
        public PlayerStats playerO;

        [Header("Game")]
        public int playerTurn = 0;
        internal int turnCount = 0;
        internal int[] gameBoard = new int[9];

        [Header("Sprites")]
        public Sprite blankSprite;
        public Sprite playerXSprite;
        public Sprite playerOSprite;

        [Header("UI")]
        public TMP_Text infoText;
        public Button[] ticTacToeButtons;

        /// <summary> Sets up the game board and player turns. </summary>
        public void SetupGame()
        {
            playerTurn = 0;
            turnCount = 0;
            infoText.text = "Player X's Turn";
            foreach (Button button in ticTacToeButtons)
            {
                button.image.sprite = blankSprite;
                button.interactable = true;
            }
            gameBoard = new int[9];
            for (int i = 0; i < gameBoard.Length; i++)
            {
                gameBoard[i] = -100;
            }
        }
        /// <summary> Checks the game board for a win condition. </summary>
        internal void WinCheck()
        {
            // possible solutions
            int s1 = gameBoard[0] + gameBoard[1] + gameBoard[2];
            int s2 = gameBoard[3] + gameBoard[4] + gameBoard[5];
            int s3 = gameBoard[6] + gameBoard[7] + gameBoard[8];
            int s4 = gameBoard[0] + gameBoard[3] + gameBoard[6];
            int s5 = gameBoard[1] + gameBoard[4] + gameBoard[7];
            int s6 = gameBoard[2] + gameBoard[5] + gameBoard[8];
            int s7 = gameBoard[0] + gameBoard[4] + gameBoard[8];
            int s8 = gameBoard[2] + gameBoard[4] + gameBoard[6];
            int[] solutions = { s1, s2, s3, s4, s5, s6, s7, s8 };
            for (int i = 0; i < solutions.Length; i++)
            {
                if (solutions[i] == 3 * (playerTurn + 1))
                {
                    Debug.Log($"{this.GetType()} :: WinCheck :: Player {playerTurn} Wins! on solution [s{i + 1}]");
                    infoText.text = playerTurn == 0 ? "Player X Wins!" : "Player O Wins!";
                    foreach (Button button in ticTacToeButtons)
                    {
                        button.interactable = false;
                    }

                    // update player ratings
                    Match MatchX = new Match(playerO.rating, playerTurn == 0 ? 1 : 0);
                    Match MatchO = new Match(playerX.rating, playerTurn == 0 ? 0 : 1);
                    playerX.rating.Update(MatchX);
                    playerO.rating.Update(MatchO);
                    playerX.rating.Apply();
                    playerO.rating.Apply();
                    // update player stats
                    if (playerTurn == 0)
                    {
                        playerX.wins++;
                        playerO.losses++;
                    }
                    else
                    {
                        playerO.wins++;
                        playerX.losses++;
                    }
                    // update UI
                    playerX.UpdateUI();
                    playerO.UpdateUI();
                    return;
                }
            }
        }
        /// <summary> Checks the game board for a draw condition. </summary>
        internal void Draw()
        {
            infoText.text = "Draw!";
            foreach (Button button in ticTacToeButtons)
            {
                button.interactable = false;
            }
            // update player ratings
            Match matchX = new Match(playerO.rating, 0.5f);
            Match matchO = new Match(playerX.rating, 0.5f);
            playerX.rating.Update(matchX);
            playerO.rating.Update(matchO);
            playerX.rating.Apply();
            playerO.rating.Apply();
            // update player stats
            playerX.draws++;
            playerO.draws++;
            // update UI
            playerX.UpdateUI();
            playerO.UpdateUI();
        }
        /// <summary> Handles the button click event and updates the game board. </summary>
        /// <param name="index">Index of the button that was clicked.</param>
        public void OnButtonClick(int index)
        {
            // register the turn
            ticTacToeButtons[index].interactable = false;
            ticTacToeButtons[index].image.sprite = playerTurn == 0 ? playerXSprite : playerOSprite;
            gameBoard[index] = playerTurn + 1;
            // check for a win
            turnCount++;
            if (turnCount > 4)
            {
                WinCheck();
            }
            if (turnCount >= 9)
            {
                Draw();
            }
            // switch the player turn
            playerTurn = playerTurn == 0 ? 1 : 0;
        }

        #region Unity Methods
        private void Start()
        {
            SetupGame();
        }
        private void Reset()
        {
            playerX.Reset();
            playerO.Reset();
            SetupGame();
        }
        #endregion
    }
}
