using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI:MonoBehaviour
{ 
    public GameObject healthUIPrefab;
    public Transform barPoint;

    private Image healthSlider;
    private Transform UIBar;

    private Transform cam;
    private CharacterStats currentStats;

    public bool alwaysVisible;

    public float visibleTime;

    private float timeLeft;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();
        currentStats.UpdateHealthBarOnAttac += UpdateHealthBar;
    }

    private void LateUpdate()
    {
        if (UIBar != null)
        {
            UIBar.position = barPoint.position;
            UIBar.transform.forward = cam.forward;
            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIBar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                UIBar = Instantiate(healthUIPrefab, canvas.transform).transform;
                healthSlider = UIBar.GetChild(0).GetComponent<Image>();
                UIBar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
        {
            Destroy(UIBar.gameObject);
        }
        UIBar.gameObject.SetActive(true);
        timeLeft = visibleTime;
        float sliderPercent =(float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
}