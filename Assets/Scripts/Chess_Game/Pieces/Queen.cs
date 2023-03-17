using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
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
    public override List<Vector2Int> SelectAvailableSquares()
    {
        availableMoves.Clear();
        float rangeX = Board.BOARD_SIZE_X;
        float rangeY = Board.BOARD_SIZE_Y;

        // adds all the possible vertical moves
        foreach (var directionY in directionsY)
        {
            for (int i = 1; i <= rangeY; i++)
            {
                Vector2Int nextCoords = occupiedSquare + directionY * i;
                Piece piece = board.GetPieceOnSquare(nextCoords);
                if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
                    break;
                if (piece == null)
                    TryToAddMove(nextCoords);
                else if (!piece.IsFromSameTeam(this))
                {
                    TryToAddMove(nextCoords);
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
                Piece piece = board.GetPieceOnSquare(nextCoords);
                if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
                    break;
                if (piece == null)
                    TryToAddMove(nextCoords);
                else if (!piece.IsFromSameTeam(this))
                {
                    TryToAddMove(nextCoords);
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
                Piece piece = board.GetPieceOnSquare(nextCoords);
                if (!board.CheckIfCoordinatesAreOnBoard(nextCoords))
                    break;
                if (piece == null)
                    TryToAddMove(nextCoords);
                else if (!piece.IsFromSameTeam(this))
                {
                    TryToAddMove(nextCoords);
                    break;
                }
                else if (piece.IsFromSameTeam(this))
                    break;
            }
        }

        return availableMoves;
    }
}
