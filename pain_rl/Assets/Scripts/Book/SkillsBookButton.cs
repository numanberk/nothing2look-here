using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillsBookButton : MonoBehaviour
{
    [Header("DONT TOUCH")]
    [SerializeField] public GameObject infoPrefab;
    [SerializeField] public GameObject mainSkillScript;
    private bool instantiated = false;
    public GameObject infoObj;

    private void Update()
    {
        if (!instantiated)
        {
            InstantiateInfo();
            instantiated = true;
        }
    }

    private void Start()
    {
        GetComponent<Animator>().SetBool("colored", false);


    }

    void InstantiateInfo()
    {
        if (infoObj == null && Book.Instance != null)
        {
            instantiated = true;
            infoObj = Instantiate(infoPrefab, Book.Instance.P2RightContainer.transform);
            infoObj.transform.localPosition = Vector3.zero;


            var skillInfo = infoObj.GetComponent<SkillInfoBook>();
            var skillManager = mainSkillScript.GetComponent<SkillManager>();

            skillInfo.thisInfosButton = this.gameObject;

            skillInfo.keybind.text = skillManager.keybind.text;
            skillInfo.cooldown.text = skillManager.skillCooldown.ToString() + "s";

            if (skillInfo.secondary != null)
                skillInfo.secondary.text = skillManager.secondary.text;
        }
    }
    public void ButtonPressed()
    {
        
        List<SkillInfoBook> found = new List<SkillInfoBook>();
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            SkillInfoBook[] scriptsInChildren = obj.GetComponentsInChildren<SkillInfoBook>(true); // true = include inactive
            found.AddRange(scriptsInChildren);
        }

        SkillInfoBook mySkillInfo = infoObj?.GetComponent<SkillInfoBook>();

        foreach (SkillInfoBook script in found)
        {
            if (script != mySkillInfo)
            {
                script.thisInfosButton.GetComponentInChildren<Button>().interactable = true;
                script.thisInfosButton.GetComponent<Animator>().SetBool("colored", false);
                script.gameObject.SetActive(false);
            }
                

        }

        if (mySkillInfo != null)
        {
            mySkillInfo.gameObject.SetActive(true);
            mySkillInfo.gameObject.GetComponent<Animator>().SetTrigger("come");
            mySkillInfo.thisInfosButton.GetComponentInChildren<Button>().interactable = false;
            mySkillInfo.thisInfosButton.GetComponent<Animator>().SetBool("colored", true);
        }

        Book.Instance.GetBookState2(this.gameObject);
            
    }


}
