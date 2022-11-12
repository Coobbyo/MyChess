using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChessUIManager : MonoBehaviour
{
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private PawnPromotionUI PromotionUI;
    [SerializeField] private TMP_Text resultText;

    public void HideUI()
    {
        GameOverUI.SetActive(false);
        PromotionUI.HideUI();
    }

    public void Promote(Piece piece)
    {
        PromotionUI.ShowUI();
        PromotionUI.pieceToPromote = piece;
    }

    public void OnGameFinished(string winner)
    {
        GameOverUI.SetActive(true);
        resultText.text = string.Format("{0} won!", winner);
    }

    public void OnGameFinished()
    {
        GameOverUI.SetActive(true);
        resultText.text = "Stalemate";
    }
}
