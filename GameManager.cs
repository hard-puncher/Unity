using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public string[] enemyObjs;
    public Transform[] spawnPoints;
    public GameObject player;

    //UI 컴포넌트들의 객체를 모두 생성한다.
    public Text scoreText;
    public Image[] lifeImages;
    public Image[] boomImages;
    public GameObject gameOverSet;
    public ObjectManager objectManager;

    public float curSpawnDelay;
    public float maxSpawnDelay;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[]{ "EnemyL", "EnemyM", "EnemyS"};
    }

    void ReadSpawnFile()
    {
        //#1. 변수 초기화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2. 리스폰 파일 읽기
        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while(stringReader != null)
        {
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            //#3. 리스폰 데이터 생성
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }     
    }
    void Update()
    {
        curSpawnDelay += Time.deltaTime;
        if(curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f); //최대스폰딜레이는 0.5초~3초 사이의 랜덤한 값
            curSpawnDelay = 0;  //소환 하고나면 현재스폰딜레이를 다시 0으로 초기화
        }

        //UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);  //0부터 3사이 -> 0, 1, 2 중 하나의 값이 담김
        int ranPoint = Random.Range(0, 9);  //9개의 랜덤 포인트 값을 담음
        GameObject enemy = objectManager.MakeObj(enemyObjs[ranEnemy]);
        enemy.transform.position = spawnPoints[ranPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.objectManager = objectManager;

        if(ranPoint == 5 || ranPoint == 6)  //오른쪽 스폰
        {
            enemy.transform.Rotate(Vector3.back * 90);
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if(ranPoint == 7 || ranPoint == 8) //왼쪽 스폰
        {
            enemy.transform.Rotate(Vector3.forward * 90);
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else                    //위쪽 스폰
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed*(-1));
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = new Vector3(0, -3, 0);
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void UpdateLifeIcon(int life)
    {
        //UI Life Init Disabled
        for (int index = 0; index < 3; index++)
        {
            lifeImages[index].color = new Color(1, 1, 1, 0);
        }

        //UI Life Active
        for (int index = 0; index < life; index++)
        {
            lifeImages[index].color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        //UI Boom Init Disabled
        for (int index = 0; index < 3; index++)
        {
            boomImages[index].color = new Color(1, 1, 1, 0);
        }

        //UI Boom Active
        for (int index = 0; index < boom; index++)
        {
            boomImages[index].color = new Color(1, 1, 1, 1);
        }
    }
    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);  
    }
}
