using UnityEngine;
using DG.Tweening;
using System.Collections;

public class FloatingHitText : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] float animationLength;
    private Vector3 size;
    public float originalY = 5;
    public float newY;
    public float scaleMultiplier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Destroy(gameObject, animationLength * 1.33f);
        AnimationPlay();
        StartCoroutine(Rotator());
    }

    private void AnimationPlay()
    {
        if(rectTransform != null)
        {
            size = rectTransform.localScale;
            var a = scaleMultiplier * 2f;
            Vector3 newPos = rectTransform.localPosition + new Vector3(0, newY + a, 0);
            rectTransform.DOAnchorPos(newPos, animationLength).SetEase(Ease.OutBack);
            rectTransform.DOScale(new Vector3(size.x * scaleMultiplier, size.y * scaleMultiplier, size.z), animationLength);
        }

        
    }

    private IEnumerator Rotator()
    {
        if(rectTransform != null)
        {
            float random = Random.Range(-30/scaleMultiplier, 30/scaleMultiplier);
            yield return new WaitForSeconds(animationLength / 3);
            rectTransform.DORotate(new Vector3(0, 0, random), animationLength);

        }

    }

}
