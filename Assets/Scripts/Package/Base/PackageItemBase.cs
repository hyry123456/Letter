using UnityEngine;

namespace Package
{
    /// <summary> /// 背包基类，定义背包基本数据 /// </summary>
    public abstract class PackageItemBase
    {
        /// <summary>   /// 物体名称     /// </summary>
        protected string itemName;
        /// <summary>   /// 物体描述   /// </summary>
        protected string itemDescription;
        /// <summary>
        /// 对应的图片名称，我们会在图片库中查找该图片
        /// </summary>
        protected string imageName;

        public string ItemName => itemName;
        public string ItemDescription => itemDescription;

        public string ImageName => imageName;

        //public PackageItemBase()
        //{
        //    itemName = "Name";
        //    itemDescription = "Description";
        //    imageName = "ImageName";
        //}
    }
}