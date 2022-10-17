using UnityEngine;


namespace Interaction
{

    public class OnTriggerClose : MonoBehaviour
    {
        string checkItemName = "祝福试炼地的信件";
        private void OnCollisionEnter(Collision collision)
        {
            if(Package.PackageSimple.Instance.CheckItemByName(checkItemName))
            {
                GameObject.Destroy(GetComponent<Collider>());
            }
        }

    }
}