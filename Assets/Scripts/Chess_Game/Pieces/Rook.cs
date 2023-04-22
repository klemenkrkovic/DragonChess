using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
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
        return availableSkyMoves;
    }

    public override List<Vector2Int> SelectAvailableGroundSquares()
    {
        availableGroundMoves.Clear();
        float rangeX = Board.BOARD_SIZE_X;
        float rangeY = Board.BOARD_SIZE_Y;

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
        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        return availableUnderworldMoves;
    }
}

