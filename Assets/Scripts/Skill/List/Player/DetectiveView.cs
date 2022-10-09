using UnityEngine;

namespace Skill
{
    /// <summary>
    /// 侦探视野技能，看到不可视之物
    /// </summary>
    public class DetectiveView : SkillBase
    {
        DefferedRender.PostFXSetting fXSetting;
        public DetectiveView()
        {
            expendSP = 0;
            nowCoolTime = 0;
            coolTime = 0;
            skillName = "侦探视野";
            skillType = SkillType.Auxiliary;
        }
        //一开始应该是增强雾效，开启鹰眼，因此默认赋值为false
        bool isUpFog = false;
        
        float nowRadio, //当前比例值
            changeTime = 2;    //变化需要的时间

        public override void OnSkillRelease(SkillManage mana)
        {
            if(fXSetting == null)
                fXSetting = Resources.Load<DefferedRender.PostFXSetting>("Render/PostFX/PostFX");
            isUpFog = !isUpFog;
            nowRadio = 0;
            if (isUpFog)
            {
                fXSetting.BeginDetective();
                Common.SustainCoroutine.Instance.AddCoroutine(UpFog);
            }
            else
            {
                Common.SustainCoroutine.Instance.AddCoroutine(DecreaseFog);
            }
        }

        //增强雾效，开启鹰眼
        bool UpFog()
        {
            //如果切换为降低雾效，退出该行为
            if (!isUpFog)
                return true;
            nowRadio += Time.deltaTime * (1.0f / changeTime);
            if(nowRadio >= 1.0f)
            {
                nowRadio = 1.0f;
                fXSetting.SetMaxDepthFog(0);
                return true;
            }
            fXSetting.SetMaxDepthFog(Mathf.Lerp(0.1f, 0, nowRadio));
            return false;
        }

        //减少雾效
        bool DecreaseFog()
        {
            //如果切换为上升雾效，退出该行为
            if(isUpFog) return true;
            nowRadio += Time.deltaTime * (1.0f / changeTime);
            if (nowRadio >= 1.0f)
            {
                nowRadio = 1.0f;
                fXSetting.SetMaxDepthFog(0.1f);
                fXSetting.CloseDetective();
                return true;
            }
            fXSetting.SetMaxDepthFog(Mathf.Lerp(0f, 0.1f, nowRadio));
            return false;
        }
    }
}