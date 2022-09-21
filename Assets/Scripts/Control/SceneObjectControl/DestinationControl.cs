using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationControl : MonoBehaviour
{
    public int LetterCount = 0;
    [SerializeField]
    string targetSceneName;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Info.CharacterInfo>().getScore() >= LetterCount)//�����ռ������жϣ�ȫ�ռ���
        {
            if(targetSceneName != null && targetSceneName.Length != 0)
                Control.SceneChangeControl.Instance.ChangeScene(targetSceneName);
            Debug.Log("Win!");
        }
        else
        {
            Debug.Log("Not Enough Letters");
        }
    }
}
