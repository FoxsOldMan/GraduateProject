using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThirdSceneGameMode : BaseGameMode
{
    public PlayerStateAndMovement gamePlayer;
    public CameraStateAndMovement gameCamera;

    public PlayerAndCameraController controller;
    public Transform thirdSceneUI;
    public GameObject countDownUI;
    public float sceneTime;

    private float timeLeft;
    private TextMeshProUGUI countDownText;
    private UIState sceneUIState;
    private GameObject loseUI;
    private GameObject pauseUI;

    // Start is called before the first frame update
    void Awake()
    {
        timeLeft = sceneTime;
        countDownText = countDownUI.GetComponentInChildren<TextMeshProUGUI>();
        loseUI = thirdSceneUI.Find("Lose").gameObject;
        pauseUI = thirdSceneUI.Find("Pause").gameObject;

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamePlayer.isAlive() || timeLeft <= 0)
            GameLose();

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
        loseUI.SetActive(sceneUIState == UIState.Lose);
        pauseUI.SetActive(sceneUIState == UIState.Pause);

        countDownUI.SetActive(sceneUIState == UIState.Playing);
    }

    public void StartGame()
    {
        Debug.Log("start");
        sceneUIState = UIState.Playing;
        UpdateUI();
        Time.timeScale = 1;
        controller.ActiveControl();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public override void GameWin()
    {
        base.GameWin();
        sceneUIState = UIState.Win;
        NextScene();
        Time.timeScale = 0;
        controller.StopControl();
        Debug.Log("ThirdSceneWin");
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
