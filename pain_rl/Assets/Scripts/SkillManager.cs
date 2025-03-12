using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SkillManager : MonoBehaviour
{
    [Header("SKILL VALUES")]
    [SerializeField] public float skillCooldown;
    [SerializeField] public KeyCode skillButton = KeyCode.Q;
    public UnityEvent UseSkill;
    public UnityEvent EndSkill;
    public UnityEvent SkillUI;

    [Space]
    [Header("DONT TOUCH")]
    public GameObject Player;
    private float elapsedTime;
    public bool SpawnedUI = false;
    public bool canUseSkill;
    public bool canGoToCooldown;
    public bool isRunning;
    public bool isInCooldown;
    public GameObject currentUI;
    public TextMeshProUGUI keybind;
    private TextMeshProUGUI timer;
    public GameObject other;


    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        elapsedTime = skillCooldown;
        canUseSkill = true;
        canGoToCooldown = true;
        isRunning = false;
    }

    private void Update()
    {
        


        if (!SpawnedUI)
        {
            SkillUI?.Invoke();
        }


        if (elapsedTime < skillCooldown) //ON COOLDOWN
        {
            canUseSkill = false;
            if(!isRunning)
            {
                isInCooldown = true;
            }
        }
        else
        {
            isInCooldown = false;
        }
            


        if (!canUseSkill && canGoToCooldown)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= skillCooldown)
            {
                elapsedTime = skillCooldown;
                canUseSkill = true;
            }
        }


        if (Input.GetKeyDown(skillButton))
        {
            if(canUseSkill)
            {
                UseSkill?.Invoke();
                elapsedTime = 0f;
                canUseSkill = false;
            }
            else if(isRunning && !canUseSkill)
            {
                EndSkill?.Invoke();
            }
            else
            {
                Debug.Log("Skill is on cooldown.");
            }
            
        }

        if (currentUI != null)
        {
            Animator animUI = currentUI.GetComponent<Animator>();

            if (isRunning)
            {
                animUI.SetBool("isInUse", true);
            }
            else if (!isRunning)
            {
                animUI.SetBool("isInUse", false);
            }

            if (isInCooldown)
            {
                animUI.SetBool("isInCooldown", true);
            }
            else if (!isInCooldown)
            {
                animUI.SetBool("isInCooldown", false);
            }

            if (keybind == null)
            {
                keybind = currentUI.transform.Find("Keybind")?.GetComponent<TextMeshProUGUI>();
            }

            if(timer == null)
            {
                timer = currentUI.transform.Find("CooldownTimer")?.GetComponent<TextMeshProUGUI>();
            }

            if(other == null)
            {
                other = currentUI.transform.Find("Other")?.gameObject;
            }
        }




        if (keybind != null)
        {
            keybind.text = skillButton.ToString();
        }

        if(timer != null)
        {
            timer.text = Mathf.RoundToInt(skillCooldown - elapsedTime).ToString();
        }


    }

    public void BackToCooldown()
    {
        isRunning = false;
        canGoToCooldown = true;
        keybind.enabled = true;
    }
}
