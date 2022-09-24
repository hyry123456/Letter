using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationControl : MonoBehaviour
{
    public int LetterCount = 0;//Not Used
    [SerializeField]
    string targetSceneName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Info.CharacterInfo>().getScore() > 0)//根据收集数量判断（全收集）//划掉，只要收集至少一个就能通关
        {
            if(targetSceneName != null && targetSceneName.Length != 0)
                Control.SceneChangeControl.Instance.ChangeScene(targetSceneName);
            //Debug.Log("Win!");
        }
        else
        {
            //Debug.Log("Not Enough Letters");
        }
    }
}
