using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Package
{
    /// <summary>
    /// 一个临时的背包类，用来显示获取的物体
    /// </summary>
    public class PackageSimple
    {
        private static PackageSimple instance;
        public static PackageSimple Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new PackageSimple();
                }
                return instance;
            }
        }

        private PackageSimple() {
            instance = this;
            allItems = new List<PackageItemBase>();
        }

        private List<PackageItemBase> allItems;

        public List<PackageItemBase> Items => allItems;

        /// <summary>   /// 添加一个物体到背包中     /// </summary>
        public void AddItem(PackageItemBase item)
        {
            for(int i=0; i < allItems.Count; i++)
            {
                if (allItems[i].ItemName == item.ItemName)
                    return;
            }
            allItems.Add(item);
        }

        Assembly assembly = Assembly.GetExecutingAssembly();

        /// <summary>     /// 通过名称，反射一个物体到背包中     /// </summary>
        public void AddItem(string itemName)
        {
            PackageItemBase item = (PackageItemBase)assembly.CreateInstance("Package." + itemName);
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i].ItemName == item.ItemName)
                    return;
            }
            allItems.Add(item);
        }


    }
}