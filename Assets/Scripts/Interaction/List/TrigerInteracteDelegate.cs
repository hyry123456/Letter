using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction
{
    /// <summary>
    /// ����ʽ�����¼����������һ���������ϣ���Ϊ��������������٣�
    /// ��ֻ�ᱻ���Ǹ�����
    /// </summary>
    public class TrigerInteracteDelegate : InteractionBase
    {
        /// <summary>        /// ����ʱִ�е���Ϊ        /// </summary>
        public Common.INonReturnAndNonParam trigerDelegate;
        public override void InteractionBehavior()
        {
        }

        /// <summary>   
        /// ����������ʱִ�еķ���������ִ�н��������������������̽����ý���
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            //ֻ��������
            if(other.tag == "Player")
            {
                if(trigerDelegate != null)
                {
                    trigerDelegate();
                    trigerDelegate = null;
                }
                //ɾ������
                Destroy(gameObject);
            }
        }

        protected override void OnEnable()
        {
        }
    }
}