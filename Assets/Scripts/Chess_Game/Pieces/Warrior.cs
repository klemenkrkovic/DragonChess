using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Piece
{

    public override List<Vector2Int> SelectAvailableSkySquares()
    {
        return availableSkyMoves;
    }
    public override List<Vector2Int> SelectAvailableGroundSquares()
    {
        availableGroundMoves.Clear();

        
        if (isFrozen(this))
        {
            return availableGroundMoves;
        }
            

        Vector2Int direction = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
        float range = 1;
        for (int i =1; i <= range; i++)
        {
            Vector2Int nextCoords = occupiedSquare + direction;
            Piece piece = groundBoard.GetPieceOnSquare(nextCoords);
            if (piece == null)
                TryToAddGroundMove(nextCoords);
        }

        Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, direction.y), new Vector2Int(-1, direction.y) };
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

        return availableGroundMoves;
    }

    public override List<Vector2Int> SelectAvailableUnderworldSquares()
    {
        return availableUnderworldMoves;
    }

    public override void MovePiece(Vector2Int coords, Board groundBoard)
    {
        base.MovePiece(coords, groundBoard);
        CheckPromotion();
    }

    private void CheckPromotion()
    {
        int endOfBoardYCoord = team == TeamColor.White ? Board.BOARD_SIZE_Y - 1 : 0;

        if (occupiedSquare.y == endOfBoardYCoord)
            groundBoard.PromotePiece(this);
    }
}
