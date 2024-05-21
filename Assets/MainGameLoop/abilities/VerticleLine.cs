using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticleLine : BaseAbility
{

   
    protected override void AbilityLogic(GameObject StartPoint)
    {
        int xAxis = StartPoint.GetComponent<ItemStats>().CurrentGridPos.x;
        for(int y = 0; y < InputHandler.gameBoard.height; y++)
        {
            InputHandler.ListOfItemsToMove.Add(InputHandler.gameBoard.grid[xAxis, y]);
        }
        InputHandler.MatchMadeClear();
    }
}
