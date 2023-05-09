using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Griffon : Piece
{

    Vector2Int[] offsets = new Vector2Int[]
    {
        new Vector2Int(3, 2),
        new Vector2Int(3, -2),
        new Vector2Int(2, 3),
        new Vector2Int(2, -3),
        new Vector2Int(-3, 2),
        new Vector2Int(-3, -2),
        new Vector2Int(-2, 3),
        new Vector2Int(-2, -3)
    };
    private Vector2Int[] directions_diagonal = new Vector2Int[]
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };

    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        availableSkyMoves.Clear();

        if (isFrozen(this))
            return availableSkyMoves;

        if (occupiedBoard == skyBoard)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                Vector2Int nextCoords = occupiedSquare + offsets[i];
                Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
                if (!skyBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    continue;
                if (piece == null || !piece.IsFromSameTeam(this))
                    TryToAddSkyMove(nextCoords);
            }
        }
        else if (occupiedBoard == groundBoard)
        {
            float range = 1;
            foreach (var direction in directions_diagonal)
            {
                for (int i = 1; i <= range; i++)
                {
                    Vector2Int nextCoords = occupiedSquare + direction * i;
                    Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
                    if (!skyBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                        break;
                    if (piece == null)
                        TryToAddSkyMove(nextCoords);
                    else if (!piece.IsFromSameTeam(this))
                        TryToAddSkyMove(nextCoords);
                }
            }
        }

        return availableSkyMoves;
    }

    public override List<Vector2Int> SelectAvailableGroundSquares()
    {
        availableGroundMoves.Clear();

        if (isFrozen(this))
            return availableGroundMoves;

        if (occupiedBoard == groundBoard)
        {
            float range = 1;
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
                        TryToAddGroundMove(nextCoords);
                }
            }
        }
        else if (occupiedBoard == skyBoard)
        {
            float range = 1;
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
                        TryToAddGroundMove(nextCoords);
                }
            }
        }
        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        return availableUnderworldMoves;
    }
}
