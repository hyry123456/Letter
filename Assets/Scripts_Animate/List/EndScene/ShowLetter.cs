using UnityEngine;

namespace ScriptAnimate
{

    [CreateAssetMenu(menuName = "ScriptAnimate/EndScene/ShowLetter")]
    public class ShowLetter : ScriptAnimateBase
    {
        GameObject letter, background;
        bool canExit;
        public override void BeginAnimate(ScriptAnimateControl animateControl)
        {
            canExit = false;
            Common.SustainCoroutine.Instance.AddCoroutine(ShowSceneLetter);
            Common.SustainCoroutine.Instance.AddCoroutine(CheckExit);
        }
        bool ShowSceneLetter()
        {
            letter = Common.SceneObjectMap.Instance.FindControlObject("Papre");
            background = Common.SceneObjectMap.Instance.FindControlObject("PapreBackground");
            letter.SetActive(true);
            background.SetActive(true);
            return true;
        }

        bool CheckExit()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                canExit = true;
                return true;
            }
            return false;
        }

        public override bool EndAnimate(ScriptAnimateControl animateControl)
        {
            if (canExit)
            {
                letter.SetActive(false);
                background.SetActive(false);
                return true;
            }
            return false;
        }
    }
}