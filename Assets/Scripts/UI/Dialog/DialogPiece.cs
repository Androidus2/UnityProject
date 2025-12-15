using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Piece", menuName = "Scripts/DialogSystem")]
public class DialogPiece : ScriptableObject
{
    [SerializeField]
    private string[] lines;

    [SerializeField]
    private string[] unlockedMechanics;

    public string[] Lines => lines.ToArray();

    public string[] UnlockedMechanics => unlockedMechanics.ToArray();
}
