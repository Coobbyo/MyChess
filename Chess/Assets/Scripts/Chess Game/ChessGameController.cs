using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour
{
	private enum GameState { Init, Play, Finished}

	[SerializeField] private BoardLayout startingBoardLayout;
	[SerializeField] private Board board;
	[SerializeField] private ChessUIManager uiManager;

	private PieceCreator pieceCreator;
	private ChessPlayer whitePlayer;
	private ChessPlayer blackPlayer;
	private ChessPlayer activePlayer;
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
		whitePlayer =  new ChessPlayer(TeamColor.White, board);
		blackPlayer =  new ChessPlayer(TeamColor.Black, board);
	}

	private void Start()
	{
		StartNewGame();
	}

	private void StartNewGame()
	{
		uiManager.HideUI();
		SetGameState(GameState.Init);
		board.SetDependencies(this);
		CreatePiecesFromLayout(startingBoardLayout);
		activePlayer = whitePlayer; //(startingBoardLayout.GetStartingPlayer() == TeamColor.White) ? whitePlayer : blackPlayer;
		GenerateAllPossiblePlayerMoves(activePlayer);
		SetGameState(GameState.Play);
	}

	public void RestartGame()
	{
		DestroyPieces();
		board.OnGameRestarted();
		whitePlayer.OnGameRestarted();
		blackPlayer.OnGameRestarted();
		StartNewGame();
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

	public bool IsGameInProgress()
	{
		return state == GameState.Play;
	}

	private void CreatePiecesFromLayout(BoardLayout layout)
	{
		for (int i = 0; i < layout.GetPiecesCount(); i++)
		{
			Vector2Int squareCoords = layout.GetSquareCoordsAtIndex(i);
			TeamColor team = layout.GetSquareTeamColorAtIndex(i);
			string typeName = layout.GetSquarePieceNameAtIndex(i);

			Type type = Type.GetType(typeName);
			CreatePieceAndInitialize(squareCoords, team, type);
		}
	}

	public void CreatePieceAndInitialize(Vector2Int squareCoords, TeamColor team, Type type)
	{
		Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
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
		if(CheckIfGameIsFinished())
			EndGame();
		else
			ChangeActiveTeam();
	}

	//TODO: Promoting seems to not trigger a check mate?
	//TODO: Check for a stalemate (total possible moves == 0 and neither king is in check)
	private bool CheckIfGameIsFinished()
	{
		Debug.Log("Checking for End");
		Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
		if(kingAttackingPieces.Length > 0)
		{
			ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
			Piece attackedKing = oppositePlayer.GetPieceOfType<King>().FirstOrDefault();
			oppositePlayer.RemoveMovesEnablingAttackOnPiece<King>(activePlayer, attackedKing);

			int availableKingMoves = attackedKing.availableMoves.Count;
			if(availableKingMoves == 0)
			{
				bool canCoverKing = oppositePlayer.CanHidePiecefromAttack<King>(activePlayer);
				if(!canCoverKing)
					return true;
			}
		}

		return false;
	}

	public void OnPieceRemoved(Piece piece)
	{
		ChessPlayer pieceOwner = (piece.team == TeamColor.White) ? whitePlayer : blackPlayer;
		pieceOwner.RemovePiece(piece);
		Destroy(piece.gameObject);
	}

	private void EndGame()
	{
		uiManager.OnGameFinished(activePlayer.team.ToString());
		SetGameState(GameState.Finished);
	}

	private void ChangeActiveTeam()
	{
		activePlayer = GetOpponentToPlayer(activePlayer);
	}

	private ChessPlayer GetOpponentToPlayer(ChessPlayer player)
	{
		return player == whitePlayer ? blackPlayer : whitePlayer;
	}

	public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece
	{
		activePlayer.RemoveMovesEnablingAttackOnPiece<T>(GetOpponentToPlayer(activePlayer), piece);
	}
}
