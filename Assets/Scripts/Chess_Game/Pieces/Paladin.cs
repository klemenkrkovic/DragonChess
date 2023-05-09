using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paladin : Piece
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
    Vector2Int[] orthogonalDirections1 = new Vector2Int[]
    {
       Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };
    Vector2Int[] orthogonalDirections2 = new Vector2Int[]
    {
        new Vector2Int(2, 0),
        new Vector2Int(0, 2),
        new Vector2Int(-2, 0),
        new Vector2Int(0, -2),
    };

    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        availableSkyMoves.Clear();

        if (isFrozen(this))
            return availableSkyMoves;

        if (occupiedBoard == skyBoard)
        {
            float range = 1;
            foreach (var direction in directions)
            {
                for (int i = 1; i <= range; i++)
                {
                    Vector2Int nextCoords = occupiedSquare + direction;
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
        }
        else if (occupiedBoard == groundBoard)
        {
            for (int i = 0; i < orthogonalDirections2.Length; i++)
            {
                Vector2Int nextCoords = occupiedSquare + orthogonalDirections2[i];
                Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
                if (!skyBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    continue;
                if (piece == null || !piece.IsFromSameTeam(this))
                    TryToAddSkyMove(nextCoords);
            }
        }
        else if (occupiedBoard == underworldBoard)
        {
            for (int i = 0; i < orthogonalDirections1.Length; i++)
            {
                Vector2Int nextCoords = occupiedSquare + orthogonalDirections1[i];
                Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
                if (!skyBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    continue;
                if (piece == null || !piece.IsFromSameTeam(this))
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
            for (int i = 0; i < offsets.Length; i++)
            {
                Vector2Int nextCoords = occupiedSquare + offsets[i];
                Piece piece = groundBoard.GetPieceOnSquare(nextCoords);
                if (!groundBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    continue;
                if (piece == null || !piece.IsFromSameTeam(this))
                    TryToAddGroundMove(nextCoords);
            }
        }
        else
        {
            float range = 1;
            foreach (var direction in orthogonalDirections2)
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

        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        availableUnderworldMoves.Clear();

        if (isFrozen(this))
            return availableUnderworldMoves;

        if (occupiedBoard == underworldBoard)
        {
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
                    else if (!piece.IsFromSameTeam(this))
                    {
                        TryToAddUnderworldMove(nextCoords);
                        break;
                    }
                    else if (piece.IsFromSameTeam(this))
                        break;
                }
            }
        }
        else if (occupiedBoard == groundBoard)
        {
            for (int i = 0; i < orthogonalDirections2.Length; i++)
            {
                Vector2Int nextCoords = occupiedSquare + orthogonalDirections2[i];
                Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
                if (!underworldBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    continue;
                if (piece == null || !piece.IsFromSameTeam(this))
                    TryToAddUnderworldMove(nextCoords);
            }
        }
        else if (occupiedBoard == skyBoard)
        {
            for (int i = 0; i < orthogonalDirections1.Length; i++)
            {
                Vector2Int nextCoords = occupiedSquare + orthogonalDirections1[i];
                Piece piece = underworldBoard.GetPieceOnSquare(nextCoords);
                if (!underworldBoard.CheckIfCoordinatesAreOnBoard(nextCoords))
                    continue;
                if (piece == null || !piece.IsFromSameTeam(this))
                    TryToAddUnderworldMove(nextCoords);
            }
        }

        return availableUnderworldMoves;
    }
}
