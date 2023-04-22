using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    private Vector2Int[] directions_diagonal = new Vector2Int[]
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };

    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        return availableSkyMoves;
    }

    public override List<Vector2Int> SelectAvailableGroundSquares()
    {
        availableGroundMoves.Clear();
        float range = Board.BOARD_SIZE_Y;   // the piece cannot move more than than 8 squares anyway
        foreach (var direction in directions_diagonal)
        {
            for (int i = 1; i <= range; i++)
            {
                Vector2Int nextCoords = occupiedSquare + direction * i;
                Piece piece = groundBoard.GetPieceOnSquare(nextCoords);
                if (!groundBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    break;
                if (piece == null)
                    TryToAddGroundMove(nextCoords);
                else if (!piece.IsFromSameTeam(this))
                {
                    TryToAddGroundMove(nextCoords);
                    break;
                }
                else if (piece.IsFromSameTeam(this))
                    break;
            }
        }
        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        return availableUnderworldMoves;
    }
}
