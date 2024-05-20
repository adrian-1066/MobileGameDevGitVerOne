using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class AOEAbility : BaseAbility
{
    protected override void AbilityLogic(GameObject StartPoint)
    {
        Vector2Int startingPoint = StartPoint.GetComponent<ItemStats>().CurrentGridPos;
        IsInGrid(startingPoint);
        InputHandler.ListOfItemsToMove.Add(StartPoint);
        InputHandler.MatchMadeClear();
    }

    private void IsInGrid(Vector2Int Pos)
    {
        int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

        for (int i = 0; i < 8; i++)
        {
            int nx = Pos.x + dx[i];
            int ny = Pos.y + dy[i];

            if (nx >= 0 && nx < InputHandler.gameBoard.width && ny >= 0 && ny < InputHandler.gameBoard.height)
            {
                InputHandler.ListOfItemsToMove.Add(InputHandler.gameBoard.grid[nx, ny]);
            }
        }
    }
}
