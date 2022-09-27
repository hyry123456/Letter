using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    public int second;
    public int minute;
    public int targetTime;
    float time = 0;
    /// <summary> 是否为倒计时  /summary>
    public bool isReverse = false;


    private Text component;
    private void Start()
    {
        component = GetComponent<Text>();
    }



    private void FixedUpdate()
    {
        time+=Time.deltaTime;
        if (time > 1)//一秒时间到了
        {
            time--;
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
    private void backward()
    {
        if (second == 0 && minute == 0)
        {
            CountDownEvent();
            return;
        }
        second--;
        if (second == 0)
        {
            minute--;
            second += 60;
        }
    }
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
            return;
        }
        while (second > 60)
        {
            second -= 60;
            minute++;
        }
    }
    /// <summary>计时器归零触发的事件 </summary>
    private void CountDownEvent()
    {

    }
    /// <summary>计时器到达指定时间触发的事件 </summary>
    private void CountUpEvent()
    {

    }

}
