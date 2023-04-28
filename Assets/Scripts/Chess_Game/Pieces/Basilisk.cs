using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basilisk : Piece
{
    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        return availableSkyMoves;
    }
    public override List<Vector2Int> SelectAvailableGroundSquares()
    {
        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        availableUnderworldMoves.Clear();
        Vector2Int direction = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
        float range = 1;

        Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(0, direction.y), new Vector2Int(1, direction.y), new Vector2Int(-1, direction.y) };
        for (int i = 0; i < takeDirections.Length; i++)
        {
            Vector2Int nextCoords = occupiedSquare + takeDirections[i];
            Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
            if (!underworldBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                continue;
            if (piece == null)
            {
                TryToAddUnderworldMove(nextCoords);
            }
            else if (!piece.IsFromSameTeam(this))
            {
                TryToAddUnderworldMove(nextCoords);
            }
        }
        for (int i = 1; i <= range; i++)
        {
            Vector2Int nextCoords = occupiedSquare - direction;
            Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
            if (!underworldBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                continue;
            if (piece == null)
                TryToAddUnderworldMove(nextCoords);
        }
        return availableUnderworldMoves;
    }

}
