using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChessUIManager : MonoBehaviour
{
    [SerializeField] private GameObject UIParent;
    [SerializeField] private TMP_Text resultText;

    public void HideUI()
    {
        UIParent.SetActive(false);
    }

    public void OnGameFinished(string winner)
    {
        UIParent.SetActive(true);
        resultText.text = string.Format("{0} won", winner);
    }
}
