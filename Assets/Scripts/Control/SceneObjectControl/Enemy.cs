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
    public int bulletSpeed = 10;

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
    protected void shootOnce()//���һ�Σ�ע���Ժ󲹳�
    {
        Debug.Log("shoot");
        Vector3 selfPosition = gameObject.transform.position;
        GameObject instance = Instantiate(bullet, selfPosition, gameObject.transform.rotation);
        Vector3 distance = playerPosition - selfPosition;
        distance = Vector3.Normalize(distance);
        instance.GetComponent<Rigidbody>().AddForce(distance * bulletSpeed * 100);
        Destroy(instance, 5f);

    }
}
