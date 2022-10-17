using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ChooseRemain : UIUseBase
    {
        [SerializeField]
        GameObject effectObject;

        protected override void Awake()
        {
            base.Awake();
            control.init += ShowSelf;
            widgrt.pointerEnter += PointerEnter;
            widgrt.pointerExit += PointerExit;
            effectObject.SetActive(false);
        }

        void PointerEnter(PointerEventData eventData)
        {
            effectObject.SetActive(true);
        }

        void PointerExit(PointerEventData eventData)
        {
            effectObject.SetActive(false);
        }

    }
}