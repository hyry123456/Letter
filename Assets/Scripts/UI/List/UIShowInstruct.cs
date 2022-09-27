using UnityEngine;
using UI;
using UnityEngine.EventSystems;

public class UIShowInstruct : UIUseBase
{
    protected override void Awake()
    {
        base.Awake();
        control.init += ShowSelf;
        widgrt.pointerClick += ShowInstruct;
    }
    [SerializeField]
    string showName = "Panel_Text";

    private void ShowInstruct(PointerEventData eventData)
    {
        UIExtentControl.Instance.AddShowObject(control.UIObjectDictionary[showName]);
    }

}
