using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    protected GameObject Player;
    protected Vector3 playerPosition;
    public GameObject bullet;
    protected int Timer = 0;
    public int shotTime = 3;

    void Start()
    {
        Player = GameObject.Find("Player");//暂时每个敌人都找一遍，敌人数量不多还用不上单独的敌人管理类
    }


    void Update()
    {
        playerPosition = Player.transform.position;
        gameObject.transform.Find("Head").transform.LookAt(playerPosition);
        
    }
    private void FixedUpdate()
    {
        Timer++;
        if (Timer % (shotTime*50) == 0)
        {
            shootOnce();
        }
    }
    protected void shootOnce()
    {
        Debug.Log("shoot");
        GameObject instance = Instantiate(bullet, gameObject.transform.position, gameObject.transform.rotation);
        
        

    }
    //计划实现的内容
    /*
     * 1.子弹缓动
     * 2.子弹销毁
     * 3.敌人移动(?)
     * 4.子弹碰撞扣血
     */

}
