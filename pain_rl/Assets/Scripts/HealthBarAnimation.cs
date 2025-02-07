using DG.Tweening;
using System.Drawing;
using UnityEngine;

public class HealthBarAnimation : MonoBehaviour
{
    [SerializeField] private float animationLength1;
    [SerializeField] private float animationLength2;
    [SerializeField] private RectTransform rectTransformALL;
    [SerializeField]  private RectTransform rectTransformText;
    private Vector2 closedTransformALL = new Vector2(0, -100);
    private Vector2 openTransformALL = new Vector2(0, -3);
    private Vector2 closedTransformText = new Vector2(6, -3);
    private Vector2 openTransformText = new Vector2(6, 55);
    private bool isOpen;



    private void Start()
    {
        isOpen = false;
        rectTransformALL.anchoredPosition = closedTransformALL;
        rectTransformText.anchoredPosition = closedTransformText;
    }

    public void HealthInfoAnimationCome()
    {
        rectTransformALL.DOAnchorPos(openTransformALL, animationLength1).SetEase(Ease.OutBack)
            .OnComplete(() => isOpen = true);
        rectTransformText.DOAnchorPos(openTransformText, animationLength2).SetEase(Ease.OutBack);
    }

    public void HealthInfoAnimationBack()
    {
        rectTransformALL.DOAnchorPos(closedTransformALL, animationLength1).SetEase(Ease.InBack)
            .OnComplete(() => isOpen = false);
        rectTransformText.DOAnchorPos(closedTransformText, animationLength2).SetEase(Ease.OutBack);
    }

    public void ToggleHealthMenu()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            HealthInfoAnimationCome();
        }
        else
        {
            HealthInfoAnimationBack();
        }
    }

}
