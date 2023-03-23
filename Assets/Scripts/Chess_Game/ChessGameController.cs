using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour
{
    private enum GameState { Init, Play, Finished }

    
    [SerializeField] private Board skyBoard;
    [SerializeField] private Board groundBoard;
    [SerializeField] private Board underworldBoard;

    [SerializeField] private BoardLayout skyBoardLayout;
    [SerializeField] private BoardLayout groundBoardLayout;
    [SerializeField] private BoardLayout underworldBoardLayout;


    [SerializeField] private ChessUIManager uIManager;
    [SerializeField] private Camera playerCamera;

    private PieceCreator pieceCreator;
    
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;

    private ChessPlayer activePlayer;
    public Board activeBoard;
    private GameState state;

    private void Awake()
    {
        SetDependencies();
        CreatePlayers();
    }
    private void SetDependencies()
    {
        pieceCreator = GetComponent<PieceCreator>();
    }
    private void CreatePlayers()
    {
        whitePlayer = new ChessPlayer(TeamColor.White, skyBoard, groundBoard, underworldBoard, new Vector3(3.43f, 10.17f, -6.84f));
        blackPlayer = new ChessPlayer(TeamColor.Black, skyBoard, groundBoard, underworldBoard, new Vector3(3.43f, 10.17f, 7.63f));
    }


    // Start is called before the first frame update
    private void Start()
    {
        StartNewGame();
    }

    private void StartNewGame()
    {
        uIManager.HideUI();
        SetGameState(GameState.Init);

        skyBoard.SetDependencies(this);
        groundBoard.SetDependencies(this);
        underworldBoard.SetDependencies(this);

        CreatePiecesFromLayout(skyBoardLayout, skyBoard);
        CreatePiecesFromLayout(groundBoardLayout, groundBoard);
        CreatePiecesFromLayout(underworldBoardLayout, underworldBoard);

        activePlayer = whitePlayer;
        activeBoard = groundBoard;

        GenerateAllPossiblePlayerMoves(activePlayer);
        SetGameState(GameState.Play);
        Debug.Log("Game STARTED");
        //Debug.Log(board.grid[0, 0]);
    }

    public void RestartGame()
    {
        DestroyPieces();
        skyBoard.OnGameRestarted();
        groundBoard.OnGameRestarted();
        underworldBoard.OnGameRestarted();
        whitePlayer.OnGameRestarted();
        blackPlayer.OnGameRestarted();
        ResetCamera();
        StartNewGame();
    }

    private void ResetCamera()
    {
        if (activePlayer == blackPlayer)
        {
            playerCamera.transform.position = whitePlayer.cameraPosition;
            playerCamera.transform.Rotate(0, 180, 0, Space.World);
        }
    }

    private void DestroyPieces()
    {
        whitePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        blackPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }

    private void SetGameState(GameState state)
    {
        this.state = state;
    }

    internal bool IsGameInProgress()
    {
        return state == GameState.Play;
    }

    private void CreatePiecesFromLayout(BoardLayout layout, Board board)
    {
        for (int i = 0; i < layout.GetPiecesCount(); i++)
        {
            Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
            TeamColor team = layout.GetSquareTeamColorAtIndex(i);
            string typeName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(typeName);
            CreatePieceAndInitialize(squareCoords, team, type, board);
        } 
    }


    public void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type, Board board)
    {
        Piece newPiece = pieceCreator.CreatePiece(type, board.transform).GetComponent<Piece>();
        newPiece.SetData(squareCoords, team, board);

        Material teamMaterial = pieceCreator.GetTeamMaterial(team);
        newPiece.SetMaterial(teamMaterial);

        board.SetPieceOnBoard(squareCoords, newPiece);

        ChessPlayer currentPlayer = team == TeamColor.White ? whitePlayer : blackPlayer;
        currentPlayer.AddPiece(newPiece);
    }

    private void GenerateAllPossiblePlayerMoves(ChessPlayer player)
    {
        player.GenerateAllPossibleMoves();
    }

    public bool IsTeamTurnActive(TeamColor team)
    {
        return activePlayer.team == team;
    }

    public void EndTurn()
    {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));

        if (CheckIfGameIsFinished())
            EndGame();
        else
            ChangeActiveTeam();
    }

    private bool CheckIfGameIsFinished()
    {
        Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
        if (kingAttackingPieces.Length > 0)
        {
            ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
            Piece attackedKing = oppositePlayer.GetPiecesPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttackOnPieceOfType<King>(activePlayer, attackedKing);

            int availableKingMoves = attackedKing.availableMoves.Count;
            if (availableKingMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                if (!canCoverKing)
                    return true;
            }
        }

        return false;
    }

    public void OnPieceRemoved(Piece piece)
    {
        ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
        pieceOwner.RemovePiece(piece);
    }

    private void EndGame()
    {
        uIManager.OnGameFinished(activePlayer.team.ToString());
        SetGameState(GameState.Finished);
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
    {
        return player == whitePlayer ? blackPlayer : whitePlayer;
    }

    private void ChangeActiveTeam()
    {
        activePlayer = activePlayer == whitePlayer ? blackPlayer : whitePlayer;

        if (activePlayer == whitePlayer)
        {
            playerCamera.transform.position = whitePlayer.cameraPosition;
            playerCamera.transform.Rotate(0, 180, 0, Space.World);
            
        }
        else if (activePlayer == blackPlayer)
        {
            playerCamera.transform.position = blackPlayer.cameraPosition;
            playerCamera.transform.Rotate(0, 180, 0, Space.World);
        }
    }

    public void ChangeActiveBoard(float scroll)
    {
        if (activeBoard == skyBoard)
        {
            if (scroll < 0)
                activeBoard = groundBoard;
        }
        else if (activeBoard == groundBoard)
        {
            if (scroll > 0)
                activeBoard = skyBoard;
            else if (scroll < 0)
                activeBoard = underworldBoard;
        }
        else if (activeBoard == underworldBoard)
        {
            if (scroll > 0)
                activeBoard = groundBoard;
        }

        Debug.Log(activeBoard);
    }

    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPieceOfType<T>(GetOpponentToPlayer(activePlayer), piece);
    }
}
