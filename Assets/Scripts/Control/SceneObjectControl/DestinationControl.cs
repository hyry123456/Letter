using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationControl : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Info.CharacterInfo>().getScore() > 3)
        {
            Debug.Log("Win!");
        }
        else
        {
            Debug.Log("Not Enough Letters");
        }
    }
}
