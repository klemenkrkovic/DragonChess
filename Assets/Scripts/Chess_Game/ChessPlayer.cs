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

    // MIGHT NOT DONE
    public void GenerateAllPossibleMoves()
    {
        foreach (var piece in activePieces)
        {
            piece.SelectAvailableSkySquares();
            piece.SelectAvailableGroundSquares();
            piece.SelectAvailableUnderworldSquares();
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

    public void RemoveMovesEnablingAttackOnPieceOfType<T>(ChessPlayer opponent, Piece selectedPiece) where T : Piece
    {
        List<Vector2Int> coordsToRemove = new List<Vector2Int>();
        foreach (var coords in selectedPiece.availableSkyMoves)
        {
            Board selectedPieceOriginalBoard = selectedPiece.occupiedBoard;
            bool pieceIsFromThisBoard = (selectedPiece.occupiedBoard == skyBoard) ? true : false;
            Piece pieceOnCoords = skyBoard.GetPieceOnSquare(coords);

            skyBoard.UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null, selectedPiece.occupiedBoard, pieceIsFromThisBoard);
            selectedPiece.occupiedBoard = skyBoard;

            opponent.GenerateAllPossibleMoves();
            if (opponent.CheckIfIsAttackingPiece<T>())
                coordsToRemove.Add(coords);
            selectedPieceOriginalBoard.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare, coords, selectedPiece, pieceOnCoords, selectedPiece.occupiedBoard, pieceIsFromThisBoard);
            selectedPiece.occupiedBoard = selectedPieceOriginalBoard;
        }
        foreach (var coords in selectedPiece.availableGroundMoves)
        {
            Board selectedPieceOriginalBoard = selectedPiece.occupiedBoard;
            bool pieceIsFromThisBoard = (selectedPiece.occupiedBoard == groundBoard) ? true : false;
            Piece pieceOnCoords = groundBoard.GetPieceOnSquare(coords);

            groundBoard.UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null, selectedPiece.occupiedBoard, pieceIsFromThisBoard);
            selectedPiece.occupiedBoard = groundBoard;

            opponent.GenerateAllPossibleMoves();
            if (opponent.CheckIfIsAttackingPiece<T>())
                coordsToRemove.Add(coords);
            selectedPieceOriginalBoard.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare, coords, selectedPiece, pieceOnCoords, selectedPiece.occupiedBoard, pieceIsFromThisBoard);
            selectedPiece.occupiedBoard = selectedPieceOriginalBoard;
        }
        foreach (var coords in selectedPiece.availableUnderworldMoves)
        {
            Board selectedPieceOriginalBoard = selectedPiece.occupiedBoard;
            bool pieceIsFromThisBoard = (selectedPiece.occupiedBoard == underworldBoard) ? true : false;
            Piece pieceOnCoords = underworldBoard.GetPieceOnSquare(coords);

            underworldBoard.UpdateBoardOnPieceMove(coords, selectedPiece.occupiedSquare, selectedPiece, null, selectedPiece.occupiedBoard, pieceIsFromThisBoard);
            selectedPiece.occupiedBoard = underworldBoard;

            opponent.GenerateAllPossibleMoves();
            if (opponent.CheckIfIsAttackingPiece<T>())
                coordsToRemove.Add(coords);
            selectedPieceOriginalBoard.UpdateBoardOnPieceMove(selectedPiece.occupiedSquare, coords, selectedPiece, pieceOnCoords, selectedPiece.occupiedBoard, pieceIsFromThisBoard);
            selectedPiece.occupiedBoard = selectedPieceOriginalBoard;
        }
        foreach(var coords in coordsToRemove)
        {
            selectedPiece.availableGroundMoves.Remove(coords);
        }
    }

    internal void OnGameRestarted()
    {
        activePieces.Clear();
    }

    // MIGHT NOT DONE
    private bool CheckIfIsAttackingPiece<T>() where T : Piece
    {
        foreach (var piece in activePieces)
        {
            if(skyBoard.HasPiece(piece) && piece.IsAttackingPieceOfType<T>())
            {
                return true;
            }
            if (groundBoard.HasPiece(piece) && piece.IsAttackingPieceOfType<T>())
            {
                return true;
            }
            if (underworldBoard.HasPiece(piece) && piece.IsAttackingPieceOfType<T>())
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
            foreach (var coords in piece.availableSkyMoves)
            {
                Board pieceOriginalBoard = piece.occupiedBoard;
                Piece pieceOnCoords = skyBoard.GetPieceOnSquare(coords);
                bool pieceIsFromThisBoard = (piece.occupiedBoard == skyBoard) ? true : false;
                skyBoard.UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null, piece.occupiedBoard, pieceIsFromThisBoard);
                opponent.GenerateAllPossibleMoves();
                if (!opponent.CheckIfIsAttackingPiece<T>())
                {
                    skyBoard.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, null, piece.occupiedBoard, pieceIsFromThisBoard);
                    piece.occupiedBoard = pieceOriginalBoard;
                    return true;
                }
                pieceOriginalBoard.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords, piece.occupiedBoard, pieceIsFromThisBoard);
                piece.occupiedBoard = pieceOriginalBoard;
            }
            foreach (var coords in piece.availableGroundMoves)
            {
                Board pieceOriginalBoard = piece.occupiedBoard;
                Piece pieceOnCoords = groundBoard.GetPieceOnSquare(coords);
                bool pieceIsFromThisBoard = (piece.occupiedBoard == groundBoard) ? true : false;
                groundBoard.UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null, piece.occupiedBoard, pieceIsFromThisBoard);
                opponent.GenerateAllPossibleMoves();
                if (!opponent.CheckIfIsAttackingPiece<T>())
                {
                    groundBoard.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, null, piece.occupiedBoard, pieceIsFromThisBoard);
                    piece.occupiedBoard = pieceOriginalBoard;
                    return true;
                }
                pieceOriginalBoard.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords, piece.occupiedBoard, pieceIsFromThisBoard);
                piece.occupiedBoard = pieceOriginalBoard;
            }
            foreach (var coords in piece.availableUnderworldMoves)
            {
                Board pieceOriginalBoard = piece.occupiedBoard;
                Piece pieceOnCoords = underworldBoard.GetPieceOnSquare(coords);
                bool pieceIsFromThisBoard = (piece.occupiedBoard == underworldBoard) ? true : false;
                underworldBoard.UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null, piece.occupiedBoard, pieceIsFromThisBoard);
                opponent.GenerateAllPossibleMoves();
                if (!opponent.CheckIfIsAttackingPiece<T>())
                {
                    underworldBoard.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, null, piece.occupiedBoard, pieceIsFromThisBoard);
                    piece.occupiedBoard = pieceOriginalBoard;
                    return true;
                }
                pieceOriginalBoard.UpdateBoardOnPieceMove(piece.occupiedSquare, coords, piece, pieceOnCoords, piece.occupiedBoard, pieceIsFromThisBoard);
                piece.occupiedBoard = pieceOriginalBoard;
            }
        }
        return false;
    }
}
