using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //��ɫ��Ѫ,todo
        Destroy(this);

    }
    private int Timer = 0;
    private void FixedUpdate()
    {
        Timer++;
        if(Timer > 500)
        {
            Destroy(this);
        }
    }

}
