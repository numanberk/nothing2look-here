using TMPro;
using UnityEngine;

public class PunchBookValues : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI damage;
    [SerializeField] public TextMeshProUGUI punchesInARow;
    [SerializeField] public TextMeshProUGUI speed;
    [SerializeField] public TextMeshProUGUI cooldown;
    [SerializeField] public TextMeshProUGUI critChance;
    [SerializeField] public TextMeshProUGUI turn1;
    [Space]
    [Space]
    [SerializeField] public TextMeshProUGUI damage2;
    [SerializeField] public TextMeshProUGUI punchesInARow2;
    [SerializeField] public TextMeshProUGUI speed2;
    [SerializeField] public TextMeshProUGUI cooldown2;
    [SerializeField] public TextMeshProUGUI critChance2;
    [SerializeField] public TextMeshProUGUI turn2;

    private bool hasFound;

    private void Start()
    {
        hasFound = false;
    }

    private void Update()
    {
        if(!hasFound && Punch.Instance != null)
        {
            hasFound = true;

            damage.text = Punch.Instance.attackDamage.ToString();
            damage2.text = Mathf.RoundToInt(Punch.Instance.attackDamage * Punch.Instance.chargedDamageMultiplier).ToString();

            punchesInARow.text = Punch.Instance.numberOfAttacks.ToString();
            punchesInARow2.text = Punch.Instance.chargedNumberOfAttacks.ToString();

            speed.text = (Punch.Instance.attackCooldown / Punch.Instance.numberOfAttacks).ToString() + "s";
            speed2.text = (Punch.Instance.attackCooldown * Punch.Instance.chargedNumberOfAttacks / Punch.Instance.numberOfAttacks / Punch.Instance.chargedNumberOfAttacks).ToString() + "s";

            cooldown.text = (Punch.Instance.delayBetweenAttacks).ToString() + "s";
            cooldown2.text = (Punch.Instance.delayBetweenAttacks).ToString() + "s";

            critChance.text = Mathf.RoundToInt(Punch.Instance.critChance * 100).ToString() + "%";
            critChance2.text = Mathf.RoundToInt(Punch.Instance.critChance * Punch.Instance.critChanceMultiplier * 100).ToString() + "%";

            turn1.text = Mathf.RoundToInt(1 / Punch.Instance.addChargeEachPunch).ToString();
            turn2.text = Mathf.RoundToInt(1 / Punch.Instance.substractChargeEachPunch).ToString();
        }
    }

    public void SwitchedToBlaze()
    {
        Animator anim = Punch.Instance.instantiatedBookImage.GetComponent<Animator>();
        anim.SetBool("Blaze", true);
    }

    public void SwitchedToNormal()
    {
        Animator anim = Punch.Instance.instantiatedBookImage.GetComponent<Animator>();
        anim.SetBool("Blaze", false);
    }
}
