using UnityEngine;

public class LetterControl : MonoBehaviour
{
    /// <summary> /// 旋转速度/// </summary>
    public float RotateSpeed = 50;
    /// <summary> /// 父节点/// </summary>
    protected Transform Parent;
    /// <summary> /// 灯光节点/// </summary>
    protected Transform Light;
    /// <summary> /// 计时器/// </summary>
    protected int Timer = 0;
    /// <summary> /// 记录灯光开关的变量/// </summary>
    protected bool IsLightActive = true;
    private void Start()
    {
        Parent = gameObject.transform.parent;
        Light = Parent.GetChild(1);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")//按名字查找
        {
            other.gameObject.GetComponent<Info.CharacterInfo>()?.gainScore();//获得分数
            Parent.gameObject.SetActive(false);
        }
        else return;
    }
    private void Update()
    {
        Parent.Rotate(Vector3.up, Time.deltaTime * RotateSpeed);//转圈
    }
    
    private void FixedUpdate()
    {
        LightSwitch();
    }
    /// <summary>///判断灯光开关/// </summary>
    private void LightSwitch()
    {
        Timer++;
        if (Timer % 100 == 0)
        {
            if (IsLightActive)
            {
                IsLightActive = false;
                Light.gameObject.SetActive(false);
            }
            else
            {
                IsLightActive = true;
                Light.gameObject.SetActive(true);
            }
        }
    }
}
