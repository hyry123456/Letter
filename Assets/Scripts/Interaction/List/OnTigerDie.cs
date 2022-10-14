using UnityEngine;

/// <summary>/// 接触后就会死亡的方法/// </summary>
public class OnTigerDie : MonoBehaviour
{
    /// <summary>   
    /// 当触发发生时执行的方法，用来执行交互，当触发结束后立刻结束该交互
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        //只触发主角
        if (other.tag == "Player")
        {
            Control.SceneChangeControl.Instance.ReloadActiveScene();
        }
    }

}
