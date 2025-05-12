using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using DG.Tweening;


public class Book : MonoBehaviour
{

    public static Book Instance;

    [SerializeField] public GameObject P1LeftContainer;
    [SerializeField] public GameObject P1RightContainer;
    [Space]
    [Space]
    [SerializeField] private Transform P2LeftContainer1;
    [SerializeField] private Transform P2LeftContainer2;
    [SerializeField] private Transform P2LeftContainer3;
    [SerializeField] public GameObject P2RightContainer;
    [Space]
    [Space]
    [SerializeField] public GameObject P3LeftContainer;
    [SerializeField] public GameObject P3RightContainer;
    [Space]
    [Space]
    public float animLength;

    [SerializeField] Vector3 downScale;
    [SerializeField] Vector3 openScale;
    [Space]
    [Space]
    [Space]
    [Space]
    public bool punchAnimState;
    public GameObject activeSkillButton;
    private int nextContainerIndex = 0;
    public Transform[] containers;
    public Vector3 basePos;
    public Vector3 camePos;

    //private bool hasGotPunch;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        containers = new Transform[] { P2LeftContainer1, P2LeftContainer2, P2LeftContainer3 };

        basePos = transform.localPosition;
        camePos = basePos;
        camePos.y = 12;
    }

    private void Update()
    {
        GetComponent<Animator>().speed = 1 / animLength;
    }


    public void PlaceNextObject(GameObject newObject)
    {
        Transform targetContainer = containers.OrderBy(c => c.childCount).First();

        newObject.transform.SetParent(targetContainer, false);
    }

    public void GetBookState1()
    {
        if (Punch.Instance != null)
        {
            if (Punch.Instance.instantiatedBookImage != null && Punch.Instance.instantiatedBookImage.activeInHierarchy)
            {
                punchAnimState = Punch.Instance.instantiatedBookImage.GetComponent<Animator>().GetBool("Blaze");
            }
        }

    }

    public void SetBookState1()
    {
        if (Punch.Instance != null)
        {
            if (Punch.Instance.instantiatedBookImage != null)
            {
                Punch.Instance.instantiatedBookImage.GetComponent<Animator>().SetBool("Blaze", punchAnimState);
            }
        }
    }

    public void GetBookState2(GameObject gameObject)
    {
        activeSkillButton = gameObject;
    }

    public void SetBookState2()
    {
        if (activeSkillButton != null)
        {
            activeSkillButton.GetComponent<Animator>().SetBool("colored", true);
            Debug.Log("set active" + activeSkillButton.gameObject.name);
        }

    }

    public void SetBookStateALL()
    {
        SetBookState1();
        SetBookState2();
    }

    public void GetBookStateALL()
    {
        GetBookState1();

        if(activeSkillButton != null)
            GetBookState2(activeSkillButton);
    }

    public void BookComeAnim()
    {
        transform.DOLocalMove(camePos, animLength).SetEase(Ease.OutBack).SetUpdate(true);
        transform.DOScale(openScale, animLength).SetEase(Ease.InBack).SetUpdate(true);
    }

    public void BookGoAnim()
    {
        transform.DOLocalMove(basePos, animLength).SetEase(Ease.InBack).SetUpdate(true);
        transform.DOScale(downScale, animLength).SetEase(Ease.OutBack).SetUpdate(true);
    }
}
