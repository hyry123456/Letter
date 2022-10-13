using Package;
using Common;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 显示单个Item用的池化类，用来显示图片以及点击事件
    /// </summary>
    public class UIPackageItem : ObjectPoolBase, IPointerClickHandler
    {
        /// <summary>  /// 该物体的图片组件    /// </summary>
        Image image;
        //名称以及描述
        string itemName, itemDescript;
        UIPackage uiPackage;

        public string ItemName => itemName;
        public string ItemDescript => itemDescript;

        public void OnPointerClick(PointerEventData eventData)
        {
            uiPackage.ChangeItemInfo(this);
        }

        public void SetImage(PackageItemBase packageItem, UIPackage uiPackage)
        {
            this.uiPackage = uiPackage;
            image.sprite = TextureDictionaries.Instance.GetTexture(packageItem.ImageName);
            itemName = packageItem.ItemName;
            itemDescript = packageItem.ItemDescription;
        }

        //获取图片
        protected override void OnEnable()
        {
            if(image == null)
                image = GetComponent<Image>();
        }

    }
}