using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Board/Set")]
public class ChessSet : ScriptableObject
{
    [SerializeField] private GameObject[] piecesPrefabs;

    public GameObject[] GetPieces()
    {
        return piecesPrefabs;
    }
}
