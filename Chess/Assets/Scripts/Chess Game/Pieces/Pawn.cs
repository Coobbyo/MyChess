using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
	//TODO: Add en passant???
	public override List<Vector2Int> SelectAvailableSquares()
	{
		availableMoves.Clear();

		//Forward Movement
        Vector2Int direction = team == TeamColor.White ? Vector2Int.up : Vector2Int.down;
		float range = hasMoved ? 1 : 2;
		for (int i = 1; i <= range; i++)
		{
			Vector2Int nextCoords = occupiedSquare + direction * i;
			Piece piece = board.GetPieceOnSquare(nextCoords);
			if(!board.CheckIfCoordinatesAreOnBoard(nextCoords))
				break;
			if(piece == null)
				TryToAddMove(nextCoords);
			else if(piece.IsFromSameTeam(this))
				break;
		}

		//Diagonal Capture
		Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, direction.y), new Vector2Int(-1, direction.y) };
		for (int i = 0; i < takeDirections.Length; i++)
		{
			Vector2Int nextCoords = occupiedSquare + takeDirections[i];
			Piece piece = board.GetPieceOnSquare(nextCoords);
			if(!board.CheckIfCoordinatesAreOnBoard(nextCoords))
				continue;
			if(piece != null && !piece.IsFromSameTeam(this))
				TryToAddMove(nextCoords);
		}

		//En Passant (I want to make sure I understand this correctly before implementation)
		//If
		//1. Capturing pawn has advanced 3 ranks (5 for white, 4 for black)
		//2. Captured pawn has moved two squares in one move and is next to pawn
		//3. Must be done IMMEDIATELY after captured pawn moves.
		//Then
		//Capturing pawn makes a diagonal capture (not adjacent to where captured pawn actually is)
		//Maybe have passable and a canPass feilds that inform of the possibility of En Passant

        return availableMoves;
	}

	public override void MovePiece(Vector2Int coords)
	{
		base.MovePiece(coords);
		CheckPromotion();
	}

	private void CheckPromotion()
	{
		int endOfBoardYCoord = team == TeamColor.White ? Board.BOARD_SIZE - 1 : 0;
		if(occupiedSquare.y == endOfBoardYCoord)
			board.PromotePiece(this);
	}
}
