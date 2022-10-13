using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Package;
using Common;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// UI的背包显示基类，用来显示拥有的物体
    /// </summary>
    public class UIPackage : MonoBehaviour
    {
        Text itemName,          //当前显示的物体名称
            itemDescription;    //当前显示的物体描述

        private PoolingList<UIPackageItem> items;

        private void OnEnable()
        {
            //创建所有的items
        }
    }
}