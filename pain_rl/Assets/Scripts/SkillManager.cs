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
    public bool SpawnedF1 = false;
    public bool canUseSkill;
    public bool canGoToCooldown;
    public bool isRunning;
    public bool isInCooldown;
    public GameObject currentUI;
    public GameObject currentF1;
    public TextMeshProUGUI keybind;
    public TextMeshProUGUI keybindF1;
    public TextMeshProUGUI timer;
    private TextMeshProUGUI cooldown;
    public TextMeshProUGUI secondary;
    public GameObject other;
    public GameObject other2;
    public bool requirementsMetForSkill;
    public bool endableSkill;
    public bool canPressButton = true;
    private EntitySFX entitySFX;
    private bool justBecameAvailable;
    public GameObject row;
    private GameObject skillInfosRow1;
    private GameObject skillInfosRow2;
    private GameObject skills;
    private int index;

    private void Awake()
    {
        entitySFX = GetComponent<EntitySFX>();
    }
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        skillInfosRow1 = GameObject.Find("Row1");
        skillInfosRow2 = GameObject.Find("Row2");
        skills = GameObject.Find("Skills");
        elapsedTime = skillCooldown;
        canUseSkill = true;
        canGoToCooldown = true;
        isRunning = false;
        canPressButton = true;
        justBecameAvailable = false;



        int index = transform.GetSiblingIndex();

        if (index <= 2)
        {
            row = skillInfosRow1;
        }
        else
        {
            row = skillInfosRow2;
        }
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
                justBecameAvailable = true;
                if(justBecameAvailable)
                {
                    entitySFX.SkillCharge();
                    justBecameAvailable = false;
                }
            }
        }


        if (Input.GetKeyDown(skillButton) && !EventManager.Instance.GameFrozen)
        {
            if(canUseSkill && requirementsMetForSkill && canPressButton)
            {
                UseSkill?.Invoke();
                elapsedTime = 0f;
                canUseSkill = false;
            }
            else if(isRunning && !canUseSkill && endableSkill)
            {
                EndSkill?.Invoke();
            }
            else
            {
                Debug.Log("Skill can't be used.");
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
                if (timer != null)
                {
                    timer.enabled = true;
                }
                if (keybind != null && keybind.enabled == false)
                {
                    keybind.enabled = true;
                }
            }
            else if (!isInCooldown)
            {
                animUI.SetBool("isInCooldown", false);
                if(keybind != null && keybind.enabled == false)
                {
                    keybind.enabled = true;
                }
            }

            if(!requirementsMetForSkill)
            {
                if(!isRunning)
                {
                    animUI.SetBool("isInCooldown", true);
                    if (timer != null && keybind != null)
                    {
                        if (timer.enabled == true)
                        {
                            timer.enabled = false;
                        }
                        if (keybind.enabled == true)
                        {
                            keybind.enabled = false;
                        }

                    }
                }

                
            }

            if(!canPressButton && !isInCooldown)
            {
                animUI.SetBool("isInCooldown", true);
                if (timer != null && keybind != null)
                {
                    if (timer.enabled == true)
                    {
                        timer.enabled = false;
                    }
                    if (keybind.enabled == true)
                    {
                        keybind.enabled = false;
                    }

                }
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

            if (other2 == null)
            {
                GameObject parent = currentUI.transform.Find("Images").gameObject;
                other2 = parent.transform.Find("Other2").gameObject;
            }

            if (keybindF1 == null)
            {
                keybindF1 = currentF1.transform.Find("Keybind")?.GetComponent<TextMeshProUGUI>();
            }

            if (cooldown == null)
            {
                cooldown = currentF1.transform.Find("Cooldown")?.GetComponent<TextMeshProUGUI>();
            }

            if (secondary == null)
            {
                secondary = currentF1.transform.Find("SecondaryStat")?.GetComponent<TextMeshProUGUI>();
            }

        }




        if (keybind != null)
        {
            keybind.text = skillButton.ToString();
        }

        if(keybindF1 != null)
        {
            keybindF1.text = skillButton.ToString();
        }

        if(timer != null)
        {
            timer.text = Mathf.RoundToInt(skillCooldown - elapsedTime).ToString();
        }

        if(cooldown != null)
        {
            cooldown.text = (skillCooldown.ToString() + "s");
        }


    }

    public void BackToCooldown()
    {
        isRunning = false;
        canGoToCooldown = true;
        elapsedTime = 0f;
        if(keybind != enabled)
        {
            keybind.enabled = true;
        }
       
    }

}
