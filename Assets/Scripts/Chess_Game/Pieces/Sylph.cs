using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sylph : Piece
{
    Vector2Int[] startingWhiteSquares = new Vector2Int[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(2, 1),
        new Vector2Int(4, 1),
        new Vector2Int(6, 1),
        new Vector2Int(8, 1),
        new Vector2Int(10, 1)
    };
    Vector2Int[] startingBlackSquares = new Vector2Int[]
    {
        new Vector2Int(0, 6),
        new Vector2Int(2, 6),
        new Vector2Int(4, 6),
        new Vector2Int(6, 6),
        new Vector2Int(8, 6),
        new Vector2Int(10, 6)
    };
    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        availableSkyMoves.Clear();

        if (isFrozen(this))
            return availableSkyMoves;

        if (occupiedBoard == skyBoard)
        {
            Vector2Int direction = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
            float range = 1;
            for (int i = 1; i <= range; i++)
            {
                Vector2Int nextCoords = occupiedSquare + direction;
                Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
                if (piece != null && !piece.IsFromSameTeam(this))
                    TryToAddSkyMove(nextCoords);
            }

            Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, direction.y), new Vector2Int(-1, direction.y) };
            for (int i = 0; i < takeDirections.Length; i++)
            {
                Vector2Int nextCoords = occupiedSquare + takeDirections[i];
                Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
                if (!skyBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    continue;
                if (piece == null)
                {
                    TryToAddSkyMove(nextCoords);
                }
            }
        }
        else if (occupiedBoard == groundBoard)
        {
            Vector2Int nextCoords = occupiedSquare;
            Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
            if (piece == null)
            {
                TryToAddSkyMove(nextCoords);
            }

            foreach (var coord in team == TeamColor.White ? startingWhiteSquares : startingBlackSquares)
            {
                Piece pieces = skyBoard.GetPieceOnSquare(coord);
                if (pieces == null)
                {
                    TryToAddSkyMove(coord);
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

        if (occupiedBoard == skyBoard)
        {
            Vector2Int nextCoords = occupiedSquare;
            Piece piece = groundBoard.GetPieceOnSquare(nextCoords);

            if (piece != null && !piece.IsFromSameTeam(this))
            {
                TryToAddGroundMove(nextCoords);
            }
        }

        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        return availableUnderworldMoves;
    }

}
