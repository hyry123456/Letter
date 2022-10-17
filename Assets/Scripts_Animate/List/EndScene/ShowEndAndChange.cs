using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

namespace ScriptAnimate
{
    [CreateAssetMenu(menuName = "ScriptAnimate/EndScene/ShowEndAndChange")]
    public class ShowEndAndChange : ScriptAnimateBase
    {
        [SerializeField]
        float perTime = 5;
        float nowRadio = 0;
        int nowIndex = 0;
        [SerializeField]
        string targetScene;
        List<string> texts = new List<string>()
        {
            "开发者：\n美术:染昊\n程序:染昊\nTA:染昊\n策划:染昊\n场景搭建:染昊\n",
            "特别鸣谢:\n策划:罗烜\n建模:橙子男",
            "感谢游玩!"
        };

        Text text;

        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            text = SceneObjectMap.Instance.FindControlObject("Text_EndText").GetComponent<Text>();
            SceneObjectMap.Instance.FindControlObject("Image_BackGround").SetActive(true);
            text.gameObject.SetActive(true);
            nowIndex = 0;
            text.text = texts[nowIndex];
            SustainCoroutine.Instance.AddCoroutine(ChangeUIText);
        }
        bool ChangeUIText()
        {
            nowRadio += Time.deltaTime * (1.0f / perTime);
            if(nowRadio > 1.0f)
            {
                nowRadio = 0;
                nowIndex++;
                if(nowIndex == texts.Count)
                {
                    Control.SceneChangeControl.Instance.ChangeScene(targetScene);
                    return true;
                }
                else
                {
                    text.text = texts[nowIndex];
                }
            }
            return false;
        }

        public override bool EndAnimate(ScriptAnimateControl animateControl)
        {
            return true;
        }
    }
}