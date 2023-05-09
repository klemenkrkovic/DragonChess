using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Piece
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
        availableSkyMoves.Clear();

        if (isFrozen(this))
            return availableSkyMoves;

        if (occupiedBoard == groundBoard)
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
            float range = 2;
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
        else
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
        availableUnderworldMoves.Clear();

        if (isFrozen(this))
            return availableUnderworldMoves;

        if (occupiedBoard == groundBoard)
        {
            float range = 1;
            foreach (var direction in directions_diagonal)
            {
                for (int i = 1; i <= range; i++)
                {
                    Vector2Int nextCoords = occupiedSquare + direction * i;
                    Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
                    if (!underworldBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                        break;
                    if (piece == null)
                        TryToAddUnderworldMove(nextCoords);
                    else if (!piece.IsFromSameTeam(this))
                        TryToAddUnderworldMove(nextCoords);
                }
            }
        }

        return availableUnderworldMoves;
    }
}
