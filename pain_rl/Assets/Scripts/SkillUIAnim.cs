using DG.Tweening;
using UnityEngine;

public class SkillUIAnim : MonoBehaviour
{
    [SerializeField] private float animationLength1;
    [SerializeField] private float animationLength2;
    private GameObject wholeUI;
    private Vector2 hiddenTransform;
    private Vector2 openTransform;

    private void Start()
    {
        wholeUI = this.gameObject;
        openTransform = wholeUI.GetComponent<RectTransform>().anchoredPosition;

        hiddenTransform.x = openTransform.x;
        hiddenTransform.y = openTransform.y - 320;
    }
    public void GoDown()
    {
        wholeUI.GetComponent<RectTransform>().DOAnchorPos(hiddenTransform, animationLength1);
    }

    public void ComeBack()
    {
        wholeUI.GetComponent<RectTransform>().DOAnchorPos(openTransform, animationLength2).SetEase(Ease.OutBack);
    }
}
