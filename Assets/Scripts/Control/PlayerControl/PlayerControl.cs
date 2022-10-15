
using UnityEngine;
using Common.ResetInput;

namespace Control
{
    public class PlayerControl : MonoBehaviour
    {
        private static PlayerControl instance;
        private Motor.RigibodyMotor motor;
        private Interaction.InteractionControl interactionControl;
        private PlayerSkillControl skillControl;

        public float dieY = -100;
        /// <summary>     /// 判断是否启动控制      /// </summary>
        bool useControl = true;
        public bool UseControl => useControl;

        public Motor.RigibodyMotor Motor => motor;

        /// <summary>        /// 启动角色控制        /// </summary>
        public void BeginControl()
        {
            useControl = true;
        }
        /// <summary>      /// 停止角色控制     /// </summary>
        public void StopControl()
        {
            Debug.Log("Stop");
            useControl = false;
            motor.Move(0, 0);
        }
        

        public static PlayerControl Instance {
            get
            {
                if(instance == null)
                    instance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
                return instance; 
            }
        }

        /// <summary>/// 传入技能类名，添加技能/// </summary>
        /// <param name="skillName">类名</param>
        public void AddSkill(string skillName)
        {
            skillControl.AddSkill(skillName);
        }

        public string verticalName = "Vertical";
        public string horizontalName = "Horizontal";
        public string jumpName = "Jump";
        public string setName = "Setting";
        public string begName = "Bag";
        public string interacteName = "Interaction";
        public string rollName = "Roll";

        private void Awake()
        {
            instance = this;
        }
        private void OnDestroy()
        {
            instance = null;
        }
        private void OnDisable()
        {
            instance = null;
        }

        protected void Start()
        {
            motor = GetComponent<Motor.RigibodyMotor>();
            interactionControl = GetComponent<Interaction.InteractionControl>();
            skillControl = GetComponent<PlayerSkillControl>();
        }

        /// <summary>
        /// 时时刷新的控制属性的存放位置
        /// </summary>
        private void Update()
        {
            if (!useControl) return;
            if (Input.GetMouseButtonDown(0))
                skillControl?.ReleaseChooseSkill();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UI.UISettings.Instance.ShowOrCloseUISettings();
            }
        }

        /// <summary>
        /// 物理帧刷新的属性计算位置，一些没有必要逐帧计算的可以在这里进行计算
        /// </summary>
        private void FixedUpdate()
        {
            if (!useControl) return;

            //先获取这些，之后补充其他
            float vertical = MyInput.Instance.GetAsis(verticalName);
            float horizontal = MyInput.Instance.GetAsis(horizontalName);
            bool jump = MyInput.Instance.GetButtonDown(jumpName);
            //bool skill = MyInput.Instance.GetButtonDown("Skill");
            bool esc = MyInput.Instance.GetButtonDown("ESC");
            bool interacte = MyInput.Instance.GetButtonDown(interacteName);

            motor.Move(horizontal, vertical);
            if (jump)
                motor.DesireJump();

            //if(skill)
            //    motor.TransferToPosition(HookRopeManage.Instance.Target, hookSpeed);
            
            if (esc)
                UIExtentControl.Instance?.ShowOrClose();

            if (interacte && interactionControl != null)
                interactionControl.RunInteraction();

            if (transform.position.y < dieY)
                SceneChangeControl.Instance.ReloadActiveScene();
        }

        /// <summary>
        /// 获得主角看向的位置，也就是摄像机前方
        /// </summary>
        public Vector3 GetLookatDir()
        {
            if (Camera.main == null) return Vector3.zero;
            return Camera.main.transform.forward;
        }

        /// <summary>
        /// 获得摄像机的世界坐标
        /// </summary>
        public Vector3 GetCameraPos()
        {
            Camera camera = Camera.main;
            if (camera != null)
                return camera.transform.position;
            else return Vector3.zero;
        }

        ///// <summary>
        ///// 获得移动基类
        ///// </summary>
        //public Motor.Motor GetMotor()
        //{
        //    return motor;
        //}
    }
}