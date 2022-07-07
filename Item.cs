using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string type;
    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        rigid.velocity = Vector2.down * 3.0f;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {   
        //화면 밖으로 나가면 아이템 비활성화
        if(collision.gameObject.tag == "BorderBullet")
        {
            gameObject.SetActive(false);
        }
    }

}
