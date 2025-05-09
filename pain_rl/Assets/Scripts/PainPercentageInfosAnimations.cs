using DG.Tweening;
using System.Drawing;
using UnityEngine;

public class PainPercentageInfosAnimations : MonoBehaviour
{
    [SerializeField] private float animationLength1;
    [SerializeField] private float animationLength2;
    [SerializeField] private float animationLength3;
    [SerializeField] private RectTransform rectTransformALL;
    [SerializeField] private RectTransform rectTransformTexts;
    private Vector2 closedTransformALL = new Vector2(-240, -35);
    private Vector2 openTransformALL = new Vector2(-50, -35);
    private Vector2 closedTransformTexts = new Vector2(-186, -35);
    private Vector2 openTransformTexts = new Vector2(-50, -35);

    private Vector2 normalTransform;

    private GameObject wholePainBar;
    private bool isOpen;



    private void Start()
    {
        isOpen = false;
        wholePainBar = this.gameObject;
        rectTransformALL.anchoredPosition = closedTransformALL;
        rectTransformTexts.anchoredPosition = closedTransformTexts;
        normalTransform = wholePainBar.GetComponent<RectTransform>().anchoredPosition;
    }

    public void PercentageInfoAnimationCome()
    {
        rectTransformALL.DOAnchorPos(openTransformALL, animationLength1).SetEase(Ease.OutBack)
            .OnComplete(() => isOpen = true);
        rectTransformTexts.DOAnchorPos(openTransformTexts, animationLength2).SetEase(Ease.OutBack);
    }

    public void PercentageInfoAnimationBack()
    {
        rectTransformALL.DOAnchorPos(closedTransformALL, animationLength1).SetEase(Ease.InBack)
            .OnComplete(() => isOpen = false);
        rectTransformTexts.DOAnchorPos(closedTransformTexts, animationLength2).SetEase(Ease.OutBack);
    }

    public void TogglePercentageMenu()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            PercentageInfoAnimationCome();   
        }
        else
        {
            PercentageInfoAnimationBack();
        }
    }

    public void GoDown()
    {
        Vector2 downPos;
        downPos.y = -940;
        downPos.x = normalTransform.x;

        wholePainBar.GetComponent<RectTransform>().DOAnchorPos(downPos, animationLength3);
    }

    public void GoUp()
    {
        wholePainBar.GetComponent<RectTransform>().DOAnchorPos(normalTransform, animationLength3).SetEase(Ease.OutBack);
    }

}
