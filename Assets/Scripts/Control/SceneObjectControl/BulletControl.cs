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
        if (rb != null)//�ж�Ŀ���Ƿ��и���
        {
            target = rb.gameObject;
            if(target.name == "Player")//ͨ�������ж�
            {
                target.GetComponent<Info.CharacterInfo>().modifyHp(-5);//��������
            }
        }
        Destroy(gameObject);//�����ӵ�
    }
}
