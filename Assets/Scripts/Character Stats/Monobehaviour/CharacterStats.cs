using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
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
