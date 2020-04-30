using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalSceneGameMode : BaseGameMode
{
    public PlayerStateAndMovement gamePlayer;
    public CameraStateAndMovement gameCamera;

    public DogBossStateAndAction dogBoss;
    public Canvas bossUI;
    private Slider bossHPBar;

    public PlayerAndCameraController controller;
    public Transform finalSceneUI;

    private UIState sceneUIState;
    private GameObject winUI;
    private GameObject loseUI;
    private GameObject pauseUI;

    // Start is called before the first frame update
    void Start()
    {

        winUI = finalSceneUI.Find("Win").gameObject;
        loseUI = finalSceneUI.Find("Lose").gameObject;
        pauseUI = finalSceneUI.Find("Pause").gameObject;

        dogBoss.gameObject.SetActive(false);
        bossUI.gameObject.SetActive(false);
        bossHPBar = bossUI.GetComponentInChildren<Slider>();
        bossHPBar.value = 0f;

        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamePlayer.isAlive())
            GameLose();

        if (dogBoss.gameObject.activeSelf)
            bossHPBar.value = dogBoss.hp / dogBoss.maxHP;

        if (sceneUIState == UIState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
                GamePause();
        }
    }

    private void UpdateUI()
    {
        winUI.SetActive(sceneUIState == UIState.Win);
        loseUI.SetActive(sceneUIState == UIState.Lose);
        pauseUI.SetActive(sceneUIState == UIState.Pause);
    }

    public void StartGame()
    {
        sceneUIState = UIState.Playing;
        UpdateUI();
        Time.timeScale = 1;
        controller.ActiveControl();
        StartCoroutine(ActiveBoss());
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

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
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

    private IEnumerator ActiveBoss()
    {
        bossUI.gameObject.SetActive(true);
        while (bossHPBar.value < 1)
        {
            bossHPBar.value += Time.deltaTime;
            Debug.Log("准备中");
            yield return new WaitForEndOfFrame();
        }

        dogBoss.gameObject.SetActive(true);
        StartCoroutine(WaitForBossDead());
        yield break;
    }

    private IEnumerator WaitForBossDead()
    {
        while (dogBoss.isAlive)
        {
            yield return new WaitForEndOfFrame();
        }

        float curTime = 0;
        while (curTime < 3)
        {
            curTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        GameWin();
        yield break;
    }
}
