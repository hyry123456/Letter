
using UnityEngine.EventSystems;

namespace UnityEngine.EventSystems
{
    /// <summary>
    /// 我的全局点击事件，因为我发现没点到目标UI上的话调用不到点击事件，因此自己实现一个全局调用
    /// </summary>
    public interface ISceneClickHandler
    {
        public void ScenePointClick(PointerEventData eventData);
    }
}
