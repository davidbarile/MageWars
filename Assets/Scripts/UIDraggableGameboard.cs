using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Lean.Touch;
using TMPro;

public class UIDraggableGameboard : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IDragHandler
{
    private Vector2 clickPosition;
    private Vector2 mouseOffset;

    private float doubleClickTolerance = .3f;
    private float doubleClickStartTime = -1;

    [SerializeField] private Transform parentTransform;

    public void OnPointerClick(PointerEventData eventData)
    {
        float elapsedTimeSinceLastClick = Time.time - this.doubleClickStartTime;

        StopAllCoroutines();

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (this.doubleClickStartTime > -1 && elapsedTimeSinceLastClick < this.doubleClickTolerance)
            {
                this.doubleClickStartTime = -1;

                if (LeanTouch.Fingers.Count == 1)
                    OnDoubleClick();
            }
            else
            {
                this.doubleClickStartTime = Time.time;
                StartCoroutine(ResetDoubleClickTimer());
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log($"Right Click");
            this.doubleClickStartTime = -1;
        }
    }

    private IEnumerator ResetDoubleClickTimer()
    {
        yield return new WaitForSeconds(this.doubleClickTolerance);
        this.doubleClickStartTime = -1;
    }

    public void OnDoubleClick()
    {
        // this.transform.localPosition = Vector3.zero;
        // this.transform.localScale = Vector3.one;
        this.transform.DOScale(Vector3.one, .5f);
        this.transform.DOLocalMove(Vector3.zero, .5f);
        this.parentTransform.DOLocalMove(Vector3.zero, .5f);
    }

    public void CenterGameboardOnObject(Transform target)
    {
        //this.transform.localPosition = -target.localPosition;
        this.parentTransform.DOLocalMove(-target.localPosition * this.transform.localScale.x, .5f);
        this.transform.DOLocalMove(Vector3.zero, .5f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.clickPosition = GetMousePosition();
        this.mouseOffset = (Vector2)this.parentTransform.localPosition - this.clickPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var dragItemPos = GetMousePosition();
        this.parentTransform.localPosition = dragItemPos + this.mouseOffset;
    }

    private Vector2 GetMousePosition()
    {
        var mousePos = UIDragOverlay.IN.Camera.WorldToScreenPoint(Input.mousePosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIDragOverlay.IN.Overlay, mousePos, UIDragOverlay.IN.Camera, out var dragItemPos);

        return dragItemPos;
    }
}
