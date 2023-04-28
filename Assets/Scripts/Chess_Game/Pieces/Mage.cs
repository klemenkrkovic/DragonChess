using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Piece
{
    private Vector2Int[] directions_diagonal = new Vector2Int[]
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
    };
    private Vector2Int[] directionsX = new Vector2Int[]
    {
        Vector2Int.left, Vector2Int.right
    };
    private Vector2Int[] directionsY = new Vector2Int[]
    {
        Vector2Int.up, Vector2Int.down
    };

    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        availableSkyMoves.Clear();

        if (isFrozen(this))
            return availableSkyMoves;

        if (occupiedBoard == skyBoard)
        {
            foreach (var directionX in directionsX)
            {
                Vector2Int nextCoords = occupiedSquare + directionX;
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
            foreach (var directionY in directionsY)
            {
                Vector2Int nextCoords = occupiedSquare + directionY;
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
        }
        else if (occupiedBoard == underworldBoard || occupiedBoard == groundBoard)
        {
            Vector2Int nextCoords = occupiedSquare;
            Piece piece = skyBoard.GetPieceOnSquare(nextCoords);
            if (piece == null)
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

        float rangeX = Board.BOARD_SIZE_X;
        float rangeY = Board.BOARD_SIZE_Y;

        if (occupiedBoard == groundBoard)
        {
            // adds all the possible vertical moves
            foreach (var directionY in directionsY)
            {
                for (int i = 1; i <= rangeY; i++)
                {
                    Vector2Int nextCoords = occupiedSquare + directionY * i;
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

            // adds all the possible horizontal moves
            foreach (var directionX in directionsX)
            {
                for (int i = 1; i <= rangeX; i++)
                {
                    Vector2Int nextCoords = occupiedSquare + directionX * i;
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

            // ditto, diagonal directions. The range is the vertical board size because the piece cannot move further than that anyway.
            foreach (var direction in directions_diagonal)
            {
                for (int i = 1; i <= rangeY; i++)
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
        }
        else if (occupiedBoard == underworldBoard || occupiedBoard == skyBoard)
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
            foreach (var directionX in directionsX)
            {
                Vector2Int nextCoords = occupiedSquare + directionX;
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
                    else if (piece.IsFromSameTeam(this))
                        break;
                }
            }
            foreach (var directionY in directionsY)
            {
                Vector2Int nextCoords = occupiedSquare + directionY;
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
                    else if (piece.IsFromSameTeam(this))
                        break;
                }
            }
        }
        else if (occupiedBoard == groundBoard || occupiedBoard == skyBoard)
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
