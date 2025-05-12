using UnityEngine;
using UnityEngine.UI;

public class Crystal : MonoBehaviour
{
    [SerializeField] private GameObject rangeSprite; //range = 25 iken scale == 95/95
    [SerializeField] public Slider Timer;
    private EntitySFX entitySFX;


    private void Awake()
    {
        entitySFX = GetComponent<EntitySFX>();
    }
    private void Start()
    {
        float newScale = 93f / 25f * ShatterdashSkill.Instance.dashRange; //forgiving = 93
        rangeSprite.transform.localScale = new Vector3(newScale, newScale, rangeSprite.transform.localScale.z);
        entitySFX.Glow();
    }
}
