using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    
    public CharacterStats playerStats;
    private CinemachineFreeLook followCamera;
    public void RegisterPlayer(CharacterStats player)
    {
        playerStats = player;
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = player.transform.GetChild(2);
            followCamera.LookAt = player.transform.GetChild(2);
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.Enter)
            {
                return item.transform;
            }
        }
        return null;
    }
}