using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterControl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<Info.CharacterInfo>().gainScore();//��÷���
        gameObject.SetActive(false);//������ʧ
    }
    private void Update()
    {
        gameObject.transform.parent.gameObject.transform.Rotate(Vector3.up, Time.deltaTime * 50f);//תȦ
    }
}
