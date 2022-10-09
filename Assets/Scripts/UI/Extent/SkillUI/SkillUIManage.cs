using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 角色的技能UI显示的管理类，用来显示当前的技能
    /// </summary>
    public class SkillUIManage : MonoBehaviour
    {
        /// <summary>/// 每一个技能图标显示用的预制件/// </summary>
        GameObject node;
        Control.PlayerSkillControl skillControl;
        TextureDictionaries textDiction;

        int nowIndex;

        void Start()
        {
            node = Resources.Load<GameObject>("UI/Node");
            textDiction = Resources.Load<TextureDictionaries>("UI/TextureDictionaries");
            textDiction.LoadTextureDictionarie();
            skillControl = Control.PlayerControl.Instance.GetComponent<Control.PlayerSkillControl>();
            nowIndex = 0;
        }

        private void FixedUpdate()
        {
            int skillCount = skillControl.SkillCount;
            if (skillCount <= 0) return;
            int nowSkillCount = transform.childCount;
            if (skillCount == 0)
            {
                if(nowSkillCount <= 0) return;
                else        //关闭正在显示中的物体
                {
                    for(int i=0; i<transform.childCount; i++)
                        transform.GetChild(i).gameObject.SetActive(false);
                    nowSkillCount = 0;
                    return;
                }
            }
            if(nowSkillCount != skillCount)     //数量发生改变
            {
                int i;
                if (nowSkillCount < skillCount) //数量不够就补充
                {
                    for(i = nowSkillCount; i<skillCount; i++)
                    {
                        GameObject newNode = GameObject.Instantiate(node);
                        newNode.transform.parent = transform;
                        nowSkillCount++;
                    }
                }
                //切换图片以及贴图
                i = 0;
                for (; i< nowSkillCount && i < skillCount; i++)
                {
                    Image image = transform.GetChild(i).GetComponentInChildren<Image>();
                    image.gameObject.SetActive(true);
                    Color color = image.color; color.a = 0.3f;
                    image.color = color;
                    image.sprite = textDiction.GetTexture(skillControl.SkillManage.Skills[i].skillName);
                }
                nowIndex = 0;  //默认初始化0
                Image tempIma = transform.GetChild(0).GetComponentInChildren<Image>();
                Color tempCol = tempIma.color; tempCol.a = 1f;
                tempIma.color = tempCol;
                for (; i<nowSkillCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }

            if(nowIndex != skillControl.nowSkill && skillControl.nowSkill >= 0)
            {
                Image image = transform.GetChild(nowIndex).GetComponentInChildren<Image>();
                Color color = image.color; color.a = 0.3f;
                image.color = color;
                Debug.Log(skillControl.nowSkill);
                image = transform.GetChild(skillControl.nowSkill).GetComponentInChildren<Image>();
                color = image.color; color.a = 1;
                image.color = color;
                nowIndex = skillControl.nowSkill;
            }
        }
    }
}