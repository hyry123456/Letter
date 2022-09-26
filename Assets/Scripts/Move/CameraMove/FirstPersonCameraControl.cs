using UnityEngine;
using UnityEngine.UI;

namespace Motor
{
    public class FirstPersonCameraControl : MonoBehaviour
    {
        /// <summary>        /// 玩家节点     /// </summary>
        public GameObject Player;
        /// <summary>        /// 灵敏度        /// </summary>
        public float Sensitivity = 200;
        /// <summary>        /// 玩家输入        /// </summary>
        Vector2 playerInput;
        /// <summary>        /// 相机高度        /// </summary>
        public float CameraHeight = 0;
        /// <summary>    /// 获取相机视锥    /// </summary>
        Vector3 CameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                Camera regularCamera = GetComponent<Camera>();
                //????????ε?Y??????С???????????????????????fieldOfView???????????????????????????????
                halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
                //????????????X???С
                halfExtends.x = halfExtends.y * regularCamera.aspect;
                //Z???????????????????????
                halfExtends.z = 0f;
                return halfExtends;
            }
        }

        //摄像机旋转的角度
        private float xRotation;
        private float yRotation;
        //鼠标移动距离
        private float xMouse;
        private float yMouse;

        private void Update()
        {
            xMouse = Input.GetAxis("Mouse X");
            yMouse = -Input.GetAxis("Mouse Y");
            SetCameraInput(yMouse, xMouse);
            setCameraPosition();
            ManualRotation();
        }
        /// <summary>
        /// 设置相机位置
        /// </summary>
        private void setCameraPosition()
        {
            //Vector3 CameraPlace = Player.transform.position;
            Vector3 CameraPlace = Control.PlayerControl.Instance.transform.position;
            CameraPlace.y += CameraHeight;//调整摄像机高度
            transform.position = CameraPlace;//同步位置
        }

        /// <summary>   /// 人为调整摄像机旋转  /// </summary>
        /// <returns>是否需要调整</returns>
        bool ManualRotation()
        {
            //调整的阈值
            float e = 0.001f;
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


        //以下内容是为了测试视角
        public Text t1;
        public Text t2;
        public Text t3;
        public void ChangeFOV(float newFOV)
        {
            GetComponent<Camera>().fieldOfView = newFOV;
            t1.text = newFOV.ToString();
        }
        public void ChangeSensitivity(float newSensitivity)
        {
            Sensitivity = newSensitivity;
            t2.text = newSensitivity.ToString(); 
        }
        public void ChangeHeight(float newHeight)
        {
            CameraHeight = newHeight;
            t3.text = newHeight.ToString();
        }
    }
}


