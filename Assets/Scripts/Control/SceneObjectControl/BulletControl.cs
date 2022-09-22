using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        GameObject target;
        if (rb != null)//判断目标是否有刚体
        {
            target = rb.gameObject;
            if(target.name == "Player")//通过名字判断
            {
                target.GetComponent<Info.CharacterInfo>().modifyHp(-5);//后续操作
            }
        }
        Destroy(gameObject);//销毁子弹
    }
}
