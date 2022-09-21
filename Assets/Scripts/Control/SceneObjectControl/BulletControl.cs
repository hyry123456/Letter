using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //½ÇÉ«µôÑª,todo
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
