using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// ��ɫ�ļ���UI��ʾ�Ĺ����࣬������ʾ��ǰ�ļ���
    /// </summary>
    public class SkillUIManage : MonoBehaviour
    {
        /// <summary>/// ÿһ������ͼ����ʾ�õ�Ԥ�Ƽ�/// </summary>
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
                else        //�ر�������ʾ�е�����
                {
                    for(int i=0; i<transform.childCount; i++)
                        transform.GetChild(i).gameObject.SetActive(false);
                    nowSkillCount = 0;
                    return;
                }
            }
            if(nowSkillCount != skillCount)     //���������ı�
            {
                int i;
                if (nowSkillCount < skillCount) //���������Ͳ���
                {
                    for(i = nowSkillCount; i<skillCount; i++)
                    {
                        GameObject newNode = GameObject.Instantiate(node);
                        newNode.transform.parent = transform;
                        nowSkillCount++;
                    }
                }
                //�л�ͼƬ�Լ���ͼ
                i = 0;
                for (; i< nowSkillCount && i < skillCount; i++)
                {
                    Image image = transform.GetChild(i).GetComponentInChildren<Image>();
                    image.gameObject.SetActive(true);
                    Color color = image.color; color.a = 0.3f;
                    image.color = color;
                    image.sprite = textDiction.GetTexture(skillControl.SkillManage.Skills[i].skillName);
                }
                nowIndex = 0;  //Ĭ�ϳ�ʼ��0
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