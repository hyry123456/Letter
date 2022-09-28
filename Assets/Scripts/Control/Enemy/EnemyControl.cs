using Common;
using UnityEngine;

namespace Control
{

    /// <summary>
    /// 一个简单的敌人AI
    /// </summary>
    public class EnemyControl : ObjectPoolBase
    {
        Transform player;
        Motor.EnemyMotor motor;
        Info.EnemyInfo enemyInfo;
        /// <summary>
        /// 用来判断是否看到过主角，看到了就要一直追赶
        /// </summary>
        private bool isSee= false;
        public LayerMask shelterMask;

        float time;
        /// <summary>     /// 每一次射出子弹需要等待的时间  /// </summary>
        public float buttleRayTime;
        public GameObject originBullet;

        //角度无所谓，毕竟敌人是个球
        public override void InitializeObject(Vector3 positon, Quaternion quaternion)
        {
            base.InitializeObject(positon, quaternion);
            player = PlayerControl.Instance.transform;
            motor = GetComponent<Motor.EnemyMotor>();
            enemyInfo = GetComponent<Info.EnemyInfo>();
        }

        public override void InitializeObject(Vector3 positon, Vector3 lookAt)
        {
            base.InitializeObject(positon, lookAt);
            player = PlayerControl.Instance.transform;
            motor = GetComponent<Motor.EnemyMotor>();
            enemyInfo = GetComponent<Info.EnemyInfo>();
        }

        private void FixedUpdate()
        {
            if (player == null || motor == null) return;
            Vector3 playerDir = player.position - transform.position;
            float seeDis = enemyInfo.seeDistance;
            bool isFar = false;
            if (!isSee)
            {
                if(playerDir.sqrMagnitude <= seeDis * seeDis)
                    isSee = true;
                return;
            }
            //大于可视范围退出
            if (playerDir.sqrMagnitude > seeDis * seeDis)
            {
                isFar = true;
            }
            playerDir.Normalize();
            RaycastHit hit;
            //判断是否有可以遮挡的物体
            if(Physics.Raycast(transform.position, playerDir, out hit, seeDis, shelterMask))
            {
                if (hit.collider.tag != "Player")
                {
                    //Debug.DrawRay(transform.position, playerDir.normalized * hit.distance);
                    playerDir += hit.normal * 1.5f;
                    motor.Move(playerDir.z, playerDir.x);
                    time = 0;
                    return;
                }
            }
            if (isFar)      //距离遥远，追赶
            {
                motor.Move(playerDir.z, playerDir.x);
                time = 0;
                return;
            }

            time += Time.fixedDeltaTime;
            motor.Move(0, 0);       //停下来射击
            if (time > buttleRayTime)
            {
                time = 0;
                Bullet_Pooling bullet_Pooling = (Bullet_Pooling)Common.SceneObjectPool.Instance.GetObject("Pooling_Bullet", originBullet,
                    transform.position + playerDir * 2, player.position);
                bullet_Pooling.attackTargetTag = "Player";
            }
        }

        protected override void OnEnable()
        {
            time = 0;
        }
    }
}