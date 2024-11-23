using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CondorHalcon.Glicko.Samples.TicTacToe
{
    public class GameController : MonoBehaviour
    {
        public System.Action<int> OnGameEnd;

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

        internal void SetupGame()
        {
            playerTurn = 0;
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
        internal void WinCheck()
        {
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
                    infoText.text = playerTurn == 0 ? "Player X Wins!" : "Player O Wins!";
                    foreach (Button button in ticTacToeButtons)
                    {
                        button.interactable = false;
                    }
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
                    OnGameEnd?.Invoke(playerTurn);
                    return;
                }
            }
        }
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
            // switch the player turn
            playerTurn = playerTurn == 0 ? 1 : 0;
        }

        #region Unity Methods
        private void Reset()
        {
            playerX.Reset();
            playerO.Reset();
            SetupGame();
        }
        #endregion
    }
}
