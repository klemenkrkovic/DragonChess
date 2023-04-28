using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elemental : Piece
{
    Vector2Int[] directions_orthogonal = new Vector2Int[]
    {
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
    };
    private Vector2Int[] directions_diagonal = new Vector2Int[]
{
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
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

        if (occupiedBoard == underworldBoard)
        {
            foreach (var direction in directions_orthogonal)
            {
                Vector2Int nextCoords = occupiedSquare + direction;
                Piece piece = groundBoard.GetPieceOnSquare(nextCoords);
                for (int i = 0; i < 1; i++)
                {
                    if (!groundBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                        break;
                    if (piece == null)
                        TryToAddGroundMove(nextCoords);
                    else if (!piece.IsFromSameTeam(this))
                    {
                        TryToAddGroundMove(nextCoords);
                        break;
                    }
                }
            }
        }
        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        availableUnderworldMoves.Clear();


        if (occupiedBoard == underworldBoard)
        {
            float range = 2;
            foreach (var direction in directions_orthogonal)
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
                    {
                        TryToAddUnderworldMove(nextCoords);
                        break;
                    }
                }
            }
            range = 1;
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
                }
            }
        }
        else if (occupiedBoard == groundBoard)
        {
            foreach (var direction in directions_orthogonal)
            {
                Vector2Int nextCoords = occupiedSquare + direction;
                Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
                for (int i = 0; i < 1; i++)
                {
                    if (!underworldBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                        break;
                    if (piece == null)
                        TryToAddUnderworldMove(nextCoords);
                    else if (!piece.IsFromSameTeam(this))
                    {
                        TryToAddUnderworldMove(nextCoords);
                        break;
                    }
                }
            }
        }
        return availableUnderworldMoves;
    }
}
