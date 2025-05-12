using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{


    public static EventManager Instance;
    public bool isPlayerDead;
    public UnityEvent PressedTAB;
    public UnityEvent PressedB;
    public UnityEvent PressedF1;
    public UnityEvent LetGoF1;
    public bool GameFrozen;
    private bool bookOpen;
    private GameObject playerValues;
    private GameObject book;
    public bool couldAttack;
    private Coroutine close;
    private Vector3 playerValuesNormalPos;
    private Vector3 playerValuesDownPos;
    private float animLengthBook = 0.3333f;
    private GameObject Player;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //F1 = false;
        GameFrozen = false;
        isPlayerDead = false;
        bookOpen = false;
        playerValues = GameObject.Find("Player Values");
        Player = GameObject.FindGameObjectWithTag("Player");

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Book")
            {
                book = obj;
            }
        }

        book.SetActive(false);
        playerValuesNormalPos = playerValues.transform.localPosition;
        playerValuesDownPos = playerValuesNormalPos;
        playerValuesDownPos.y = -600;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PressedTAB?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            PressedF1?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.F1))
        {
            LetGoF1?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBook();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && bookOpen)
        {
            CloseBook();
        }

        if (Book.Instance != null)
        {
            animLengthBook = Book.Instance.animLength;
        }

    }

    public void ToggleBook()
    {
        if(bookOpen)
        {
            CloseBook();
        }
        else
        {
            OpenBook();
        }
    }

    public void OpenBook()
    {
        Time.timeScale = 0f;
        GameFrozen = true;
        couldAttack = PlayerAttack.Instance.canAttack;
        bookOpen = true;
        playerValues.transform.DOLocalMove(playerValuesDownPos, animLengthBook).SetEase(Ease.InBack).SetUpdate(true);
        book.SetActive(true);
        book.GetComponent<Animator>().SetTrigger("come");
        Book.Instance.BookComeAnim();
        Book.Instance.SetBookStateALL();
        PlayerMovement.instance.canMove = false;
    }

    public void CloseBook()
    {
        GameFrozen = false;
        PlayerAttack.Instance.canAttack = couldAttack;
        Book.Instance.GetBookStateALL();
        Time.timeScale = 1f;
        bookOpen = false;
        playerValues.transform.DOLocalMove(playerValuesNormalPos, animLengthBook).SetEase(Ease.OutBack).SetUpdate(true);
        Animator bookAnim = book.GetComponent<Animator>();
        bookAnim.SetTrigger("go");
        Book.Instance.BookGoAnim();
        PlayerMovement.instance.canMove = true;
        if(PlayerAttack.Instance.sword)
        {
            StartCoroutine(Sword.Instance.ChargeInterrupt());
        }

        close = StartCoroutine(WaitForBookClose());
        if (close != null)
        {
            StopCoroutine(close);
            close = null;
        }





    }

    private IEnumerator WaitForBookClose()
    {
        yield return new WaitForSeconds(Book.Instance.animLength);
        book.SetActive(false);
    }











}
