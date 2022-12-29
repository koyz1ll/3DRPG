using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI:MonoBehaviour
{
    private Text levelText;
    private Image healthSlider;
    private Image expSlider;

    private void Awake()
    {
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        levelText = transform.GetChild(2).GetComponent<Text>();
    }

    private void Update()
    {
        UpdateLevel();
        UpdateHealth();
        UpdateExp();
    }

    private void UpdateHealth()
    {
        float sliderPercent = (float) GameManager.Instance.playerStats.characterData.currentHealth /
                              GameManager.Instance.playerStats.characterData.maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    private void UpdateExp()
    {
        float sliderPercent = (float) GameManager.Instance.playerStats.characterData.currentExp /
                              GameManager.Instance.playerStats.characterData.baseExp;
        expSlider.fillAmount = sliderPercent;
    }

    private void UpdateLevel()
    {
        levelText.text = "Level " + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
    }
}