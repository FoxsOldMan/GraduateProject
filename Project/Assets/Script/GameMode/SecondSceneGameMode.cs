using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SecondSceneGameMode : BaseGameMode
{
    public PlayerStateAndMovement gamePlayer;
    public CameraStateAndMovement gameCamera;

    public Transform enemyGroup;
    private int maxEnemy;
    private int enemyLeft;

    public HumanBossStateAndAction humanBoss;
    public Canvas bossUI;
    private Slider bossHPBar;

    public PlayerAndCameraController controller;
    public Transform secondSceneUI;

    private UIState sceneUIState;
    private GameObject winUI;
    private GameObject loseUI;
    private GameObject pauseUI;

    // Start is called before the first frame update
    void Start()
    {
        maxEnemy = enemyGroup.childCount;
        enemyLeft = maxEnemy;

        winUI = secondSceneUI.Find("Win").gameObject;
        loseUI = secondSceneUI.Find("Lose").gameObject;
        pauseUI = secondSceneUI.Find("Pause").gameObject;

        humanBoss.gameObject.SetActive(false);
        bossUI.gameObject.SetActive(false);
        bossHPBar = bossUI.GetComponentInChildren<Slider>();
        bossHPBar.value = 0f;

        StartGame();
        StartCoroutine(WaitForBossDead());
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamePlayer.isAlive())
            GameLose();

        //if (!humanBoss.isAlive)
        //    GameWin();


        if(humanBoss.gameObject.activeSelf)
            bossHPBar.value = humanBoss.hp / humanBoss.maxHP;

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
        StartCoroutine(WaitUntillAllEnemyDown());
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

    public void EnemyDown(string name)
    {
        Debug.Log(name + " was defeated");
        enemyLeft--;
    }

    private IEnumerator WaitUntillAllEnemyDown()
    {
        while (enemyLeft > 0)
        {
            yield return new WaitForEndOfFrame();
        }

        bossUI.gameObject.SetActive(true);
        while(bossHPBar.value < 1)
        {
            bossHPBar.value += Time.deltaTime;
            Debug.Log("准备中");
            yield return new WaitForEndOfFrame();
        }

        humanBoss.gameObject.SetActive(true);
        yield break;
    }

    private IEnumerator WaitForBossDead()
    {
        while (humanBoss.isAlive)
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

