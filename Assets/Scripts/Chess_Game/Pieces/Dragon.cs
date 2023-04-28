using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Piece
{
    private Vector2Int[] directions_diagonal = new Vector2Int[]
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
    };
    private Vector2Int[] directions_orthogonal = new Vector2Int[]
    {
        Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down
    };

    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        availableSkyMoves.Clear();

        foreach (var direction in directions_diagonal)
        {
            for (int i = 1; i <= 8; i++)
            {
                Vector2Int nextCoords = occupiedSquare + direction * i;
                Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
                if (!skyBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    break;
                if (piece == null)
                    TryToAddSkyMove(nextCoords);
                else if (!piece.IsFromSameTeam(this))
                {
                    TryToAddSkyMove(nextCoords);
                    break;
                }
                else if (piece.IsFromSameTeam(this))
                    break;
            }
        }
        foreach (var direction in directions_orthogonal)
        {
            Vector2Int nextCoords = occupiedSquare + direction;
            Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
            for (int i = 0; i < 1; i++)
            {
                if (!skyBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    break;
                if (piece == null)
                    TryToAddSkyMove(nextCoords);
                else if (!piece.IsFromSameTeam(this))
                {
                    TryToAddSkyMove(nextCoords);
                    break;
                }
                else if (piece.IsFromSameTeam(this))
                    break;
            }
        }
        return availableSkyMoves;
    }

    public override List<Vector2Int> SelectAvailableGroundSquares()
    {
        availableGroundMoves.Clear();
        Vector2Int nextCoords = occupiedSquare;
        Piece piece = groundBoard.GetPieceOnSquare(nextCoords);
        if (piece != null && !piece.IsFromSameTeam(this))
        {
            TryToAddGroundMove(nextCoords);
        }

        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        return availableUnderworldMoves;
    }
}
