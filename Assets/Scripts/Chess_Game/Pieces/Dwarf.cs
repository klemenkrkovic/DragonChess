using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dwarf : Piece
{
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
    };
    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        return availableSkyMoves;
    }
    public override List<Vector2Int> SelectAvailableGroundSquares()
    {
        availableGroundMoves.Clear();

        if (isFrozen(this))
            return availableGroundMoves;

        if (occupiedBoard == groundBoard)
        {
            Vector2Int moveDirection = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;

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
                    else if (piece.IsFromSameTeam(this))
                        break;
                }
            }

            Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, moveDirection.y), new Vector2Int(-1, moveDirection.y) };
            for (int i = 0; i < takeDirections.Length; i++)
            {
                Vector2Int nextCoords = occupiedSquare + takeDirections[i];
                Piece piece = groundBoard.GetPieceOnSquare(nextCoords);
                if (!groundBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    continue;
                if (piece != null && !piece.IsFromSameTeam(this))
                {
                    TryToAddGroundMove(nextCoords);
                }
            }
        }
        if (occupiedBoard == underworldBoard)
        {
            Vector2Int nextCoords = occupiedSquare;
            Piece piece = groundBoard.GetPieceOnSquare(nextCoords);
            if (piece == null)
            {
                TryToAddGroundMove(nextCoords);
            }
            else if (!piece.IsFromSameTeam(this))
            {
                TryToAddGroundMove(nextCoords);
            }
        }

        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        availableUnderworldMoves.Clear();

        if (occupiedBoard == underworldBoard)
        {
            Vector2Int moveDirection = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;

            float range = 1;
            foreach (var direction in directions)
            {
                for (int i = 1; i <= range; i++)
                {
                    Vector2Int nextCoords = occupiedSquare + direction;
                    Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
                    if (!underworldBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                        break;
                    if (piece == null)
                        TryToAddUnderworldMove(nextCoords);
                    else if (piece.IsFromSameTeam(this))
                        break;
                }
            }

            Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, moveDirection.y), new Vector2Int(-1, moveDirection.y) };
            for (int i = 0; i < takeDirections.Length; i++)
            {
                Vector2Int nextCoords = occupiedSquare + takeDirections[i];
                Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
                if (!underworldBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    continue;
                if (piece != null && !piece.IsFromSameTeam(this))
                {
                    TryToAddUnderworldMove(nextCoords);
                }
            }
        }
        else if (occupiedBoard == groundBoard)
        {
            Vector2Int nextCoords = occupiedSquare;
            Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
            if (piece == null)
            {
                TryToAddUnderworldMove(nextCoords);
            }
        }

        return availableUnderworldMoves;
    }
}
