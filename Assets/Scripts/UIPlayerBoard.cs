using Sirenix.OdinInspector;
using UnityEngine;

public class UIPlayerBoard : MonoBehaviour
{
    public enum EPlayerType
    {
        None,
        PlayerA,
        PlayerB,
        PlayerC,
        PlayerD    
    }

    public string Name;
    public EPlayerType PlayerType;

    [ReadOnly]
    public int CurrentMoveNumber;
    public int TotalMovesTaken;

    [Space]
    public UIHand Hand;

    [Header("TRUE for Opponents, FALSE for Player")]
    [SerializeField] private bool shouldDisplayFaceUpPoints;

    public bool IsTakingTurn { get; private set; }
}