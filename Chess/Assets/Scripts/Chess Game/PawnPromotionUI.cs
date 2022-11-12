using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPromotionUI : MonoBehaviour
{
    [SerializeField] private Board board;
    //[SerializeField] private UIButton BishopButton;

    public Piece pieceToPromote { get; set; }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void PromoteToPiece(PieceType piece)
    {
        board.PromotePiece(pieceToPromote, Type.GetType(piece.ToString()));
    }

    public void PromoteToPiece(string piece)
    {
        board.PromotePiece(pieceToPromote, Type.GetType(piece));
    }

    public void PromoteToBishop()
    {
        board.PromotePiece(pieceToPromote, typeof(Bishop));
    }

    public void PromoteToKnight()
    {
        board.PromotePiece(pieceToPromote, typeof(Knight));
    }

    public void PromoteToRook()
    {
        board.PromotePiece(pieceToPromote, typeof(Rook));
    }

    public void PromoteToQueen()
    {
        board.PromotePiece(pieceToPromote, typeof(Queen));
    }
}
