using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    Slider staminaBar;
    Slider hpBar;
    Slider manaBar;
    PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        staminaBar = transform.Find("StaminaBar").GetComponent<Slider>();
        hpBar = transform.Find("HPBar").GetComponent<Slider>();
        manaBar = transform.Find("ManaBar").GetComponent<Slider>();
    }
    // Update is called once per frame
    void Update()
    {
        staminaBar.value = pc.GetStaminaPercentage();
        if(pc.GetStaminaPercentage() > .99f) { staminaBar.gameObject.SetActive(false); }
        else { staminaBar.gameObject.SetActive(true); }
        hpBar.value = pc.GetHPPercentage();
        if (pc.GetHPPercentage() > .99f) { hpBar.gameObject.SetActive(false); }
        else { hpBar.gameObject.SetActive(true); }
        manaBar.value = pc.ManaPercentage;
        if(pc.ManaPercentage > 0.99f) { manaBar.gameObject.SetActive(false); }
        else { manaBar.gameObject.SetActive(true); }
    }
}
