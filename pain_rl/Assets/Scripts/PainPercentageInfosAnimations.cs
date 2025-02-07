using DG.Tweening;
using System.Drawing;
using UnityEngine;

public class PainPercentageInfosAnimations : MonoBehaviour
{
    [SerializeField] private float animationLength1;
    [SerializeField] private float animationLength2;
    [SerializeField] private RectTransform rectTransformALL;
    [SerializeField] private RectTransform rectTransformTexts;
    private Vector2 closedTransformALL = new Vector2(-240, -35);
    private Vector2 openTransformALL = new Vector2(-50, -35);
    private Vector2 closedTransformTexts = new Vector2(-186, -35);
    private Vector2 openTransformTexts = new Vector2(-50, -35);
    private bool isOpen;



    private void Start()
    {
        isOpen = false;
        rectTransformALL.anchoredPosition = closedTransformALL;
        rectTransformTexts.anchoredPosition = closedTransformTexts;
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

}
