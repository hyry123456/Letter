

using UnityEngine;

namespace Common.ResetInput
{
    public struct MyInputValueStruct
    {
        /// <summary>
        /// 这个轴的类型，是仅有按下还是有增加和减少
        /// </summary>
        public MyInputValueType valueType;
        //当前这个轴的值
        public float value;
        /// <summary>
        /// 这个轴的检测按键，单一时默认检测Up键
        /// </summary>
        public KeyCode valueUp;
        public KeyCode valueDown;
        /// <summary>
        /// 变化速度
        /// </summary>
        public float changeSpeed;
        /// <summary>
        /// 这个轴的名称
        /// </summary>
        public string asisName;
        //[HideInInspector]
        /// <summary>
        /// 之前的值
        /// </summary>
        public int forValue;
        //[HideInInspector]
        /// <summary>
        /// 轴的变化是否有改变
        /// </summary>
        public bool forValueIsChange;
    }

    [System.Serializable]
    /// <summary>
    /// 用于外部读取用的显示结构体
    /// </summary>
    public struct InputValueOutReadStruct
    {
        /// <summary>
        /// 这个轴的类型，是仅有按下还是有增加和减少
        /// </summary>
        public MyInputValueType valueType;
        //当前这个轴的值
        public float value;
        /// <summary>
        /// 这个轴的检测按键，单一时默认检测Up键
        /// </summary>
        public string valueUp;
        public string valueDown;
        /// <summary>
        /// 变化速度
        /// </summary>
        public float changeSpeed;
        /// <summary>
        /// 这个轴的名称
        /// </summary>
        public string asisName;
        //[HideInInspector]
        /// <summary>
        /// 之前的值
        /// </summary>
        public int forValue;
        //[HideInInspector]
        /// <summary>
        /// 轴的变化是否有改变
        /// </summary>
        public bool forValueIsChange;
    }
}