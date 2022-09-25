using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    protected GameObject Player;//玩家
    protected Vector3 playerPosition;//玩家坐标
    public GameObject bullet;//子弹预制体
    protected int Timer = 0;//计时器
    public int shotTime = 3;//射击间隔
    public int bulletSpeed = 10;//子弹速度
    public int viewDistance = 100;//敌人视野距离

    void Start()
    {
        Player = GameObject.Find("Player");//暂时每个敌人都找一遍，敌人数量不多还用不上单独的敌人管理类
    }


    void Update()
    {
        playerPosition = Player.transform.position;//获取玩家坐标
        transform.GetChild(1).LookAt(playerPosition);//时刻看向玩家
        
    }
    private void FixedUpdate()
    {
        Timer++;
        if (Timer % (shotTime*50) == 0)
        {
            shootOnce();//射击一次
        }
    }
    protected void shootOnce()//射击一次
    {
        Vector3 selfPosition = gameObject.transform.position;
        Vector3 distance = playerPosition - selfPosition;//获取敌人与玩家的坐标差
        //Debug.Log(distance.sqrMagnitude);
        if (distance.sqrMagnitude > (viewDistance*viewDistance))
        {
            return;//太远了就不打了
        }
        GameObject instance = Instantiate(bullet, selfPosition, gameObject.transform.rotation);//射一发，实例化一个子弹
        distance = Vector3.Normalize(distance);
        instance.GetComponent<Rigidbody>().AddForce(distance * bulletSpeed * 100);//由坐标向量方向决定发射方向
        Destroy(instance, 5f);//设置定时销毁，优化内存

    }
}
