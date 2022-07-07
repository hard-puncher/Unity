using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public Sprite[] sprites;
    public int enemyScore;

    public float maxShotDelay;
    public float curShotDelay;

    //총알
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    //아이템
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;

    public GameObject player;
    public ObjectManager objectManager;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    //OnEnable() : 컴포넌트가 활성화 될 때 호출되는 생명주기함수
    void OnEnable()
    {
        switch(enemyName)
        {
            case "L":
                health = 10;
                break;
            case "M":
                health = 5;
                break;
            case "S":
                health = 3;
                break;
        }
    }

    void Update()
    {
        EnemyFire();
        Reload();
    }

    //피격 시
    public void OnHit(int dmg)
    {
        if (health <= 0)
            return;

        health -= dmg;
        spriteRenderer.sprite = sprites[1]; //피격 시 흐린 색깔 스프라이트로 변경
        Invoke("ReturnSprite", 0.1f);   //0.1초 후 원래 스프라이트로 변경

        if (health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            //#Random Ratio Item Drop
            int ran = Random.Range(0, 10);
            if(ran < 5)
            {
                Debug.Log("Not Item");
            }
            else if(ran < 8)    //Coin
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if(ran < 9)    //Power
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
            }
            else if(ran < 10)   //Boom
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            }
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;   //Quaternion.identity : 기본 회전값 = 0
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet")
        {
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(false);    //보더불렛에 닿으면 에너미 비활성화
        }
        else if (collision.gameObject.tag == "Bullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);
            collision.gameObject.SetActive(false);  //총알에 피격시 총알 삭제
        }
    }

    void EnemyFire()
    {
        if (curShotDelay < maxShotDelay)
            return;

        if (enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;

            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;
                
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
            Vector3 dirVecR = player.transform.position - transform.position;
            Vector3 dirVecL = player.transform.position - transform.position;
            rigidR.AddForce(dirVecR.normalized * 3, ForceMode2D.Impulse);  //normalized: 벡터가 단위 값(1)로 변환된 변수 (단위벡터화)
            rigidL.AddForce(dirVecL.normalized * 3, ForceMode2D.Impulse);
        }

        if (curShotDelay >= maxShotDelay)
        {
            curShotDelay = 0;
        }
        else
        {
            return;
        }


    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    } 
   
}
