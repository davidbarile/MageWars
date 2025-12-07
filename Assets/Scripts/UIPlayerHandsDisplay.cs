using UnityEngine;

public class UIPlayerHandsDisplay : MonoBehaviour
{
    [SerializeField] private GameObject buttonsGroup;
    [SerializeField] private GameObject[] hands;

    private void Start()
    {
        HideAllHands();
    }

    public void ShowHand(int handIndex)
    {
        for (int i = 0; i < this.hands.Length; i++)
        {
            this.hands[i].SetActive(i == handIndex);
        }

        this.buttonsGroup.SetActive(handIndex == -1);
    }

    public void HideAllHands()
    {
        ShowHand(-1);
    }
}