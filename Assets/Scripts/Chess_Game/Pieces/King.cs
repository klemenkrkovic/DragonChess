using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1)
    };

    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        availableSkyMoves.Clear();

        if (isFrozen(this))
            return availableSkyMoves;

        if (occupiedBoard == groundBoard)
        {
            Vector2Int nextCoords = occupiedSquare;
            Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
            if(piece == null)
            {
                TryToAddSkyMove(nextCoords);
            }
            else if (!piece.IsFromSameTeam(this))
            {
                TryToAddSkyMove(nextCoords);
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
            foreach (var direction in directions)
            {
                for (int i = 1; i <= range; i++)
                {
                    Vector2Int nextCoords = occupiedSquare + direction;
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
        }
        else
        {
            Vector2Int nextCoords = occupiedSquare;
            Piece piece = groundBoard.GetPieceOnSquare(nextCoords);
            if (piece == null)
            {
                TryToAddGroundMove(nextCoords);
            }
        }
        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        availableUnderworldMoves.Clear();

        if (isFrozen(this))
            return availableUnderworldMoves;

        if (occupiedBoard == groundBoard)
        {
            Vector2Int nextCoords = occupiedSquare;
            Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
            if (piece == null)
            {
                TryToAddUnderworldMove(nextCoords);
            }
            else if (!piece.IsFromSameTeam(this))
            {
                TryToAddUnderworldMove(nextCoords);
            }
        }
        return availableUnderworldMoves;
    }
}
