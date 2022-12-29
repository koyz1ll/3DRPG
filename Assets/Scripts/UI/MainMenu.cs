using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button newGameBtn;
    public Button continueBtn;
    public Button quitBtn;

    private PlayableDirector director;
    // Start is called before the first frame update
    void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();
        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;
        
        newGameBtn.onClick.AddListener(PlayTimeLine);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    void PlayTimeLine()
    {
        director.Play();
    }

    void NewGame(PlayableDirector playableDirector)
    {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.LogWarning("退出游戏");
    }
}
