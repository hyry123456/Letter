using UnityEngine;


/// <summary> /// 钩锁节点类，用来标识该点有一个钩锁  /// </summary>
public class HookRopeNode : MonoBehaviour
{
    void Start()
    {
        HookRopeManage.Instance.AddNode(transform);
    }

    private void OnDestroy()
    {
        HookRopeManage.Instance.RemoveNode(transform);
    }

    private void OnDisable()
    {
        HookRopeManage.Instance.RemoveNode(transform);
    }
}
