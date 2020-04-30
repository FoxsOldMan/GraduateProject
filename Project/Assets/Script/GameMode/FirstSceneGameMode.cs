using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstSceneGameMode : BaseGameMode
{
    public PlayerStateAndMovement gamePlayer;
    public CameraStateAndMovement gameCamera;

    public PlayerAndCameraController controller;
    public Transform firstSceneUI;
    public float sceneTime;
    public GameObject countDownUI;
    private TextMeshProUGUI countDownText;
    private float timeLeft;

    public GameObject enemyCreater;

    private UIState sceneUIState;
    private GameObject mainMenuUI;
    private GameObject introduceUI;
    private GameObject controlUI;
    private GameObject winUI;
    private GameObject loseUI;
    private GameObject pauseUI;

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = sceneTime;
        countDownUI.SetActive(false);
        countDownText = countDownUI.GetComponentInChildren<TextMeshProUGUI>();
        enemyCreater.SetActive(false);

        mainMenuUI = firstSceneUI.Find("MainMenu").gameObject;
        introduceUI = firstSceneUI.Find("Introduce").gameObject;
        controlUI = firstSceneUI.Find("Control").gameObject;
        winUI = firstSceneUI.Find("Win").gameObject;
        loseUI = firstSceneUI.Find("Lose").gameObject;
        pauseUI = firstSceneUI.Find("Pause").gameObject;

        ShowMainMenu(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamePlayer.isAlive())
            GameLose();

        if (timeLeft <= 0)
            GameWin();

        if (sceneUIState == UIState.Playing)
        {
            timeLeft -= Time.deltaTime;
            countDownText.text = ((int)timeLeft).ToString();


            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
                GamePause();
        }


    }

    private void UpdateUI()
    {
        mainMenuUI.SetActive(sceneUIState == UIState.MainMenu);
        introduceUI.SetActive(sceneUIState == UIState.Introduce);
        controlUI.SetActive(sceneUIState == UIState.Control);
        winUI.SetActive(sceneUIState == UIState.Win);
        loseUI.SetActive(sceneUIState == UIState.Lose);
        pauseUI.SetActive(sceneUIState == UIState.Pause);

        countDownUI.SetActive(sceneUIState == UIState.Playing);
        enemyCreater.SetActive(sceneUIState == UIState.Playing);
    }

    public void ShowMainMenu()
    {
        sceneUIState = UIState.MainMenu;
        UpdateUI();
        Time.timeScale = 1;
        controller.StopControl();
    }

    public void StartGame()
    {
        sceneUIState = UIState.Playing;
        UpdateUI();
        Time.timeScale = 1;
        controller.ActiveControl();
    }

    public void ShowIntroduce()
    {
        sceneUIState = UIState.Introduce;
        UpdateUI();
    }

    public void ShowControl()
    {
        sceneUIState = UIState.Control;
        UpdateUI();
    }

    public void BackPlaying()
    {
        sceneUIState = UIState.Playing;
        UpdateUI();
        Time.timeScale = 1;
        controller.ActiveControl();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public override void GameWin()
    {
        base.GameWin();
        sceneUIState = UIState.Win;
        UpdateUI();
        Time.timeScale = 0;
        controller.StopControl();
    }

    public override void GameLose()
    {
        base.GameLose();
        sceneUIState = UIState.Lose;
        UpdateUI();
        Time.timeScale = 0;
        controller.StopControl();
    }

    public override void GamePause()
    {
        base.GamePause();
        sceneUIState = UIState.Pause;
        UpdateUI();
        pauseUI.SetActive(true);
        Time.timeScale = 0;
        controller.StopControl();
    }
}

enum UIState
{
    MainMenu,
    Introduce,
    Control,
    Win,
    Lose,
    Pause,
    Playing
}
