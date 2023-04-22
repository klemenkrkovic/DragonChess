using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    Vector2Int[] offsets = new Vector2Int[]
    {
        new Vector2Int(2, 1),
        new Vector2Int(2, -1),
        new Vector2Int(1, 2),
        new Vector2Int(1, -2),
        new Vector2Int(-2, 1),
        new Vector2Int(-2, -1),
        new Vector2Int(-1, 2),
        new Vector2Int(-1, -2)
    };

    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        return availableSkyMoves;
    }

    public override List<Vector2Int> SelectAvailableGroundSquares()
    {
        availableGroundMoves.Clear();
        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2Int nextCoords = occupiedSquare + offsets[i];
            Piece piece = groundBoard.GetPieceOnSquare (nextCoords);
            if (!groundBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                continue;
            if(piece == null || !piece.IsFromSameTeam(this))
                TryToAddGroundMove(nextCoords);
        }
        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        return availableUnderworldMoves;
    }
}
