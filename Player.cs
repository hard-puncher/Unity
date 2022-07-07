using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //컴포넌트 객체 생성
    Animator animator;

    //플레이어 관련
    public float playerSpeed;
    public int playerPower;
    public int maxPower;
    private float hMove;
    private float vMove;
    //필살기 관련
    public int maxBoom;
    public int boom;

    //2발 맞을때 목숨 2개깎임 현상 방지
    public bool isHit;
    //필살기 시전 중인지 감지
    public bool isBoomTime;

    //UI관련
    public int life;
    public int score;

    //총알
    public GameObject bulletObjA;
    public GameObject bulletObjB;

    //필살기
    public GameObject boomEffect;

    public GameManager gameManager;
    public ObjectManager objectManager;
    public float maxShotDelay;
    public float curShotDelay;

    //화면 밖으로 나감 방지
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();    
        PlayerFire();
        Boom();
    }

    //플레이어 이동
    void PlayerMove()
    {
        //좌, 우 이동
        hMove = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && hMove == 1) || (isTouchLeft && hMove == -1))
        {
            hMove = 0;
        }
        //상, 하 이동
        vMove = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && vMove == 1) || (isTouchBottom && vMove == -1))
        {
            vMove = 0;
        }

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(hMove, vMove, 0) * playerSpeed * Time.deltaTime;

        transform.position = curPos + nextPos;

        //좌우 이동 애니메이션
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            animator.SetInteger("Input", (int)hMove);
        }
    }

    void PlayerFire()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            curShotDelay += Time.deltaTime;

            switch(playerPower)
            {
                //power 0 (기본 1발)
                case 0:
                    GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                    bullet.transform.position = transform.position;
                 
                    Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();    //AddForce로 발사하기 위해 bullet객체에 Rigidbody2D 컴포넌트 연결
                    rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    break;
                //power 1 (2발)
                case 1:
                    GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                    bulletR.transform.position = transform.position + Vector3.right*0.1f;

                    GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                    bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                    Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                    Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                    rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    break;
                //power 2 (3발)
                case 2:
                    GameObject bulletRight = objectManager.MakeObj("BulletPlayerA");
                    bulletRight.transform.position = transform.position + Vector3.right * 0.35f;

                    GameObject bulletLeft = objectManager.MakeObj("BulletPlayerA");
                    bulletLeft.transform.position = transform.position + Vector3.left * 0.35f;

                    GameObject bulletM = objectManager.MakeObj("BulletPlayerB");
                    bulletM.transform.position = transform.position;

                    Rigidbody2D rigidRight = bulletRight.GetComponent<Rigidbody2D>();
                    Rigidbody2D rigidLeft = bulletLeft.GetComponent<Rigidbody2D>();
                    Rigidbody2D rigidM = bulletM.GetComponent<Rigidbody2D>();
                    rigidRight.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    rigidLeft.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    rigidM.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    break;

            }

           
            if(curShotDelay >= maxShotDelay)
            {
                curShotDelay = 0;
            }
            else
            {
                return;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
           switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "E_Bullet")
        {
            //중복 피격 방지
            if (isHit)
                return;

            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);

            if(life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }
            gameObject.SetActive(false);
        }

        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch(item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if(playerPower == maxPower)
                    {
                        score += 500;
                    }
                    else
                    {
                        playerPower++;
                    } 
                    break;
                case "Boom":
                    if (boom == maxBoom)
                    {
                        score += 500;
                    }
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;
            }
            collision.gameObject.SetActive(false);  //먹었으면 아이템 삭제
        }
    }

   

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }

    void Boom()
    {
        if (!Input.GetKeyDown(KeyCode.Q))
            return;

        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);
        //Effect Visible
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4.0f);

        //Remove Enemies
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");
        for (int index = 0; index < enemiesL.Length; index++)
        {
            if(enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        //Remove EnemyBullets
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");

        for (int index = 0; index < bulletsA.Length; index++)
        {
            if (bulletsA[index].activeSelf)
            {
                bulletsA[index].SetActive(false);
            }        
        }
        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
            {
                bulletsB[index].SetActive(false);
            }
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }
}
