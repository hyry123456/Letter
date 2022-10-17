using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomUI : MonoBehaviour
{
    public GameObject left;
    public GameObject right;

    public float moveSize = 1;
    public float moveSpeed = 2;
    private float nowRadio = 0;
    private Vector3 leftBegin, leftEnd, rightBegin, rightEnd;
    private bool isUp;

    private void OnEnable()
    {
        nowRadio = 0;
        isUp = true;
        leftBegin = left.transform.position;
        leftEnd = left.transform.position + Vector3.left * moveSize;
        rightBegin = right.transform.position;
        rightEnd = right.transform.position + Vector3.right * moveSize;
    }

    private void Update()
    {
        nowRadio += isUp? Time.deltaTime * moveSpeed : -Time.deltaTime * moveSpeed;
        if(nowRadio > 1)
        {
            nowRadio = 1;
            isUp = false;
        }
        else if(nowRadio < 0)
        {
            nowRadio = 0;
            isUp = true;
        }
        left.transform.position = Vector3.Lerp(leftBegin, leftEnd, nowRadio);
        right.transform.position = Vector3.Lerp(rightBegin, rightEnd, nowRadio);
    }

    private void OnDisable()
    {
        left.transform.position = leftBegin;
        right.transform.position = rightBegin;
    }
}
