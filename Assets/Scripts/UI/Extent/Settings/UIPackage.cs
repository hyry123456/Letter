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
        [SerializeField]
        Text itemName,          //当前显示的物体名称
            itemDescription;    //当前显示的物体描述

        private PoolingList<UIPackageItem> uiItems = new PoolingList<UIPackageItem>();

        public GameObject origin;
        /// <summary>   /// 创建的所有物体存储的位置   /// </summary>
        public RectTransform context;
        public int columnHeight = 35,   //每一行的高度
            perColumnSize = 5;          //每一行的数量

        private void OnEnable()
        {
            //创建所有的items
            SustainCoroutine.Instance.AddCoroutine(LaterLoadItem);
        }

        private void OnDisable()
        {
            while(uiItems.Count > 0)
            {
                uiItems.GetValue(0).CloseObject();
                uiItems.Remove(0);
            }
        }

        /// <summary>/// 延迟加载全部的背包内容 /// </summary>
        bool LaterLoadItem()
        {
            if (!gameObject.activeSelf) return true;
            List<PackageItemBase> items = PackageSimple.Instance.Items;
            if (items == null || items.Count == 0) return true;

            int column = items.Count / perColumnSize + 1;
            //缩放大小
            context.sizeDelta = new Vector2(0, columnHeight * column);

            for (int i=0; i<items.Count; i++)
            {
                UIPackageItem item = (UIPackageItem)SceneObjectPool.Instance.GetObject(
                    "PackageItem", origin, Vector3.zero, Quaternion.identity);
                item.SetImage(items[i], this);
                item.transform.parent = context;
                item.transform.localScale = Vector3.one;
                uiItems.Add(item);
            }
            ChangeItemInfo(uiItems.GetValue(0));
            return true;
        }

        public void ChangeItemInfo(UIPackageItem item)
        {
            itemName.text = item.ItemName;
            itemDescription.text = item.ItemDescript;
        }
    }
}