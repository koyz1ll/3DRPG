using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    // [HideInInspector]
    // public CharacterStats playerStats;
    //
    // List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    //
    // public void RegisterPlayer(CharacterStats player)
    // {
    //     playerStats = player;
    // }
    //
    // public void AddObserver(IEndGameObserver observer)
    // {
    //     endGameObservers.Add(observer);
    // }
    //
    // public void RemoveObserver(IEndGameObserver observer)
    // {
    //     endGameObservers.Remove(observer);
    // }
    //
    // public void NotifyObserver()
    // {
    //     foreach (var observer in endGameObservers)
    //     {
    //         observer.EndNotify();
    //     }
    // }
}