using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFeedbackToastPanel : UIPanelBase
{
    public static UIFeedbackToastPanel IN;

    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image background;

    public void Show(string inFeedback, Color inBgColor, float inDuration = 0)
    {
        this.feedbackText.text = inFeedback;
        this.background.color = inBgColor;

        base.Show();

        if (inDuration > 0)
            Invoke(nameof(Hide), inDuration);
    }
}