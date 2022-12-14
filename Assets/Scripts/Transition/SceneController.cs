using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;
    private GameObject player;
    private NavMeshAgent playerAgent;
    public SceneFader sceneFaderPrefab;
    protected override void Awake()
    {
        base.Awake();
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string name, TransitionDestination.DestinationTag destinationTag)
    {
        //TODO:保存数据
        SaveManager.Instance.SavePlayerData();
        
        if (SceneManager.GetActiveScene().name != name)
        {
            yield return SceneManager.LoadSceneAsync(name);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
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

    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("SampleScene"));
    }

    IEnumerator LoadLevel(string name)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        if (name != "")
        {
            yield return StartCoroutine(fade.FadeOut(2.5f));
            yield return SceneManager.LoadSceneAsync(name);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position,
                GameManager.Instance.GetEntrance().rotation);
            //保存游戏
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2.5f));
            yield break;
        }

    }

    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }
    IEnumerator LoadMain()
    {
        yield return SceneManager.LoadSceneAsync("MainScene");
    }
}
