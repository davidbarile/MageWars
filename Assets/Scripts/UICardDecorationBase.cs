using UnityEngine;

public class UICardDecorationBase : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [Space]
    [SerializeField] private UICardDecorationBase[] wildElementIcons;

    protected virtual void OnValidate()
    {
        this.canvasGroup = this.GetComponent<CanvasGroup>();
    }

    public void SetDimmedState(bool inIsDimmed)
    {
        try
        {
            this.canvasGroup.alpha = inIsDimmed ? .2f : 1;
        }
        catch
        {
            Debug.Log($"No canvas group on {this.name}", this.gameObject);
        }
    }
}