using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    public int second;
    public int minute;
    /// <summary> 设置目标时间（正计时）  /summary>
    public int targetTime;
    float time = 0;
    /// <summary> 是否停止  /summary>
    public bool isStopped = false;
    /// <summary> 是否为倒计时  /summary>
    public bool isReverse = false;
    /// <summary> 文本组件  /summary>
    private Text component;
    private void Start()
    {
        component = GetComponent<Text>();
        component.text = minute.ToString() + ":" + second.ToString();
    }


    private void FixedUpdate()
    {
        time+=Time.deltaTime;
        if (time > 1)//一秒时间到了
        {
            time--;
            if (!isStopped)
            {
                if (!isReverse)
                {
                    forward();
                }
                else
                {
                    backward();
                }
                component.text = minute.ToString() + ":" + second.ToString();//屏幕上输出时间
            }
        }
    }
    /// <summary>正计时 </summary>
    private void forward()
    {
        second++;
        if (second == 60)
        {
            minute++;
            second -= 60;
        }
        if ((minute * 60 + second) == targetTime)
        {
            CountUpEvent();
        }

    }
    /// <summary>倒计时 </summary>
    private void backward()
    {
        if (second == 0 && minute == 0)
        {
            CountDownEvent();
            return;
        }
        second--;
        if (second < 0)
        {
            minute--;
            second += 60;
        }
    }
    /// <summary>修改当前时间 </summary>
    public void modifyTime(int length)
    {
        
        int min = length / 60;
        int sec = length % 60;
        second += sec;
        minute += min;
        while (second < 0)
        {

            second += 60;
            minute--;
        }
        if (minute < 0)
        {
            minute = 0;
            second = 0;
            isStopped = true;
            return;
        }
        while (second > 60)
        {
            second -= 60;
            minute++;
        }
    }
    /// <summary>设置目标时间（秒） </summary>
    public void setTargetTime(int time)
    {
        if (time <= 0) targetTime = 0;
        else targetTime = time;
    }
    /// <summary>暂停计时器 </summary>
    public void stopTimer()
    {
        isStopped = true;
    }
    /// <summary>恢复 </summary>
    public void resumeTimer()
    {
        isStopped = false;
    }
    /// <summary>计时器归零触发的事件 </summary>
    private void CountDownEvent()
    {

        isStopped = true;
    }
    /// <summary>计时器到达指定时间触发的事件 </summary>
    private void CountUpEvent()
    {

        isStopped = true;
    }

}
