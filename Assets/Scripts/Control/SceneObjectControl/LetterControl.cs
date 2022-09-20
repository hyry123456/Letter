using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterControl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<Info.CharacterInfo>().gainScore();//获得分数
        gameObject.SetActive(false);//让信消失
    }
    private void Update()
    {
        gameObject.transform.parent.gameObject.transform.Rotate(Vector3.up, Time.deltaTime * 50f);//转圈
    }
}
