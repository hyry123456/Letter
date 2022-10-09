using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "UI/TextureDictionaries")]
    /// <summary> /// ��ͼ�ֵ䣬�������Ʋ���ͼƬ  /// </summary>
    public class TextureDictionaries : ScriptableObject
    {
        [SerializeField]
        Sprite[] textures;

        Dictionary<string, Sprite> texturesDictionary;

        public void LoadTextureDictionarie()
        {
            texturesDictionary = new Dictionary<string, Sprite>(textures.Length);
            for(int i=0; i<textures.Length; i++)
            {
                texturesDictionary.Add(textures[i].name, textures[i]);
            }
        }

        public Sprite GetTexture(string name)
        {
            return texturesDictionary[name];
        }
    }
}