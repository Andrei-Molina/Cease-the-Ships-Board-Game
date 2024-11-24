using System.Collections.Generic;
using System;
using UnityEngine;

public class Destroyer : ShipPieces
{
    /*
    public override List<Vector2Int> GetAvailableMoves(ref ShipPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        int direction = (team == 0) ? 1 : -1;

        //Move One in front
        if (currentY + direction >= 0 && currentY + direction < tileCountY && board[currentX, currentY + direction] == null)
            r.Add(new Vector2Int(currentX, currentY + direction));

        //Move One in down
        if (currentY - direction >= 0 && currentY - direction < tileCountY && board[currentX, currentY - direction] == null)
            r.Add(new Vector2Int(currentX, currentY - direction));

        //Move One in left
        if (currentX - 1 >= 0 && currentX - 1 < tileCountX && board[currentX - 1, currentY] == null)
            r.Add(new Vector2Int(currentX - 1, currentY));

        //Move One in right
        if (currentX + 1 >= 0 && currentX + 1 < tileCountX && board[currentX + 1, currentY] == null)
            r.Add(new Vector2Int(currentX + 1, currentY));

        //Kill Action

        //Both team

        //Kill One in front
        if (currentX != tileCountX && currentY + direction >= 0 && currentY + direction < tileCountY)
        {
            if (board[currentX, currentY + direction] != null && board[currentX, currentY + direction].team != team)
                if (board[currentX, currentY + direction].type != ShipPieceType.BlueSubmarine &&
                    board[currentX, currentY + direction].type != ShipPieceType.RedSubmarine &&
                    board[currentX, currentY + direction].type != ShipPieceType.BlackSubmarine &&
                    board[currentX, currentY + direction].type != ShipPieceType.SilverSubmarine)
                    r.Add(new Vector2Int(currentX, currentY + direction));
        }

        //Kill One in down
        if (currentX != tileCountX && currentY - direction >= 0 && currentY - direction < tileCountY)
            if (board[currentX, currentY - direction] != null && board[currentX, currentY - direction].team != team &&
                (board[currentX, currentY - direction].type != ShipPieceType.BlueSubmarine &&
                board[currentX, currentY - direction].type != ShipPieceType.RedSubmarine &&
                board[currentX, currentY - direction].type != ShipPieceType.BlackSubmarine &&
                board[currentX, currentY - direction].type != ShipPieceType.SilverSubmarine))
                r.Add(new Vector2Int(currentX, currentY - direction));

        //Kill One in left
        if (currentY != tileCountY && currentX - 1 >= 0 && currentX - 1 < tileCountX)
            if (board[currentX - 1, currentY] != null && board[currentX - 1, currentY].team != team &&
                (board[currentX - 1, currentY].type != ShipPieceType.BlueSubmarine &&
                board[currentX - 1, currentY].type != ShipPieceType.RedSubmarine &&
                board[currentX - 1, currentY].type != ShipPieceType.BlackSubmarine &&
                board[currentX - 1, currentY].type != ShipPieceType.SilverSubmarine))
                r.Add(new Vector2Int(currentX - 1, currentY));

        //Kill One in right
        if (currentY != tileCountY && currentX + 1 >= 0 && currentX + 1 < tileCountX)
            if (board[currentX + 1, currentY] != null && board[currentX + 1, currentY].team != team &&
                (board[currentX + 1, currentY].type != ShipPieceType.BlueSubmarine &&
                board[currentX + 1, currentY].type != ShipPieceType.RedSubmarine &&
                board[currentX + 1, currentY].type != ShipPieceType.BlackSubmarine &&
                board[currentX + 1, currentY].type != ShipPieceType.SilverSubmarine))
                r.Add(new Vector2Int(currentX + 1, currentY));

        return r;
    }*/

    public override List<Vector2Int> GetAvailableMoves(ref ShipPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;
        try
        {
            //Move One in front
            if (currentY + direction >= 0 && currentY + direction < tileCountY && board[currentX, currentY + direction] == null)
                r.Add(new Vector2Int(currentX, currentY + direction));

            //Move One in down
            if (currentY - direction >= 0 && currentY - direction < tileCountY && board[currentX, currentY - direction] == null)
                r.Add(new Vector2Int(currentX, currentY - direction));

            //Move One in left
            if (currentX - 1 >= 0 && currentX - 1 < tileCountX && board[currentX - 1, currentY] == null)
                r.Add(new Vector2Int(currentX - 1, currentY));

            //Move One in right
            if (currentX + 1 >= 0 && currentX + 1 < tileCountX && board[currentX + 1, currentY] == null)
                r.Add(new Vector2Int(currentX + 1, currentY));

            //Two in front
            if (board[currentX, currentY + direction] == null)
            {
                //Player 1 Team
                if (team == 0 && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)
                    r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
                //Player 2 Team
                if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)
                    r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }

            //Kill move
            if (currentX != tileCountX - 1)
                if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team &&
                    (board[currentX + 1, currentY + direction].type != ShipPieceType.BlueSubmarine &&
                    board[currentX + 1, currentY + direction].type != ShipPieceType.RedSubmarine &&
                    board[currentX + 1, currentY + direction].type != ShipPieceType.BlackSubmarine &&
                    board[currentX + 1, currentY + direction].type != ShipPieceType.SilverSubmarine))
                    r.Add(new Vector2Int(currentX + 1, currentY + direction));
            if (currentX != 0)
                if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team &&
                    (board[currentX - 1, currentY + direction].type != ShipPieceType.BlueSubmarine &&
                    board[currentX - 1, currentY + direction].type != ShipPieceType.RedSubmarine &&
                    board[currentX - 1, currentY + direction].type != ShipPieceType.BlackSubmarine &&
                    board[currentX - 1, currentY + direction].type != ShipPieceType.SilverSubmarine))
                    r.Add(new Vector2Int(currentX - 1, currentY + direction));
        }
        catch (IndexOutOfRangeException ex)
        {
            UnityEngine.Debug.LogWarning("Out-of-bounds move detected and ignored.");
        }
        return r;
    }
}
