using UnityEngine;

namespace Motor
{
    public class FirstPersonCameraControl : MonoBehaviour
    {
        /// <summary>        /// ��ҽڵ�     /// </summary>
        public GameObject Player;
        /// <summary>        /// �û������������        /// </summary>
        public float Sensitivity = 200;
        /// <summary>        /// �û����������        /// </summary>
        Vector2 playerInput;
        /// <summary>        /// ����ĸ߶�        /// </summary>
        public float CameraHeight = 0;
        /// <summary>    /// ȷ�����������׶��Ĵ�С    /// </summary>
        Vector3 CameraHalfExtends
        {
            get
            {
                Vector3 halfExtends;
                Camera regularCamera = GetComponent<Camera>();
                //ȷ��ͶӰ���ε�Y��һ���С����ֱ��ʹ�ý�ƽ���һ������ΪfieldOfView�����һ�������ţ�������Ҫ�����ű��ȥ
                halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
                //���ݱ��������X���С
                halfExtends.x = halfExtends.y * regularCamera.aspect;
                //Z����ͶӰ���������ã�û��Ӱ��
                halfExtends.z = 0f;
                return halfExtends;
            }
        }

        //�����ת�ķ�����Ϊ�˷���ʹ�ð����Ƿ�����ȫ��
        private float xRotation;
        private float yRotation;
        //����ƶ��ķ�����Ϊ�˷���ʹ�ð����Ƿ�����ȫ��
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
        /// ���������λ��
        /// </summary>
        private void setCameraPosition()
        {
            Vector3 CameraPlace = Player.transform.position;
            CameraPlace.y += CameraHeight;//��΢����������ĸ߶�
            transform.position = CameraPlace;//���������λ��
        }

        /// <summary>   /// ������ת�Ƕ�  /// </summary>
        /// <returns>�Ƿ���Ҫ������ת</returns>
        bool ManualRotation()
        {
            //�������ֵ
            float e = 0.001f;
            //�ж��Ƿ�������,ע�����������
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
    }
}


