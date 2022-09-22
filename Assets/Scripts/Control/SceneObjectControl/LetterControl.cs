using UnityEngine;

public class LetterControl : MonoBehaviour
{
    protected Transform Parent;
    protected Transform Light;
    protected int Timer = 0;
    protected bool IsLightActive = true;
    private void Start()
    {
        Parent = gameObject.transform.parent;
        Light = Parent.GetChild(1);
    }
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<Info.CharacterInfo>().gainScore();//获得分数
        Parent.gameObject.SetActive(false);
    }
    private void Update()
    {
        Parent.Rotate(Vector3.up, Time.deltaTime * 50f);//转圈
    }
    
    private void FixedUpdate()
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
