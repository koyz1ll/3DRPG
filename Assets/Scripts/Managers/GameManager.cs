using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public CharacterStats playerStats;

    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
}