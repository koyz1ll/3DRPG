using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Data", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float cooldown;
    public float minDamage;
    public float maxDamage;

    //暴击
    public float criticalMultiplier;//暴击加成百分比
    public float criticalChance;//暴击几率
}
