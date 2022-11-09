using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

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
        
        //TODO:Update UI
        //TODO:经验update
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
