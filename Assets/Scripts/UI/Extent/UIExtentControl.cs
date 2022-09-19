using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class UIExtentControl : MonoBehaviour
{
    private static UIExtentControl instance;
    public static UIExtentControl Instance
    {
        get
        {
            return instance;
        }
    }

    UIControl control;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        showStack = new Stack<GameObject>();
        control = GetComponent<UIControl>();
    }
    private void OnDestroy()
    {
        instance = null;
    }

    Stack<GameObject> showStack;
    [SerializeField]
    string beginMenuName = "Panel_Button";
    [SerializeField]
    string backGroundName = "Panel_Image";
    //[SerializeField]
    //string instructName = "Panel_Text";

    /// <summary>
    /// 用于显示或者关闭当前的UI，根据栈来判断
    /// </summary>
    public void ShowOrClose()
    {
        if(showStack.Count == 0)
        {
            GameObject game = UICommon.Instance.GetGameObject(beginMenuName, control);
            GameObject background = UICommon.Instance.GetGameObject(backGroundName, control);
            game.SetActive(true);
            background.SetActive(true);
            showStack.Push(game);
        }
        else
        {
            GameObject game = showStack.Pop();
            game.SetActive(false);
            if(showStack.Count == 0)
                UICommon.Instance.GetGameObject(backGroundName, control).SetActive(false);
            else
            {
                showStack.Peek().SetActive(true);
            }
        }
    }

    /// <summary>
    /// 添加一个显示的物体，放到栈中方便统一管理
    /// </summary>
    public void AddShowObject(GameObject game)
    {
        game.SetActive(true);
        showStack.Peek()?.SetActive(false);
        showStack.Push(game);
    }

}
