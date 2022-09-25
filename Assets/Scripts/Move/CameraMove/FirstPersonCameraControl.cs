using UnityEngine;

namespace Motor
{
    public class FirstPersonCameraControl : MonoBehaviour
    {
        /// <summary>        /// 玩家节点     /// </summary>
        public GameObject Player;
        /// <summary>        /// 用户的鼠标灵敏度        /// </summary>
        public float Sensitivity = 200;
        /// <summary>        /// 用户的鼠标输入        /// </summary>
        Vector2 playerInput;
        /// <summary>        /// 相机的高度        /// </summary>
        public float CameraHeight = 0;
        /// <summary>    /// 确定摄像机的视锥体的大小    /// </summary>
        Vector3 CameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                Camera regularCamera = GetComponent<Camera>();
                //确定投影矩形的Y轴一半大小，不直接使用近平面的一半是因为fieldOfView会进行一定的缩放，我们需要将缩放变回去
                halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
                //根据比例计算出X轴大小
                halfExtends.x = halfExtends.y * regularCamera.aspect;
                //Z根据投影长度来设置，没有影响
                halfExtends.z = 0f;
                return halfExtends;
            }
        }

        //相机旋转的分量，为了方便使用把他们放在了全局
        private float xRotation;
        private float yRotation;
        //鼠标移动的分量，为了方便使用把他们放在了全局
        private float xMouse;
        private float yMouse;

        private void Start()
        {
            
        }

        private void Update()
        {
            xMouse = Input.GetAxis("Mouse X");
            yMouse = -Input.GetAxis("Mouse Y");
            SetCameraInput(yMouse, xMouse);
            setCameraPosition();
            ManualRotation();
        }
        /// <summary>
        /// 调整摄像机位置
        /// </summary>
        private void setCameraPosition()
        {
            Vector3 CameraPlace = Player.transform.position;
            CameraPlace.y += CameraHeight;//稍微调高摄像机的高度
            transform.position = CameraPlace;//设置摄像机位置
        }

        /// <summary>   /// 管理旋转角度  /// </summary>
        /// <returns>是否需要进行旋转</returns>
        bool ManualRotation()
        {
            //鼠标输入值
            float e = 0.001f;
            //判断是否有输入,注意是鼠标输入
            if (playerInput.x < -e || playerInput.x > e || playerInput.y < -e || playerInput.y > e)
            {
                xRotation = xRotation - playerInput.x * Sensitivity * Time.deltaTime;
                yRotation = yRotation + playerInput.y * Sensitivity * Time.deltaTime;
                ConstrainAngle();
                transform.localRotation = Quaternion.Euler(-xRotation, yRotation, 0);
                return true;

            }
            return false;
        }
        /// <summary>
        /// 根据移动的差距值判断旋转角度，注意传入值要标准化，
        /// 设置为静态因为这个函数不需要用到对象数据，因此只用开辟一个函数体就够了
        /// </summary>
        static float GetAngle(Vector2 direction)
        {
            //通过反余弦函数计算出旋转到这个移动方向所需要的y值角度
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            //判断是哪边，也就是顺时针还是逆时针
            return direction.x < 0f ? 360f - angle : angle;
        }

        /// <summary>   /// 限制角度大小  /// </summary>
        private void ConstrainAngle()
        {
            if (yRotation > 0f)
            {
                yRotation -= 360f;
            }
            else if(yRotation < -360f)
            {
                yRotation += 360f;
            }
            xRotation = Mathf.Clamp(xRotation, -89f, 89f);
        }
        /// <summary>   /// 设置相机旋转角度的输入  /// </summary>
        public void SetCameraInput(float mouseY, float mouseX)
        {
            playerInput = new Vector2(mouseY, mouseX);
        }
    }
}


