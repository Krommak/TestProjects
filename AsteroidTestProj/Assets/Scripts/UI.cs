using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private GameMod gameMod;
    public GameObject menuPanel;
    public GameObject player;
    public GameObject endGamePanel;
    [SerializeField] GameObject resumeButton;
    [SerializeField] Text scoreText, maxScoreText, settingText;
    private int score = 0;

    void Awake()
    {
        gameMod = GameObject.Find("GameMod").GetComponent<GameMod>();
    }

    void Start()
    {
        scoreText.text = "Score: " + score;
        maxScoreText.text = "MaxScore: " + PlayerPrefs.GetInt("MaxScore");
        
        int newType = PlayerPrefs.GetInt("ControlType") == 0 ? 1 : PlayerPrefs.GetInt("ControlType");
        if(newType == 1)
        {
            settingText.text = "Управление Клавиатура";
        }
        else
        {
            settingText.text = "Управление Клавиатура + Мышь";
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && gameMod.isStart)
        {
            StopGame();
        }
    }

    public void StartGame()
    {   
        gameMod.RevertGame();
        menuPanel.SetActive(false);
        gameMod.isStart = true;
        gameMod.SpawnAsteroids("big");
        StartCoroutine(gameMod.RespawnUFODelay());
        StartCoroutine(player.GetComponent<PlayerController>().PlayerStart());
    }

    private void StopGame()
    {
        menuPanel.SetActive(true);
        resumeButton.SetActive(true);
        gameMod.isStart = false;

        gameMod.GamePause(true);
    }

    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        gameMod.isStart = true;

        gameMod.GamePause(false);
    }

    public IEnumerator EndGame()
    {
        yield return new WaitForSecondsRealtime(1f);
        endGamePanel.SetActive(true);
        if(PlayerPrefs.GetInt("MaxScore") < score)
        {
            PlayerPrefs.SetInt("MaxScore", score);
        }
    }

    public void SetScore(GameObject collisionObj, GameObject bullet)
    {
        if (bullet.GetComponent<Bullet>().GetOwner())
        {
            if (collisionObj.tag == "Asteroid")
            {
                switch (collisionObj.GetComponent<Asteroid>().GetAsteroidType())
                {
                    case "big":
                        score+=20;
                        CheckScore();
                        break;
                    case "medium":
                        score+=50;
                        CheckScore();
                        break;
                    case "small":
                        score+=100;
                        CheckScore();
                        break;
                }
            }
            else if (collisionObj.tag == "UFO")
            {
                score+=200;
                CheckScore();
            }
        }
    }

    void CheckScore()
    {
        scoreText.text = score.ToString();
    }

    public void ChangeControl()
    {
        GameObject.Find("Player").GetComponent<PlayerController>().ChangeControlType();

        int newType = PlayerPrefs.GetInt("ControlType") == 0 ? 1 : PlayerPrefs.GetInt("ControlType");
        if(newType == 1)
        {
            settingText.text = "Управление Клавиатура";
        }
        else
        {
            settingText.text = "Управление Клавиатура + Мышь";
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
