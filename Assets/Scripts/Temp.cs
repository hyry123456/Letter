using UnityEngine;

public class Temp : MonoBehaviour
{
    [GradientUsage(true)]
    public Gradient gradient;
    void Start()
    {
        //UI.BigDialog.Instance.ShowBigdialog("你好\n我是马云\n我很有钱", null);
        //UI.SmallDialog.Instance.ShowSmallDialog("你好\n我是马云\n我很有钱", null);
        GradientColorKey[] colorKeys = gradient.colorKeys;
        for(int i=0; i<colorKeys.Length; i++)
        {
            Debug.Log(i.ToString() + " " + colorKeys[i].color);
            Debug.Log(i.ToString() + " " + colorKeys[i].time);
        }
    }


}
