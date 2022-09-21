using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationControl : MonoBehaviour
{
    public int LetterCount = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Info.CharacterInfo>().getScore() >= LetterCount)//根据收集数量判断（全收集）
        {
            Debug.Log("Win!");
        }
        else
        {
            Debug.Log("Not Enough Letters");
        }
    }
}
