using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelectorCreator))]
public class Board : MonoBehaviour
{
    public const int BOARD_SIZE_Y = 8;
    public const int BOARD_SIZE_X = 12;

    [SerializeField] private Transform bottomLeftSquareTransform;
    [SerializeField] private float squareSize;

    public Piece[,] grid;
    private Piece selectedPiece { get; set; }
    private ChessGameController chessController;
    private SquareSelectorCreator squareSelector;


    private void Awake()
    {
        squareSelector = GetComponent<SquareSelectorCreator>();
        CreateGrid();
    }

    public void SetDependencies(ChessGameController chessController)
    {
        this.chessController = chessController;
    }



    private void CreateGrid()
    {
        grid = new Piece[BOARD_SIZE_X, BOARD_SIZE_Y];
    }

    public Vector3 CalculatePositionFromCoords(Vector2Int coords)
    {
        return bottomLeftSquareTransform.position + new Vector3(coords.x * squareSize, 0f, coords.y * squareSize);
    }

    private Vector2Int CalculateCoordsFromPosition(Vector3 inputPosition)
    {
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize);
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize);

        return new Vector2Int(x, y);
    }

    public void OnSquareSelected(Vector3 inputPosition)
    {
        //Debug.Log("OnSquareSelected");
        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);

        Piece piece = GetPieceOnSquare(coords);
        if (chessController.skyBoard.selectedPiece || chessController.groundBoard.selectedPiece || chessController.underworldBoard.selectedPiece)
        {
            bool pieceIsFromThisBoard = true;
            Board oldPieceBoard = this;
            if (!selectedPiece)
            {
                pieceIsFromThisBoard = false;
                
                if (chessController.skyBoard.selectedPiece)
                {
                    oldPieceBoard = chessController.skyBoard;
                    selectedPiece = chessController.skyBoard.selectedPiece;
                }
                else if (chessController.groundBoard.selectedPiece)
                {
                    oldPieceBoard = chessController.groundBoard;
                    selectedPiece = chessController.groundBoard.selectedPiece;
                }
                else if (chessController.underworldBoard.selectedPiece)
                {
                    oldPieceBoard = chessController.underworldBoard;
                    selectedPiece = chessController.underworldBoard.selectedPiece;
                }
                
            }

            if (piece != null && selectedPiece == piece)
                DeselectPiece();
            else if (piece != null && selectedPiece != piece && chessController.IsTeamTurnActive(piece.team))
                SelectPiece(piece);
            else if (selectedPiece.CanMoveTo(coords, this))
            {
                OnSelectedPieceMoved(coords, selectedPiece, oldPieceBoard, pieceIsFromThisBoard);
            }
        }
        else
        {
            if (piece != null && chessController.IsTeamTurnActive(piece.team))
                SelectPiece(piece);
        }
    }

    internal void PromotePiece(Piece piece)
    {
        TakePiece(piece);
        chessController.CreatePieceAndInitialize(piece.occupiedSquare, piece.team, typeof(Hero), this);
    }

    private void SelectPiece(Piece piece)
    {
        chessController.RemoveMovesEnablingAttackOnPieceOfType<King>(piece);
        selectedPiece = piece;

        List<Vector2Int> skySelection = selectedPiece.availableSkyMoves;
        List<Vector2Int> groundSelection = selectedPiece.availableGroundMoves;
        List<Vector2Int> underworldSelection = selectedPiece.availableUnderworldMoves;

        if (chessController.activeBoard == chessController.skyBoard)
        {
            ShowSelectionSquares(skySelection);
            chessController.groundBoard.ShowSelectionSquares(groundSelection);
            chessController.underworldBoard.ShowSelectionSquares(underworldSelection);
        }
        else if (chessController.activeBoard == chessController.groundBoard)
        {
            ShowSelectionSquares(groundSelection);
            chessController.skyBoard.ShowSelectionSquares(skySelection);
            chessController.underworldBoard.ShowSelectionSquares(underworldSelection);
        }
        else if (chessController.activeBoard == chessController.underworldBoard)
        {
            ShowSelectionSquares(underworldSelection);
            chessController.groundBoard.ShowSelectionSquares(groundSelection);
            chessController.skyBoard.ShowSelectionSquares(skySelection);
        }  
    }

    private void ShowSelectionSquares(List<Vector2Int> selection)
    {
        Dictionary<Vector3, bool> squaresData = new Dictionary<Vector3, bool>();
        for (int i = 0; i < selection.Count; i++)
        {
            Vector3 position = CalculatePositionFromCoords(selection[i]);
            bool isSquareFree = GetPieceOnSquare(selection[i]) == null;
            squaresData.Add(position, isSquareFree);
        }
        squareSelector.ShowSelection(squaresData, this);
    }

    private void DeselectPiece()
    {
        chessController.skyBoard.selectedPiece = null;
        chessController.groundBoard.selectedPiece = null;
        chessController.underworldBoard.selectedPiece = null;
        chessController.skyBoard.squareSelector.ClearSelection();
        chessController.groundBoard.squareSelector.ClearSelection();
        chessController.underworldBoard.squareSelector.ClearSelection();
    }
    private void OnSelectedPieceMoved(Vector2Int coords, Piece piece, Board oldPieceBoard, bool pieceIsFromThisBoard)
    {
        TryToTakeOppositePiece(coords);
        if (!(this == chessController.groundBoard && selectedPiece.GetType() == typeof(Dragon)))
        {
            UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null, oldPieceBoard, pieceIsFromThisBoard);
            piece.occupiedBoard = this;
            selectedPiece.MovePiece(coords, this);
        }
        DeselectPiece();
        EndTurn();
    }

    private void EndTurn()
    {
        chessController.EndTurn();
    }

    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece, Board oldPieceBoard, bool pieceIsFromThisBoard)
    {
        if (pieceIsFromThisBoard)
        {
            grid[oldCoords.x, oldCoords.y] = oldPiece;
            grid[newCoords.x, newCoords.y] = newPiece;
        }
        else
        {
            oldPieceBoard.grid[oldCoords.x, oldCoords.y] = oldPiece;
            grid[newCoords.x, newCoords.y] = newPiece;
        }
    }

    public Piece GetPieceOnSquare(Vector2Int coords)
    {

        if (CheckIfCoordinatesAreOnBoard(coords))
        {
            return grid[coords.x, coords.y];
        }
        return null;
    }

    public bool CheckIfCoordinatesAreOnBoard(Vector2Int coords)
    {
        if (coords.x < 0 || coords.y < 0 || coords.x >= BOARD_SIZE_X || coords.y >= BOARD_SIZE_Y)
            return false;
        return true;
    }

    public bool HasPiece(Piece piece)
    {
        for (int x = 0; x < BOARD_SIZE_X; x++)
        {
            for (int y = 0; y < BOARD_SIZE_Y; y++)
            {
                if (grid[x, y] == piece)
                    return true;
            }
        }
        return false;
    }

    public void SetPieceOnBoard(Vector2Int coords, Piece piece)
    {
        if (CheckIfCoordinatesAreOnBoard(coords))
            grid[coords.x, coords.y] = piece;
    }

    private void TryToTakeOppositePiece(Vector2Int coords)
    {
        Piece piece = GetPieceOnSquare(coords);

        if (piece && !selectedPiece.IsFromSameTeam(piece))
        {
            TakePiece(piece);
        }
    }

    private void TakePiece(Piece piece)
    {
        if (piece)
        {
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            chessController.OnPieceRemoved(piece);
            Destroy(piece.gameObject);
        }
    }
    
    internal void OnGameRestarted()
    {
        selectedPiece = null;
        CreateGrid();
    }

}