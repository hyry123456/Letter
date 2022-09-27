using UnityEngine;
using UI;
using UnityEngine.EventSystems;

public class UIGameExit : UIUseBase
{
    protected override void Awake()
    {
        base.Awake();
        control.init += ShowSelf;
        widgrt.pointerClick += ExitGame;
    }

    private void ExitGame(PointerEventData eventData)
    {
        Control.SceneChangeControl.Instance.GameExit();
    }

}
