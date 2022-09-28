using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    [SerializeField]
    public AnimationCurve cure;
    void Start()
    {
        Keyframe[] keyframes = cure.keys;
        for(int i=0; i<keyframes.Length; i++)
        {
            Debug.Log(i + " time" + keyframes[i].time);
            Debug.Log(i + " value" + keyframes[i].value);
            Debug.Log(i + " in" + keyframes[i].inTangent);
            Debug.Log(i + " out" + keyframes[i].outTangent);
        }
    }

}
