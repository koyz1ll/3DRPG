using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;

    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
    }
}