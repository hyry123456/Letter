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
        if (other.gameObject.GetComponent<Info.CharacterInfo>().getScore() > 0)//�����ռ������жϣ�ȫ�ռ���//������ֻҪ�ռ�����һ������ͨ��
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
