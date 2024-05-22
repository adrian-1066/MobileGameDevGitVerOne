using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class wheelButtonUpdate : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text ScoreText;
    private float ScoreVal;
    public int valueOnText;
    public GameManager gameManager;

    public GameObject GameBoard;
    public GameObject WheelScene;
    public void UpdateText(int NewNum)
    {
        valueOnText = NewNum;
        text.text = NewNum.ToString();
        if (gameManager != null)
        {


            if (valueOnText - 1 < gameManager.highScores.Count && valueOnText > 0)
            {
                ScoreVal = gameManager.highScores[valueOnText -1];
                ScoreText.text = ScoreVal.ToString();
            }
            else
            {
                ScoreText.text = "no highscore";
            }
        }
       
    }

    public void OpenLevel()
    {
        gameManager.SetSeedForGrid(valueOnText);
        GameBoard.SetActive(true);
        WheelScene.SetActive(false);
    }
}
