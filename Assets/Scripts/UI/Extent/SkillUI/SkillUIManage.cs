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
        Common.PoolingList<Image> images;

        ///// <summary>  
        ///// 是否在交互中，用来在交互中时判断是否需要关闭技能提示   
        ///// </summary>
        //bool isNoneControl;

        void Start()
        {
            node = Resources.Load<GameObject>("UI/Node");
            skillControl = Control.PlayerControl.Instance.GetComponent<Control.PlayerSkillControl>();
            images = new Common.PoolingList<Image>();
        }

        private void FixedUpdate()
        {
            Control.PlayerControl player = Control.PlayerControl.Instance;
            if (player == null)
                return;
            if (!player.UseControl)
            {
                if(images.Count > 0)
                {
                    while(images.Count > 0)
                    {
                        RemoveOne();
                    }
                }
                return;
            }

            int skillCount = skillControl.SkillCount;
            if(skillCount > images.Count)       //添加
            {
                while(images.Count < skillCount)
                {
                    GetOne();
                }
            }
            else            //移除
            {
                while(images.Count > skillCount)
                {
                    RemoveOne();
                }
            }

            int nowChose = skillControl.nowSkill;
            //在这里才检查是否为空是保证技能都被移除时能够把显示的都关闭了
            if (skillCount == 0 || nowChose < 0)
                return;

            string str = "";

            if (images.GetValue(nowChose).color.a < 1)      //但前选中切换了
            {
                Color color; Image image;
                for (int i=0; i<images.Count; i++)
                {
                    image = images.GetValue(i);
                    image.sprite = TextureDictionaries.Instance.GetTexture(skillControl.SkillManage.Skills[i].skillName);
                    str += skillControl.SkillManage.Skills[i].skillName + " ";
                    color = image.color; color.a = 0.3f;
                    image.color = color;
                }
                image = images.GetValue(nowChose);
                color = image.color; color.a = 1f;
                image.color = color;
            }
        }


        //错误的写法，之后需要改
        void RemoveOne()
        {
            Image image = images.GetValue(0);
            image.gameObject.SetActive(false);
            Color color = image.color; color.a = 0.3f;
            image.color = color;
            images.Remove(0);
        }

        void GetOne()
        {
            GameObject newNode = GameObject.Instantiate(node);
            newNode.transform.parent = transform; newNode.SetActive(true);
            Image image = newNode.GetComponent<Image>();
            Color color = image.color; color.a = 0.3f;
            image.color = color;
            images.Add(image);
        }
    }
}