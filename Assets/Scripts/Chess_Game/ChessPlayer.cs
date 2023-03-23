using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessPlayer
{
    public TeamColor team { get; set; }
    public Board skyBoard { get; set; }
    public Board groundBoard { get; set; }
    public Board underworldBoard { get; set; }
    public List<Piece> activePieces { get; private set; }

    public Vector3 cameraPosition { get; set; }


    public ChessPlayer(TeamColor team, Board skyBoard, Board groundBoard, Board underworldBoard, Vector3 cameraPosition)
    {
        this.skyBoard = skyBoard;
        this.groundBoard = groundBoard;
        this.underworldBoard = underworldBoard;
        this.team = team;
        this.cameraPosition = cameraPosition;
        activePieces = new List<Piece>();
    }

    public void AddPiece(Piece piece)
    {
        if(!activePieces.Contains(piece))
            activePieces.Add(piece);
    }

    public void RemovePiece(Piece piece)
    {
        if(activePieces.Contains(piece))
            activePieces.Remove(piece);
    }

    public void GenerateAllPossibleMoves()
    {
        foreach(var piece in activePieces)
        {
            if (skyBoard.HasPiece(piece))
                piece.SelectAvailableSquares();
        }
        foreach (var piece in activePieces)
        {
            if (groundBoard.HasPiece(piece))
                piece.SelectAvailableSquares();
        }
        foreach (var piece in activePieces)
        {
            if (underworldBoard.HasPiece(piece))
                piece.SelectAvailableSquares();
        }
    }

    public Piece[] GetPiecesAttackingOppositePieceOfType<T>() where T : Piece
    {
        return activePieces.Where(p => p.IsAttackingPieceOfType<T>()).ToArray();
    }


    public Piece[] GetPiecesPiecesOfType<T>() where T : Piece
    {
        return activePieces.Where(p => p is T).ToArray();
    }

    // NOT DONE
    public void RemoveMovesEnablingAttackOnPieceOfType<T>(ChessPlayer opponent, Piece selectedPiece) where T : Piece
    {
        List<Vector2Int> coordsToRemove = new List<Vector2Int>();
        foreach (var coords in selectedPiece.availableMoves)
        {
            Piece pieceOnCoords = groundBoard.GetPieceOnSquare(coords);
            groundBoard.UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null);
            opponent.GenerateAllPossibleMoves();
            if (opponent.CheckIfIsAttackingPiece<T>())
                coordsToRemove.Add(coords);
            groundBoard.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare, coords, selectedPiece, pieceOnCoords);
        }
        foreach(var coords in coordsToRemove)
        {
            selectedPiece.availableMoves.Remove(coords);
        }
    }

    internal void OnGameRestarted()
    {
        activePieces.Clear();
    }

    // NOT DONE
    private bool CheckIfIsAttackingPiece<T>() where T : Piece
    {
        foreach (var piece in activePieces)
        {
            if(groundBoard.HasPiece(piece) && piece.IsAttackingPieceOfType<T>())
            {
                return true;
            }
        }
        return false;
    }

    // NOT DONE
    internal bool CanHidePieceFromAttack<T>(ChessPlayer opponent) where T : Piece
    {
        foreach (var piece in activePieces)
        {
            foreach (var coords in piece.availableMoves)
            {
                Piece pieceOnCoords = groundBoard.GetPieceOnSquare(coords);
                groundBoard.UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
                opponent.GenerateAllPossibleMoves();
                if (!opponent.CheckIfIsAttackingPiece<T>())
                {
                    groundBoard.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, null);
                    return true;
                }
                groundBoard.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords);
            }
        }
        return false;
    }
}
