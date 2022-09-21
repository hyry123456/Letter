using UnityEngine;
using UI;
using UnityEngine.EventSystems;

public class UIGameBegin : UIUseBase
{
    [SerializeField]
    string targetScene;
    protected override void Awake()
    {
        base.Awake();
        control.init += ShowSelf;
        widgrt.pointerClick += ChangeScene;
    }

    private void ChangeScene(PointerEventData eventData)
    {
        Debug.Log("Change");
        Control.SceneChangeControl.Instance.ChangeScene(targetScene);
    }

}
