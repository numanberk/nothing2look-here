using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoBook : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI keybind;
    [SerializeField] public TextMeshProUGUI cooldown;
    [SerializeField] public TextMeshProUGUI secondary;

    public GameObject thisInfosButton;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }


}
