using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour
{
    private enum GameState { Init, Play, Finished }
    private enum TypeOfGameFinish { Ongoing, Checkmate, Stalemate }

    
    [SerializeField] public Board skyBoard;
    [SerializeField] public Board groundBoard;
    [SerializeField] public Board underworldBoard;

    [SerializeField] private BoardLayout skyBoardLayout;
    [SerializeField] private BoardLayout groundBoardLayout;
    [SerializeField] private BoardLayout underworldBoardLayout;


    [SerializeField] private ChessUIManager uIManager;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform boardAnchor;

    private PieceCreator pieceCreator;
    
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;

    private ChessPlayer activePlayer;
    public Board activeBoard;
    private GameState state;
    private TypeOfGameFinish typeOfGameFinish;

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
        skyBoard.GetComponent<ColliderInputReceiver>().enabled = false;
        underworldBoard.GetComponent<ColliderInputReceiver>().enabled = false;
        activeBoard = groundBoard;

        GenerateAllPossiblePlayerMoves(activePlayer);
        typeOfGameFinish = TypeOfGameFinish.Ongoing;
        SetGameState(GameState.Play);
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
        if (activeBoard == skyBoard)
        {
            ChangeActiveBoard(-1);
            boardAnchor.Translate(Vector3.up * 11);
        }
        else if (activeBoard == underworldBoard)
        {
            ChangeActiveBoard(1);
            boardAnchor.Translate(Vector3.up * -11);
        }
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
        newPiece.SetData(squareCoords, team, board, skyBoard, groundBoard, underworldBoard);
        if (newPiece.team == TeamColor.Black)
        {
            newPiece.transform.Rotate(0, 180, 0, Space.World);
        }

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

        // NOT DONE NEED TO ADD STALEMATE
        ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);

        if (kingAttackingPieces.Length > 0)
        {  
            Piece attackedKing = oppositePlayer.GetPiecesPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttackOnPieceOfType<King>(activePlayer, attackedKing);

            int availableKingMoves = attackedKing.availableSkyMoves.Count + attackedKing.availableGroundMoves.Count + attackedKing.availableUnderworldMoves.Count;
            if (availableKingMoves == 0)
            {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                if (!canCoverKing)
                {
                    typeOfGameFinish = TypeOfGameFinish.Checkmate;
                    return true;
                }
            }
        }
        if (!oppositePlayer.canMove())
        {
            typeOfGameFinish = TypeOfGameFinish.Stalemate;
            return true;
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
        if (typeOfGameFinish == TypeOfGameFinish.Checkmate)
            uIManager.OnGameFinished(activePlayer.team.ToString() + " won");
        else if (typeOfGameFinish == TypeOfGameFinish.Stalemate)
            uIManager.OnGameFinished("Stalemate");
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
            {
                skyBoard.GetComponent<ColliderInputReceiver>().enabled = false;
                activeBoard = groundBoard;
                groundBoard.GetComponent<ColliderInputReceiver>().enabled = true;
            }
        }
        else if (activeBoard == groundBoard)
        {
            if (scroll > 0)
            {
                groundBoard.GetComponent<ColliderInputReceiver>().enabled = false;
                activeBoard = skyBoard;
                skyBoard.GetComponent<ColliderInputReceiver>().enabled = true;
            }
            else if (scroll < 0)
            {
                groundBoard.GetComponent<ColliderInputReceiver>().enabled = false;
                activeBoard = underworldBoard;
                underworldBoard.GetComponent<ColliderInputReceiver>().enabled = true;
            }
        }
        else if (activeBoard == underworldBoard)
        {
            if (scroll > 0)
            {
                underworldBoard.GetComponent<ColliderInputReceiver>().enabled = false;
                activeBoard = groundBoard;
                groundBoard.GetComponent<ColliderInputReceiver>().enabled = true;
            }
        }

    }

    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
    {
        activePlayer.RemoveMovesEnablingAttackOnPieceOfType<T>(GetOpponentToPlayer(activePlayer), piece);
    }
}
