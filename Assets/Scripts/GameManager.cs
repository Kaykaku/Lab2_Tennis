using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int timeRound = 60;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI botText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Button restartButton;
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject[] walls;

    private int timer;
    private int playerScore;
    private int botScore;
    public bool playerTurn;
    public GameObject ballPrefab;
    public GameObject botSpawnBall;
    public GameObject playerSpawnBall;
    public bool isGameActive;
    public bool hasWall;
    public int difficulty;


    public void StartGame(int difficult , string mode, bool hasWall)
    {
        //Setting StartGame 
        //Includes difficulty, round time, AI bot, default score
        //Start countdown time
        //Spawn ball
        //Hide title Screen
        this.difficulty = difficult;
        this.hasWall = hasWall;
        this.playerTurn = hasWall;
        timer = timeRound;
        isGameActive = true;
        modeText.text = "Mode : " + mode;

        botSpawnBall.transform.parent.GetComponent<BotController>().SetBotDifficulty(difficulty);
        AddPlayerScore(0);
        AddBotScore(0);
        SetTimerText();
        SpawnBall();
        StartCoroutine(CountDown());
        ShowWall();

        titleScreen.SetActive(false);
        infoText.gameObject.SetActive(false);
    }

    IEnumerator CountDown()
    {
        //Count down the round time
        //Time to 0 ends the game
        while (timer > 0)
        {
            yield return new WaitForSeconds(1f);
            timer--;
            SetTimerText();
        }
        GameOver();
    }

    IEnumerator SpawnBallAfter()
    {
        //Spawn ball after 2 seconds
        //Spawn position alternates between Player and Bot
        yield return new WaitForSeconds(2f);

        infoText.gameObject.SetActive(false);
        Vector3 pos;
        if (playerTurn) pos = playerSpawnBall.transform.position;
        else pos = botSpawnBall.transform.position;
        Instantiate(ballPrefab, pos, ballPrefab.transform.rotation);
    }

    void ShowWall()
    {
        if (!hasWall) return;
        foreach (GameObject o in walls)
        {
            o.SetActive(!o.activeSelf);
        }
        Camera.main.transform.position = new Vector3(0,18f,-15f);
        PlayerController playerController = playerSpawnBall.transform.parent.GetComponent<PlayerController>();
        playerController.xRange =13f;
        playerController.ZMinPos = -15f;
    }

    void SetTimerText()
    {
        //Set timer text
        timerText.text = "Time : " + timer;
    }

    public void AddPlayerScore(int score)
    {
        //Add points for Player
        //Change Spawn location
        //Show text Player scores
        playerScore += score;
        playerText.text = "Player : " + playerScore;
        if(!hasWall) playerTurn = !playerTurn;
        infoText.gameObject.SetActive(true);
        infoText.text = "Player Score";
        infoText.color = Color.blue;
    }
    public void AddBotScore(int score)
    {
        //Add points for Bot
        //Change Spawn location
        //Show text Bot scores
        botScore += score;
        botText.text = "Bot : " + botScore;
        if (!hasWall) playerTurn = !playerTurn;
        infoText.gameObject.SetActive(true);
        infoText.text = "Bot Score";
        infoText.color = Color.red;
    }

    public void SpawnBall()
    {
        //Spawn ball if Game not end
        if (!isGameActive) return;
        StartCoroutine(SpawnBallAfter());
    }

    public void GameOver()
    {
        //Game over
        //Show match result
        //Show the replay button
        infoText.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        isGameActive = false;
        if (playerScore > botScore) gameOverText.text = "Player Win!";
        else if (playerScore < botScore) gameOverText.text = "Bot Win!";
        else gameOverText.text = "Draw";
    }

    // Restart game by reloading the scene
    public void RestartGame()
    {
        //Reload Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
