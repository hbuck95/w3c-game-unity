using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class waveManager : MonoBehaviour {

    public Button waveStartButton;
    public GameObject waveStartButtonobj;
    public GameObject enemies;
    public GameObject endGameobj;
    public Transform startPoint;
    public Text enemyCountTxt, timeTxt;
    public static int enemyCount;
    public static float timeLeft;
    private bool isWaveStarted, isGameEnd, hasTimeRunOut, isBuildStarted;
    public static waveManager Instance;

	// Use this for initialization
	void Start () {

        waveStartButton.onClick.AddListener(Wrapper);
        timeLeft = 15;
        isWaveStarted = false;
        Instance = this;
        hasTimeRunOut = false;
		
	}
	
	// Update is called once per frame
	void Update () {

        enemyCountTxt.text = "Enemies killed: " + enemyCount;
        timeTxt.text = "Time left: " + Mathf.Round(timeLeft);

        if (isBuildStarted == true)
        timeLeft -= Time.deltaTime;

        if (isWaveStarted == true)
        {
            turretManager.Instance.canBuild = false;
        }

        if (enemyCount >= 100)
        {
            endGame();
        }

	}

    void Wrapper ()
    {
        StartCoroutine(startWave());
        waveStartButtonobj.SetActive(false);
    }

    IEnumerator startWave()
    {
        isBuildStarted = true;
        turretManager.Instance.canBuild = true;
        yield return new WaitForSeconds(timeLeft);
        isBuildStarted = false;
        isWaveStarted = true;
        if (isWaveStarted == true)
        {
            turretManager.Instance.canBuild = false;
            for (int i = 0; i < 12 && isWaveStarted == true; i++)
            {
                Instantiate(enemies, startPoint.localPosition, Quaternion.identity);
                yield return new WaitForSeconds(1);
            }
        }
        isWaveStarted = false;
       
    }

    private GameObject[] enemyPool, turretPool;

    public void endWave()
    {
        enemyPool = GameObject.FindGameObjectsWithTag("NPC");

        for (int i = 0; i < enemyPool.Length; i++)
        {
            Destroy(enemyPool[i]);
        }

        turretPool = GameObject.FindGameObjectsWithTag("turret");

        for (int i = 0; i < turretPool.Length; i++)
        {
            Destroy(turretPool[i]);
        }

        turretManager.Instance.bTurretAmount = 2;
        turretManager.Instance.sTurretAmount = 4;

        waveStartButtonobj.SetActive(true);

        if (hasTimeRunOut == false){
            RobotDialogue.Instance.QueueDialogue("Oh no! It didn't look like you had enough time to build turrets!");
            RobotDialogue.Instance.QueueDialogue("Don't worry. I've given you some extra time. You now have 35 seconds to build all 6 of your turrets");
            RobotDialogue.Instance.QueueDialogue("Just click the 'Start Game' button to start again");
            RobotDialogue.Instance.StartDialogue();
        }

        isWaveStarted = false;
        enemyCount = 0;
        timeLeft = 35;

        if (hasTimeRunOut == true){
            RobotDialogue.Instance.QueueDialogue("The enemies made it to the end of the stage!");
            RobotDialogue.Instance.QueueDialogue("Maybe you should try out a new layout of turrets");
            RobotDialogue.Instance.StartDialogue();
        }

        hasTimeRunOut = true;
    }

    public void endGame()
    {
        enemyPool = GameObject.FindGameObjectsWithTag("NPC");

        for (int i = 0; i < enemyPool.Length; i++)
        {
            Destroy(enemyPool[i]);
        }

        endGameobj.SetActive(true);
    }
}
