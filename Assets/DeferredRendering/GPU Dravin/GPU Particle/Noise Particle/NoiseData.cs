using UnityEngine;

namespace DefferedRender
{
    [System.Serializable]
    public struct NoiseParticleData
    {
        public Vector4 random;          //xyz是随机数，w是目前存活时间
        public Vector2Int index;             //状态标记，x是当前编号，y是是否存活
        public Vector3 worldPos;        //当前位置
        public Vector4 uvTransData;     //uv动画需要的数据
        public float interpolation;    //插值需要的数据
        public Vector4 color;           //颜色值，包含透明度
        public float size;             //粒子大小
        public Vector3 nowSpeed;        //xyz是当前速度，w是存活时间
        public float liveTime;         //最多存活时间
    };


    public struct ParticleInitializeData
    {
        public Vector3 beginPos;        //该组粒子运行初始位置
        public Vector3 velocityBeg;     //初始速度范围
        public Vector3 velocityEnd;     //初始速度范围
        public Vector3Int InitEnum;     //初始化的根据编号
        public Vector2 sphereData;      //初始化球坐标需要的数据
        public Vector3 cubeRange;       //初始化矩形坐标的范围
        public Matrix4x4 transfer_M;    //初始化矩形坐标的范围
        public Vector2 lifeTimeRange;   //生存周期的范围

        public Vector3 noiseData;       //噪声调整速度时需要的数据

        public Vector3Int outEnum;      //确定输出时算法的枚举
        public Vector2 smoothRange;       //粒子的大小范围

        public uint arriveIndex;
    };

    public enum SizeBySpeedMode
    {
        TIME = 0,
        X = 1,
        Y = 2,
        Z = 3,
    }

    public enum InitialShapeMode
    {
        Pos = 0,
        Sphere = 1,
        Cube = 2
    }

    /// <summary>   /// 用来控制单组噪声粒子的数据结构体    /// </summary>
    [System.Serializable]
    public class NoiseData 
    {
        //初始化
        public InitialShapeMode shapeMode = InitialShapeMode.Pos;
        public Transform position;
        [Range(0.01f, 6.18f)]
        public float arc = 0.1f;              //粒子生成范围
        public float radius = 1;           //圆大小
        public Vector3 cubeRange;          //矩形大小
        public Vector3 velocityBegin;      //速度范围
        public Vector3 velocityEnd;
        public Vector2 lifeTime = new Vector2(0.01f, 1);

        //更新
        [Range(1, 8)]
        public int octave = 1;
        public float frequency = 1;
        [Min(0.1f)]
        public float intensity = 0.5f;

        //输出粒子
        public bool isSizeBySpeed;
        public SizeBySpeedMode sizeBySpeedMode = SizeBySpeedMode.TIME;
        public Vector2 smoothRange = Vector2.up;

        //设置
        public bool isPhysical;     //物理粒子
    }
}