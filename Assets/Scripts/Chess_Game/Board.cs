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
    private Piece selectedPiece;
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
        /*
        Debug.Log("Y ====================================================================================================");
        Debug.Log(transform.InverseTransformPoint(inputPosition));
        Debug.Log(transform.InverseTransformPoint(inputPosition).z / squareSize);
        Debug.Log(transform.InverseTransformPoint(inputPosition).z / squareSize + (BOARD_SIZE_Y / 2));
        Debug.Log(Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + (BOARD_SIZE_Y / 2));
        */

        /*
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize) + (BOARD_SIZE_X / 2);
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + (BOARD_SIZE_Y / 2);
        */
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize);
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize);

        return new Vector2Int(x, y);
    }

    public void OnSquareSelected(Vector3 inputPosition)
    {
        

        Vector2Int coords = CalculateCoordsFromPosition(inputPosition);

        //Debug.Log(coords);

        Piece piece = GetPieceOnSquare(coords);
        if (selectedPiece)
        {
            if (piece != null && selectedPiece == piece)
                DeselectPiece();
            else if (piece != null && selectedPiece != piece && chessController.IsTeamTurnActive(piece.team))
                SelectPiece(piece);
            else if (selectedPiece.CanMoveTo(coords))
                OnSelectedPieceMoved(coords, selectedPiece);
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
        chessController.CreatePieceAndInitialize(piece.occupiedSquare, piece.team, typeof(Queen));
    }

    private void SelectPiece(Piece piece)
    {
        chessController.RemoveMovesEnablingAttackOnPieceOfType<King>(piece);
        selectedPiece = piece;
        List<Vector2Int> selection = selectedPiece.availableMoves;
        ShowSelectionSquares(selection);
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
        squareSelector.ShowSelection(squaresData);
    }

    private void DeselectPiece()
    {
        selectedPiece = null;
        squareSelector.ClearSelection();
    }
    private void OnSelectedPieceMoved(Vector2Int coords, Piece piece)
    {
        TryToTakeOppositePiece(coords);
        UpdateBoardOnPieceMove(coords, piece.occupiedSquare, piece, null);
        selectedPiece.MovePiece(coords);
        DeselectPiece();
        EndTurn();
    }

    private void EndTurn()
    {
        /*
        Debug.Log("END OF TURN TEST");
        Debug.Log(grid[0, 0]);
        Debug.Log(GetPieceOnSquare(Vector2Int.zero));
        */
        chessController.EndTurn();
    }

    public void UpdateBoardOnPieceMove(Vector2Int newCoords, Vector2Int oldCoords, Piece newPiece, Piece oldPiece)
    {
        grid[oldCoords.x, oldCoords.y] = oldPiece;
        grid[newCoords.x, newCoords.y] = newPiece;
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
        
        //Debug.LogAssertion(grid[coords.x, coords.y]);
        Piece piece = GetPieceOnSquare(coords);
        /*
        Debug.LogWarning("attempt to take");
        Debug.Log(CheckIfCoordinatesAreOnBoard(coords));

        Debug.Log(coords);
        Debug.Log(piece);
        */

        if (piece && !selectedPiece.IsFromSameTeam(piece))
        {
            Debug.Log("success to take");
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
            Debug.Log(piece);
        }
    }
    
    internal void OnGameRestarted()
    {
        selectedPiece = null;
        CreateGrid();
    }

}