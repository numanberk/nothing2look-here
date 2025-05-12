using TMPro;
using UnityEngine;

public class SwordBookValues : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI damage;
    [SerializeField] public TextMeshProUGUI speed;
    [SerializeField] public TextMeshProUGUI critChance;
    //[SerializeField] public TextMeshProUGUI turn1;
    [Space]
    [Space]
    [SerializeField] public TextMeshProUGUI damage2;
    [SerializeField] public TextMeshProUGUI speed2;
    [SerializeField] public TextMeshProUGUI moveSpeedDebuff;
    //[SerializeField] public TextMeshProUGUI critChance2;
    //[SerializeField] public TextMeshProUGUI turn2;

    private bool hasFound;

    private void Start()
    {
        hasFound = false;
    }

    private void Update()
    {
        if (!hasFound && Sword.Instance != null)
        {
            hasFound = true;

            damage.text = Sword.Instance.attackDamage.ToString();
            damage2.text = Mathf.RoundToInt(Sword.Instance.attackDamage * Sword.Instance.thirdHitDamageMultiplier).ToString() + "-" + Mathf.RoundToInt(Sword.Instance.attackDamage * Sword.Instance.thirdHitDamageMultiplier * Sword.Instance.maxChargeDamageMultiplier).ToString();


            speed.text = (Sword.Instance.attackCooldown).ToString() + "s"; //"F2" ekle tostring() parantezinin içine eðer çok virgüllü bir sayý olduysa
            speed2.text = (Sword.Instance.thirdHitAnimLength + Sword.Instance.delayAfterThirdHit).ToString() + "-" + (Sword.Instance.maxChargeInSeconds + Sword.Instance.thirdHitAnimLength + Sword.Instance.delayAfterThirdHit).ToString() + "s"; //NERD

            critChance.text = Mathf.RoundToInt(Sword.Instance.critChance * 100).ToString() + "%";
            moveSpeedDebuff.text = "-" + Mathf.RoundToInt(100 - (Sword.Instance.speedFallOffWhileCharging * 100)).ToString() + "%";
            //critChance2.text = Mathf.RoundToInt(Sword.Instance.critChance * Punch.Instance.critChanceMultiplier * 100).ToString() + "%";

            //turn1.text = Mathf.RoundToInt(1 / Punch.Instance.addChargeEachPunch).ToString();
            //turn2.text = Mathf.RoundToInt(1 / Punch.Instance.substractChargeEachPunch).ToString();
        }
    }
}
