using DG.Tweening;
using System.Drawing;
using UnityEngine;

public class HealthBarAnimation : MonoBehaviour
{
    [SerializeField] private float animationLength1;
    [SerializeField] private float animationLength2;
    [SerializeField] private float animationLength3;
    [SerializeField] private RectTransform rectTransformALL;
    [SerializeField]  private RectTransform rectTransformText;
    private Vector2 closedTransformALL = new Vector2(0, -100);
    private Vector2 openTransformALL = new Vector2(0, -3);
    private Vector2 closedTransformText = new Vector2(6, -3);
    private Vector2 openTransformText = new Vector2(6, 55);
    private Vector2 normalTransform;

    private GameObject wholeHealthBar;
    private bool isOpen;



    private void Start()
    {
        isOpen = false;
        wholeHealthBar = this.gameObject;
        rectTransformALL.anchoredPosition = closedTransformALL;
        rectTransformText.anchoredPosition = closedTransformText;
        normalTransform = wholeHealthBar.GetComponent<RectTransform>().anchoredPosition;
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

    public void GoDown()
    {
        Vector2 downPos;
        downPos.y = -820;
        downPos.x = normalTransform.x;

        wholeHealthBar.GetComponent<RectTransform>().DOAnchorPos(downPos, animationLength3);
    }

    public void GoUp()
    {
        wholeHealthBar.GetComponent<RectTransform>().DOAnchorPos(normalTransform, animationLength3).SetEase(Ease.OutBack);
    }

}
