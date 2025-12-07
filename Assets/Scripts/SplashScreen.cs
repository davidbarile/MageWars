using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Image image1, image2;

    [SerializeField] private Sprite[] mageSprites;

    [SerializeField, Range(0, 5)] private float fadeDuration = 1;
    [SerializeField, Range(0, 5)] private float showImageDuration = 1;

    private void OnEnable()
    {
        StartCoroutine(StartAnim());
    }

    private IEnumerator StartAnim()
    {
        int lastIndex = -1;
        List<int> indices = Enumerable.Range(0, this.mageSprites.Length).OrderBy(x => Random.value).ToList();

        while (this.gameObject.activeSelf)
        {
            foreach (int randomIndex in indices)
            {
                lastIndex = randomIndex;
                this.image1.sprite = mageSprites[randomIndex];
                this.image1.DOFade(1, this.fadeDuration).OnComplete(() =>
                {
                    this.image2.sprite = image1.sprite;
                    this.image1.DOFade(0, this.fadeDuration);
                });
                yield return new WaitForSeconds(this.showImageDuration);
            }
        }
    }
}