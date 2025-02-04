using System;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : ShipPieces
{
    public override List<Vector2Int> GetAvailableMoves(ref ShipPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int maxRange = isBurned ? 3 : Math.Max(tileCountX, tileCountY); // Limit to 3 if burned, otherwise unrestricted

        // Down
        for (int i = 1; i <= maxRange; i++)
        {
            int y = currentY - i;
            if (y < 0) break;

            if (board[currentX, y] == null)
                r.Add(new Vector2Int(currentX, y));
            else
            {
                if (board[currentX, y].team != team)
                    r.Add(new Vector2Int(currentX, y));

                break;
            }
        }

        // Up
        for (int i = 1; i <= maxRange; i++)
        {
            int y = currentY + i;
            if (y >= tileCountY) break;

            if (board[currentX, y] == null)
                r.Add(new Vector2Int(currentX, y));
            else
            {
                if (board[currentX, y].team != team)
                    r.Add(new Vector2Int(currentX, y));

                break;
            }
        }

        // Left
        for (int i = 1; i <= maxRange; i++)
        {
            int x = currentX - i;
            if (x < 0) break;

            if (board[x, currentY] == null)
                r.Add(new Vector2Int(x, currentY));
            else
            {
                if (board[x, currentY].team != team)
                    r.Add(new Vector2Int(x, currentY));

                break;
            }
        }

        // Right
        for (int i = 1; i <= maxRange; i++)
        {
            int x = currentX + i;
            if (x >= tileCountX) break;

            if (board[x, currentY] == null)
                r.Add(new Vector2Int(x, currentY));
            else
            {
                if (board[x, currentY].team != team)
                    r.Add(new Vector2Int(x, currentY));

                break;
            }
        }

        return r;
    }
}
