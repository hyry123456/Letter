
using UnityEngine;

namespace Interaction
{
    public class InteractionControl : MonoBehaviour
    {
        private static InteractionControl instance;

        public static InteractionControl Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("intreraction");
                    instance = go.AddComponent<InteractionControl>();
                    go.hideFlags = HideFlags.HideAndDontSave;
                }
                return instance;
            }
        }

        private Control.PlayerControl playerControl;
        /// <summary>        /// ����������ߵ���ʼ��        /// </summary>
        //public Transform beginPos;
        /// <summary>        /// ��ǰ���Դ����Ľ�����Ϣ        /// </summary>
        public InteractionBase nowInteractionInfo;
        /// <summary>        /// �Ƿ����ڽ�����        /// </summary>
        public bool isInteracting;
        /// <summary>        /// ���߼��ľ���        /// </summary>
        public float interacteCheckDistance = 3f;

        private InteractionControl() { }

        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            instance = this;
        }
        
        /// <summary>        /// �ҵ������ϣ�������ʱʱ����        /// </summary>
        private void Start()
        {
            playerControl = gameObject.GetComponent<Control.PlayerControl>();
            isInteracting = false;
        }

        /// <summary>
        /// ����Ƿ��п��Խ����Ķ���
        /// </summary>
        protected void FixedUpdate()
        {
            //�����о��˳�
            if (isInteracting) 
            {
                return;
            }
            RaycastHit hit;
            if (Physics.Raycast(playerControl.transform.position, playerControl.GetLookatDir() * 3, out hit, interacteCheckDistance))
            {
                InteractionBase hitInfo = hit.transform.GetComponent<InteractionBase>();
                if (hitInfo != null)
                {
                    nowInteractionInfo = hitInfo;
                    return;
                }
                nowInteractionInfo = null;
            }
        }

        /// <summary>        /// ���н����¼�        /// </summary>
        /// <param name="interactionInfo">�����Ľ����¼�</param>
        public void RunInteraction(InteractionBase interactionInfo)
        {
            isInteracting = true;   //�������ڽ���
            if (interactionInfo == null) { 
                Debug.Log("�����������");
            }
            //���н�����Ϊ
            interactionInfo.InteractionBehavior();
        }

        /// <summary>
        /// ������ϵͳ��UI��ʾ��������ã���ʾ������UI�����ˣ�׼���������Ĳ�����
        /// </summary>
        public void ReRunInteraction()
        {
            RunInteraction(nowInteractionInfo);
        }

        /// <summary>
        /// ���е�ǰ���ڽ����Ľ����¼�
        /// </summary>
        public void RunInteraction()
        {
            if (nowInteractionInfo == null) return;
            RunInteraction(nowInteractionInfo);
        }

        /// <summary>
        /// ��ʾֹͣ��������ϵͳ���¿�ʼ����
        /// </summary>
        public void StopInteraction()
        {
            isInteracting = false;
        }

        /// <summary>        /// ��������        /// </summary>
        public void StartInteraction()
        {
            isInteracting = true;
        }

        public bool GetInteraction()
        {
            return isInteracting;
        }

        /// <summary>
        /// ��ӽ�����Ϣ�����ڵ������¼��������ߵ��ʱ����ʱ��Ҫ��ӽ�������Ҫ�ֶ������
        /// </summary>
        /// <param name="interaction">��ӵĽ�����Ϣ</param>
        public void AddInteractionInfo(InteractionBase interaction)
        {
            nowInteractionInfo = interaction;
        }
    }
}