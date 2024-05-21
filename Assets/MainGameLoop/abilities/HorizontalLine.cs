using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalLine : BaseAbility
{
    protected override void AbilityLogic(GameObject StartPoint)
    {
        int yAxis = StartPoint.GetComponent<ItemStats>().CurrentGridPos.y;

        for (int x = 0; x < InputHandler.gameBoard.width; x++)
        {
            InputHandler.ListOfItemsToMove.Add(InputHandler.gameBoard.grid[x, yAxis]);
        }
        InputHandler.MatchMadeClear();
    }
}
