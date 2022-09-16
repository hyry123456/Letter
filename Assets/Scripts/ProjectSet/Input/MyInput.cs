
using System.Collections.Generic;
using UnityEngine;


namespace Common.ResetInput
{
    [System.Serializable]
    /// <summary>
    /// 我的Input数据类
    /// </summary>
    public class MyInput : MonoBehaviour
    {
        private static MyInput input = null;
        [SerializeField]
        public MyInputValueStruct[] myInputValues;

        public string targetPath = "InputFile.input";

        public void OnEnable()
        {
            input = this;
            LoadInputValue();
            gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        public static MyInput Instance
        {
            get
            {
                if(input == null)
                {
                    GameObject game = new GameObject();
                    game.hideFlags = HideFlags.HideAndDontSave;
                    input = game.AddComponent<MyInput>();
                    game.name = "MyInput";
                }
                return input;
            }
            set
            {
                input = value;
            }
        }

        /// <summary>
        /// 清除现有对象
        /// </summary>
        public static void DestoryNow()
        {
            if(input == null) return;
            GameObject game = input.gameObject;
            input = null;
            GameObject.DestroyImmediate(game);
        }


        private void FixedUpdate()
        {
            //循环刷新每一个轴的值
            for(int i=0; i<myInputValues.Length; i++)
            {
                //单一按键情况
                if(myInputValues[i].valueType == MyInputValueType.Single)
                {
                    //按下了
                    if (Input.GetKey(myInputValues[i].valueUp))
                    {
                        myInputValues[i].value += myInputValues[i].changeSpeed * Time.fixedDeltaTime;
                        myInputValues[i].value = Mathf.Clamp01(myInputValues[i].value);
                        //检测轴值是否有变化
                        if(myInputValues[i].forValue != 1)
                        {
                            myInputValues[i].forValue = 1;
                            myInputValues[i].forValueIsChange = true;
                        }
                        else myInputValues[i].forValueIsChange = false;
                    }
                    //没有按下
                    else
                    {
                        myInputValues[i].value -= myInputValues[i].changeSpeed * Time.fixedDeltaTime;
                        myInputValues[i].value = Mathf.Clamp01(myInputValues[i].value);
                        if (myInputValues[i].forValue != 0)
                        {
                            myInputValues[i].forValue = 0;
                            myInputValues[i].forValueIsChange = true;
                        }
                        else myInputValues[i].forValueIsChange = false;
                    }
                }
                //双重按键情况
                else
                {
                    //按下增加
                    if (Input.GetKey(myInputValues[i].valueUp))
                    {
                        myInputValues[i].value += 2 * myInputValues[i].changeSpeed * Time.fixedDeltaTime;
                        myInputValues[i].value = Mathf.Clamp(myInputValues[i].value, -1, 1);
                        if (myInputValues[i].forValue != 1)
                        {
                            myInputValues[i].forValue = 1;
                            myInputValues[i].forValueIsChange = true;
                        }
                        else myInputValues[i].forValueIsChange = false;
                    }
                    //按下减少
                    else if (Input.GetKey(myInputValues[i].valueDown))
                    {
                        myInputValues[i].value -= 2 * myInputValues[i].changeSpeed * Time.fixedDeltaTime;
                        myInputValues[i].value = Mathf.Clamp(myInputValues[i].value, -1, 1);
                        if (myInputValues[i].forValue != -1)
                        {
                            myInputValues[i].forValue = -1;
                            myInputValues[i].forValueIsChange = true;
                        }
                        else myInputValues[i].forValueIsChange = false;
                    }
                    //啥都没按
                    else
                    {
                        myInputValues[i].value = 
                            Mathf.Lerp(myInputValues[i].value, 0, myInputValues[i].changeSpeed * Time.fixedDeltaTime); ;
                        if (myInputValues[i].forValue != 0)
                        {
                            myInputValues[i].forValue = 0;
                            myInputValues[i].forValueIsChange = true;
                        }
                        else myInputValues[i].forValueIsChange = false;
                    }
                }
            }
        }

        /// <summary>
        /// 获得这个轴的值
        /// </summary>
        /// <param name="asisName">轴名称</param>
        /// <returns>返回该轴的值</returns>
        public float GetAsis(string asisName)
        {
            for(int i=0; i< myInputValues.Length; i++)
            {
                if (myInputValues[i].asisName.Equals(asisName))
                {
                    return myInputValues[i].value;
                }
            }

            Debug.LogError(asisName + " is null");
            return 0;
        }

        /// <summary>
        /// 获得该轴的结构体，用来确定一些数据时使用
        /// </summary>
        /// <param name="asxisName">轴名称</param>
        /// <param name="myInputValueStruct">输出值的存放位置</param>
        /// <returns>是否有该轴</returns>
        public bool GetAsxisStruct(string asxisName, out MyInputValueStruct myInputValueStruct)
        {
            for (int i = 0; i < myInputValues.Length; i++)
            {
                if (myInputValues[i].asisName.Equals(asxisName))
                {
                    myInputValueStruct = myInputValues[i];
                    return true;
                }
            }
            myInputValueStruct = new MyInputValueStruct();
            return false;
        }

        /// <summary>
        /// 检测该轴是否被按下
        /// </summary>
        /// <param name="axisName">轴名称</param>
        public bool GetButton(string axisName)
        {
            for (int i = 0; i < myInputValues.Length; i++)
            {
                if (myInputValues[i].asisName.Equals(axisName))
                {
                    if(myInputValues[i].forValue == 1 || myInputValues[i].forValue == -1)
                        return true;
                    return false;
                }
            }

            Debug.LogError(axisName + " is null");
            return false;
        }

        /// <summary>
        /// 检测该轴是否被按下的第一帧放回true
        /// </summary>
        /// <param name="axisName">轴名称</param>
        public bool GetButtonDown(string axisName)
        {
            for (int i = 0; i < myInputValues.Length; i++)
            {
                if (myInputValues[i].asisName.Equals(axisName))
                {
                    if ((myInputValues[i].forValue == 1 || myInputValues[i].forValue == -1) 
                        && myInputValues[i].forValueIsChange)
                        return true;
                    return false;
                }
            }

            Debug.LogError(axisName + " is null");
            return false;
        }

        /// <summary>
        /// 该轴松开的第一帧返回true
        /// </summary>
        /// <param name="axisName">轴名称</param>
        public bool GetButtonUp(string axisName)
        {
            for (int i = 0; i < myInputValues.Length; i++)
            {
                if (myInputValues[i].asisName.Equals(axisName))
                {
                    if (myInputValues[i].forValue == 0 && myInputValues[i].forValueIsChange)
                        return true;
                    return false;
                }
            }

            Debug.LogError(axisName + " is null");
            return false;
        }

        /// <summary>
        /// 根据轴名称确定轴对象编号
        /// </summary>
        /// <param name="axisName">轴名称</param>
        /// <returns>轴对象编号</returns>
        public int GetInputValueStruct(string axisName)
        {
            for (int i = 0; i < myInputValues.Length; i++)
            {
                if (myInputValues[i].asisName.Equals(axisName))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 读取Input数据存储的位置
        /// 数据格式：<Type=n,up=n,down=n,speed=n,name=n>
        /// </summary>
        public void LoadInputValue()
        {
            string value =
                Common.FileReadAndWrite.DirectReadFile(
                    Application.streamingAssetsPath + "/" +this.targetPath);
            List<MyInputValueStruct> values = new List<MyInputValueStruct>();
            for(int i=0; i<value.Length; i++)
            {
                int begin = value.IndexOf('<', i);
                if (begin == -1) break;
                i = value.IndexOf('>', begin);
                //获得一行数据
                string temp = value.Substring(begin+1, i-begin-1);
                //切割数据，确定每一个区域的值
                string[] strs = temp.Split(',');
                MyInputValueStruct temStrtcu = new MyInputValueStruct();

                //根据一行数据设置值
                temStrtcu.valueType = (MyInputValueType)int.Parse(strs[0].Split('=')[1]);
                //根据ASCLL码获取对应的按键
                temStrtcu.valueUp = (KeyCode)int.Parse(strs[1].Split('=')[1]);
                temStrtcu.valueDown = (KeyCode)int.Parse(strs[2].Split('=')[1]);
                temStrtcu.changeSpeed = float.Parse(strs[3].Split('=')[1]);
                temStrtcu.asisName = strs[4].Split('=')[1];
                values.Add(temStrtcu);
            }
            myInputValues = values.ToArray();
            values.Clear();
        }

        /// <summary>
        /// 重新保存Input数据
        /// </summary>
        public void ResetInputValue()
        {
            string loadStr = "";
            if (myInputValues == null) return;
            for(int i=0; i<myInputValues.Length; i++)
            {
                loadStr += "<Type=" + ((int)myInputValues[i].valueType).ToString() + "," +
                    "up=" + ((int)myInputValues[i].valueUp).ToString() + "," +
                    "down=" + ((int)myInputValues[i].valueDown).ToString() + ","+
                    "speed=" + myInputValues[i].changeSpeed.ToString() + "," +
                    "name=" + myInputValues[i].asisName + ">\n";
            }
            Common.FileReadAndWrite.WriteFile(
                    Application.streamingAssetsPath + "/" + this.targetPath, loadStr);
        }

    }
}