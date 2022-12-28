using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{

    public Action<int, int> UpdateHealthBarOnAttac;
    public CharacterData_SO tempCharacterData;
    public AttackData_SO tempAttackData;
    
    [HideInInspector]
    public CharacterData_SO characterData;
    [HideInInspector]
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {
        if (tempCharacterData != null)
        {
            characterData = Instantiate(tempCharacterData);
        }

        if (tempAttackData != null)
        {
            attackData = Instantiate(tempAttackData);
        }
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth;else return 0; }
        set { characterData.maxHealth = value; }
    }
    
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth;else return 0; }
        set { characterData.currentHealth = value; }
    }
    
    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence;else return 0; }
        set { characterData.baseDefence = value; }
    }
    
    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence;else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Math.Max(0, attacker.CurrentDamage() - defener.CurrentDefence);
        CurrentHealth = Math.Max(0, CurrentHealth - damage);
        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        UpdateHealthBarOnAttac?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);   
        }
    }

    public void TakeDamage(int damage, CharacterStats defener)
    {
        int currentDamage = Math.Max(0, damage - defener.CurrentDefence);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarOnAttac?.Invoke(CurrentHealth, MaxHealth);
        
        GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
    }

    private int CurrentDamage()
    {
        float coreDamage = Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("暴击！" + coreDamage);
        }
        return (int) coreDamage;
    }

    #endregion
}
