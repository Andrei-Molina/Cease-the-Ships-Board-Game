using System.Collections.Generic;
using UnityEngine;

public class AircraftCarrier : ShipPieces
{
    public override List<Vector2Int> GetAvailableMoves(ref ShipPieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Determine movement range based on the burned state
        int longMove = isBurned ? 1 : 2; // Reduced from 2 to 1 if burned
        int shortMove = 1;               // Short move remains the same

        // Top right
        int x = currentX + shortMove;
        int y = currentY + longMove;
        if (x < tileCountX && y < tileCountY)
            if (board[x, y] == null || board[x, y].team != team && (board[x, y].type != ShipPieceType.BlueSubmarine &&
                board[x, y].type != ShipPieceType.RedSubmarine &&
                board[x, y].type != ShipPieceType.BlackSubmarine &&
                board[x, y].type != ShipPieceType.SilverSubmarine ||
                board[x, y].isRevealed)) // Check if submarine is revealed
                r.Add(new Vector2Int(x, y));

        x = currentX + longMove;
        y = currentY + shortMove;
        if (x < tileCountX && y < tileCountY)
            if (board[x, y] == null || board[x, y].team != team && (board[x, y].type != ShipPieceType.BlueSubmarine &&
                board[x, y].type != ShipPieceType.RedSubmarine &&
                board[x, y].type != ShipPieceType.BlackSubmarine &&
                board[x, y].type != ShipPieceType.SilverSubmarine ||
                board[x, y].isRevealed)) // Check if submarine is revealed
                r.Add(new Vector2Int(x, y));

        // Top left
        x = currentX - shortMove;
        y = currentY + longMove;
        if (x >= 0 && y < tileCountY)
            if (board[x, y] == null || board[x, y].team != team && (board[x, y].type != ShipPieceType.BlueSubmarine &&
                board[x, y].type != ShipPieceType.RedSubmarine &&
                board[x, y].type != ShipPieceType.BlackSubmarine &&
                board[x, y].type != ShipPieceType.SilverSubmarine ||
                board[x, y].isRevealed)) // Check if submarine is revealed
                r.Add(new Vector2Int(x, y));

        x = currentX - longMove;
        y = currentY + shortMove;
        if (x >= 0 && y < tileCountY)
            if (board[x, y] == null || board[x, y].team != team && (board[x, y].type != ShipPieceType.BlueSubmarine &&
                board[x, y].type != ShipPieceType.RedSubmarine &&
                board[x, y].type != ShipPieceType.BlackSubmarine &&
                board[x, y].type != ShipPieceType.SilverSubmarine ||
                board[x, y].isRevealed)) // Check if submarine is revealed
                r.Add(new Vector2Int(x, y));

        // Bottom right
        x = currentX + shortMove;
        y = currentY - longMove;
        if (x < tileCountX && y >= 0)
            if (board[x, y] == null || board[x, y].team != team && (board[x, y].type != ShipPieceType.BlueSubmarine &&
                board[x, y].type != ShipPieceType.RedSubmarine &&
                board[x, y].type != ShipPieceType.BlackSubmarine &&
                board[x, y].type != ShipPieceType.SilverSubmarine ||
                board[x, y].isRevealed)) // Check if submarine is revealed
                r.Add(new Vector2Int(x, y));

        x = currentX + longMove;
        y = currentY - shortMove;
        if (x < tileCountX && y >= 0)
            if (board[x, y] == null || board[x, y].team != team && (board[x, y].type != ShipPieceType.BlueSubmarine &&
                board[x, y].type != ShipPieceType.RedSubmarine &&
                board[x, y].type != ShipPieceType.BlackSubmarine &&
                board[x, y].type != ShipPieceType.SilverSubmarine ||
                board[x, y].isRevealed)) // Check if submarine is revealed
                r.Add(new Vector2Int(x, y));

        // Bottom left
        x = currentX - shortMove;
        y = currentY - longMove;
        if (x >= 0 && y >= 0)
            if (board[x, y] == null || board[x, y].team != team && (board[x, y].type != ShipPieceType.BlueSubmarine &&
                board[x, y].type != ShipPieceType.RedSubmarine &&
                board[x, y].type != ShipPieceType.BlackSubmarine &&
                board[x, y].type != ShipPieceType.SilverSubmarine ||
                board[x, y].isRevealed)) // Check if submarine is revealed
                r.Add(new Vector2Int(x, y));

        x = currentX - longMove;
        y = currentY - shortMove;
        if (x >= 0 && y >= 0)
            if (board[x, y] == null || board[x, y].team != team && (board[x, y].type != ShipPieceType.BlueSubmarine &&
                board[x, y].type != ShipPieceType.RedSubmarine &&
                board[x, y].type != ShipPieceType.BlackSubmarine &&
                board[x, y].type != ShipPieceType.SilverSubmarine ||
                board[x, y].isRevealed)) // Check if submarine is revealed
                r.Add(new Vector2Int(x, y));

        return r;
    }
}
