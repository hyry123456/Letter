using UnityEngine;
using UnityEngine.UI;

namespace Motor
{
    public class FirstPersonCameraControl : MonoBehaviour
    {
        /// <summary>        /// ��ҽڵ�     /// </summary>
        public GameObject Player;
        /// <summary>        /// ������        /// </summary>
        public float Sensitivity = 200;
        /// <summary>        /// �������        /// </summary>
        Vector2 playerInput;
        /// <summary>        /// ����߶�        /// </summary>
        public float CameraHeight = 0;
        /// <summary>    /// ��ȡ�����׶    /// </summary>
        Vector3 CameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                Camera regularCamera = GetComponent<Camera>();
                //????????��?Y??????��???????????????????????fieldOfView???????????????????????????????
                halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
                //????????????X???��
                halfExtends.x = halfExtends.y * regularCamera.aspect;
                //Z???????????????????????
                halfExtends.z = 0f;
                return halfExtends;
            }
        }

        //�������ת�ĽǶ�
        private float xRotation;
        private float yRotation;
        //����ƶ�����
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
        /// �������λ��
        /// </summary>
        private void setCameraPosition()
        {
            //Vector3 CameraPlace = Player.transform.position;
            Vector3 CameraPlace = Control.PlayerControl.Instance.transform.position;
            CameraPlace.y += CameraHeight;//����������߶�
            transform.position = CameraPlace;//ͬ��λ��
        }

        /// <summary>   /// ��Ϊ�����������ת  /// </summary>
        /// <returns>�Ƿ���Ҫ����</returns>
        bool ManualRotation()
        {
            //��������ֵ
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
        /// �����ƶ��Ĳ��ֵ�ж���ת�Ƕȣ�ע�⴫��ֵҪ��׼����
        /// ����Ϊ��̬��Ϊ�����������Ҫ�õ��������ݣ����ֻ�ÿ���һ��������͹���
        /// </summary>
        static float GetAngle(Vector2 direction)
        {
            //ͨ�������Һ����������ת������ƶ���������Ҫ��yֵ�Ƕ�
            float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
            //�ж����ıߣ�Ҳ����˳ʱ�뻹����ʱ��
            return direction.x < 0f ? 360f - angle : angle;
        }

        /// <summary>   /// ���ƽǶȴ�С  /// </summary>
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
        /// <summary>   /// ���������ת�Ƕȵ�����  /// </summary>
        public void SetCameraInput(float mouseY, float mouseX)
        {
            playerInput = new Vector2(mouseY, mouseX);
        }


        //����������Ϊ�˲����ӽ�
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


