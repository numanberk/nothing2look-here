using UnityEngine;

public class SortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public SortingPoint sortingPoint;

    [SerializeField] int changeOrder;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    private void Start()
    {
        sortingPoint = GetComponentInParent<SortingPoint>();

    }
    void Update()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = Mathf.RoundToInt(sortingPoint.transform.position.y * -100) + changeOrder;
        }
    }
}
