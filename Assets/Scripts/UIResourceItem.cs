using UnityEngine;
using UnityEngine.UI;

public class UIResourceItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image iconShadow;
    [SerializeField] private Image background;
    [SerializeField] private GameObject divider;
    [SerializeField] private bool isFirstItem;

    public void SetSprite(Sprite inSprite)
    {
        this.icon.sprite = inSprite;
        this.icon.gameObject.SetActive(inSprite != null);

        if (this.iconShadow != null)
        {
            this.iconShadow.sprite = inSprite;
            this.iconShadow.gameObject.SetActive(inSprite != null);
        }

        this.gameObject.SetActive(inSprite != null);
        //this.background.gameObject.SetActive(inSprite != null);

        if (this.divider != null)
            this.divider.SetActive(inSprite != null && !this.isFirstItem);
    }
}