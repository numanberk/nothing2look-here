using DG.Tweening;
using UnityEngine;

public class SkillInfos : MonoBehaviour
{
    [SerializeField] private float animationLength1;
    [SerializeField] private float animationLength2;
    private GameObject wholeInfo;
    private Vector2 hiddenTransform;
    private Vector2 openTransform;

    private void Start()
    {
        wholeInfo = this.gameObject;
        hiddenTransform = wholeInfo.GetComponent<RectTransform>().anchoredPosition;

        openTransform.x = hiddenTransform.x;
        openTransform.y = hiddenTransform.y + 1180;
    }
    public void Come()
    {
        wholeInfo.GetComponent<RectTransform>().DOAnchorPos(openTransform, animationLength1).SetEase(Ease.OutBack);
    }

    public void Go()
    {
        wholeInfo.GetComponent<RectTransform>().DOAnchorPos(hiddenTransform, animationLength2);
    }
}
