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
        Player = GameObject.Find("Player");//��ʱÿ�����˶���һ�飬�����������໹�ò��ϵ����ĵ��˹�����
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
    //�ƻ�ʵ�ֵ�����
    /*
     * 1.�ӵ�����
     * 2.�ӵ�����
     * 3.�����ƶ�(?)
     * 4.�ӵ���ײ��Ѫ
     */

}
