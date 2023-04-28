using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IObjectTweener))]
[RequireComponent(typeof(MaterialSetter))]

public abstract class Piece : MonoBehaviour
{
    private MaterialSetter materialSetter;
    public Board skyBoard { protected get; set; }
    public Board groundBoard { protected get; set;}
    public Board underworldBoard { protected get; set; }
    public Vector2Int occupiedSquare { get; set; }
    public Board occupiedBoard { get; set; }
    public TeamColor team { get; set; }
    public bool hasMoved { get; private set; }
    public List<Vector2Int> availableSkyMoves;
    public List<Vector2Int> availableGroundMoves;
    public List<Vector2Int> availableUnderworldMoves;


    private IObjectTweener tweener;

    public abstract List<Vector2Int> SelectAvailableSkySquares();
    public abstract List<Vector2Int> SelectAvailableGroundSquares();
    public abstract List<Vector2Int> SelectAvailableUnderworldSquares();

    private void Awake()
    {
        availableSkyMoves = new List<Vector2Int>();
        availableGroundMoves = new List<Vector2Int>();
        availableUnderworldMoves = new List<Vector2Int>();
        tweener = GetComponent<IObjectTweener>();
        materialSetter = GetComponent<MaterialSetter>();
        hasMoved = false;
    }

    public void SetMaterial (Material material)
    {
        if (materialSetter == null)
            materialSetter = GetComponent<MaterialSetter> ();
        materialSetter.SetSingleMaterial(material);
    }

    public bool IsFromSameTeam(Piece piece)
    {
        return team == piece.team;
    }

    public bool isFrozen(Piece piece)
    {
        if (piece.occupiedBoard == groundBoard)
        {
            
            Piece pieceOnCoords = underworldBoard.grid[piece.occupiedSquare.x, piece.occupiedSquare.y];
            if (pieceOnCoords != null && pieceOnCoords.GetType() == typeof(Basilisk) && !pieceOnCoords.IsFromSameTeam(piece))
            {
                return true;
            }
        }
        return false;
    }

    public bool CanMoveTo(Vector2Int coords, Board board)
    {
        if (board == skyBoard)
            return availableSkyMoves.Contains(coords);
        else if (board == groundBoard)
            return availableGroundMoves.Contains(coords);
        else if (board == underworldBoard)
            return availableUnderworldMoves.Contains(coords);

        return false;
    }

    public virtual void MovePiece(Vector2Int coords, Board board)
    {
        Vector3 targetPosition = board.CalculatePositionFromCoords(coords);
        occupiedSquare = coords;
        hasMoved = true;
        tweener.MoveTo(transform, targetPosition);
        transform.parent = board.transform;
    }

    protected void TryToAddSkyMove(Vector2Int coords)
    {
        availableSkyMoves.Add(coords);
    }

    protected void TryToAddGroundMove(Vector2Int coords)
    {
        availableGroundMoves.Add(coords);
    }

    protected void TryToAddUnderworldMove(Vector2Int coords)
    {
        availableUnderworldMoves.Add(coords);
    }

    public void SetData(Vector2Int coords, TeamColor team, Board board, Board skyBoard, Board groundBoard, Board underworldBoard)
    {
        this.team = team;
        occupiedSquare = coords;
        this.skyBoard = skyBoard;
        this.groundBoard = groundBoard;
        this.underworldBoard = underworldBoard;
        this.occupiedBoard = board;
        transform.position = board.CalculatePositionFromCoords(coords);
    }

    public bool IsAttackingPieceOfType<T>() where T : Piece
    {
        foreach (var square in availableSkyMoves)
        {
            if (skyBoard.GetPieceOnSquare(square) is T)
                return true;
        }
        foreach (var square in availableGroundMoves)
        {
            if (groundBoard.GetPieceOnSquare(square) is T)
                return true;
        }
        foreach (var square in availableUnderworldMoves)
        {
            if (underworldBoard.GetPieceOnSquare(square) is T)
                return true;
        }
        return false;
    }
}
