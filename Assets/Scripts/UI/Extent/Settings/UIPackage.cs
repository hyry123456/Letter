using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Package;
using Common;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// UI�ı�����ʾ���࣬������ʾӵ�е�����
    /// </summary>
    public class UIPackage : MonoBehaviour
    {
        Text itemName,          //��ǰ��ʾ����������
            itemDescription;    //��ǰ��ʾ����������

        private PoolingList<UIPackageItem> items;

        private void OnEnable()
        {
            //�������е�items
        }
    }
}