using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UI
{

    public abstract class UIDialogBase : MonoBehaviour
    {
        Queue<string> readyStrings;
        /// <summary>  /// 存储所有文本用的结构   /// </summary>
        protected StringBuilder sb;
        /// <summary>  /// 当前改变中的文字颜色  /// </summary>
        Color changeColor;
        /// <summary>  /// 当前透明中的文本   /// </summary>
        protected string alphaChar;
        /// <summary> /// 待添加的字符在当前显示的文本的编号   /// </summary>
        protected int nowIndex;
        /// <summary>   /// 每一个字符显示时需要的时间   /// </summary>
        float perCharWaitTime = 0.05f;
        /// <summary>  /// 当前显示的字符   /// </summary>
        protected StringBuilder nowShowString;

        /// <summary>   /// 结束时执行的行为    /// </summary>
        protected Common.INonReturnAndNonParam endBehavior;


        /// <summary>  /// 当UI的文本显示结束时执行的方法，用来关闭UI    /// </summary>
        protected abstract void CloseUI();
        /// <summary>
        /// 获得该行中真正的文本内容，也就是需要作为对话显示的部分，
        /// 比如剔除名称后的文本内容
        /// </summary>
        /// <param name="str">一行的文本数据</param>
        /// <returns>真正的文本数据</returns>
        protected abstract string ReadyOneLineString(string str);
        /// <summary>  
        /// 设置文本图片的内容显示内容，注意这个文本是包含颜色的文本，也就是已经剔除过其他数据，
        /// 只剩下正在用来显示的文本文件
        /// </summary>
        /// <param name="text">显示的内容</param>
        protected abstract void SetTextData(string text);
        /// <summary>  /// 获取需要用来显示的文本的颜色  /// </summary>
        protected abstract Color GetTextColor();
        /// <summary>  /// 逐帧检查是否等待完成    /// </summary>
        /// <returns>当等待完成时返回true</returns>
        protected abstract bool CheckWaitEnd();


        protected virtual void Update()
        {
            if (sb == null)
            {
                //为空有两种可能，一种是还在等待加载中
                if (readyStrings != null && readyStrings.Count > 0)
                {
                    if (CheckWaitEnd())
                    {
                        sb = new StringBuilder(ReadyOneLineString(readyStrings.Dequeue()));
                        nowShowString = new StringBuilder("");
                        nowIndex = 0;
                    }
                    else
                    {
                        return;
                    }
                }
                //一种是结束了，需要死亡
                else
                {
                    if (CheckWaitEnd())
                    {
                        CloseUI();
                        readyStrings = null;    //标记为这个对话条目已经结束
                    }
                    return;
                }

            }
            if (sb == null) return;
            if (alphaChar == null)
            {
                changeColor = GetTextColor(); changeColor.a = 0;
                if (nowIndex >= sb.Length)      //这行显示结束了，移除该行
                {
                    sb = null;
                    return;
                }
                alphaChar = sb[nowIndex].ToString();
                nowIndex++;
            }

            changeColor.a += Time.deltaTime * (1.0f / perCharWaitTime);
            if (changeColor.a >= 1)
            {
                nowShowString.Append(alphaChar);
                alphaChar = null;
                SetTextData(nowShowString.ToString());
            }
            else
            {
                SetTextData(nowShowString + "<color=\"#" + ColorUtility.ToHtmlStringRGBA(changeColor)
                    + "\">" + alphaChar + "</color>");
            }
        }

        /// <summary>
        /// 插入对话，将输入的字符按行分割插入准备显示数组中
        /// </summary>
        /// <param name="strs">需要显示的所有文本，文本用换行符分割</param>
        /// <returns>是否启动成功，当正在显示时会放回false</returns>
        protected bool InsertDialog(string strs)
        {
            if (readyStrings != null) return false;
            List<string> strLists = new List<string>(strs.Split('\n'));
            nowIndex = 0;
            readyStrings = new Queue<string>();
            for (int i = 0; i < strLists.Count; i++)
            {
                readyStrings.Enqueue(strLists[i]);
            }
            alphaChar = null;
            sb = null;

            //初始化第一行
            sb = new StringBuilder(ReadyOneLineString(readyStrings.Dequeue()));
            nowShowString = new StringBuilder("");
            nowIndex = 0;
            return true;
        }


    }
}