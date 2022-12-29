using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class SceneController : Singleton<SceneController>
{
    private GameObject player;
    private NavMeshAgent playerAgent;
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                
                break;
        }
    }

    IEnumerator Transition(string name, TransitionDestination.DestinationTag destinationTag)
    {
        player = GameManager.Instance.playerStats.gameObject;
        playerAgent = player.GetComponent<NavMeshAgent>();
        playerAgent.enabled = false;
        player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);
        playerAgent.enabled = true;
        yield return null;
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        foreach (var entrance in entrances)
        {
            if (entrance.destinationTag == destinationTag) return entrance;
        }
        return null;
    }
}
