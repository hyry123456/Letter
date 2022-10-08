using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    void Start()
    {
        //UI.BigDialog.Instance.ShowBigdialog("你好\n我是马云\n我很有钱", null);
        UI.SmallDialog.Instance.ShowSmallDialog("你好\n我是马云\n我很有钱", null);
    }


}
