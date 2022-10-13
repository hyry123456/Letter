using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "UI/TextureDictionaries")]
    /// <summary> /// ÌùÍ¼×Öµä£¬¸ù¾ÝÃû³Æ²éÕÒÍ¼Æ¬  /// </summary>
    public class TextureDictionaries : ScriptableObject
    {
        private static TextureDictionaries instance;
        public static TextureDictionaries Instance
        {
            get
            {
                if(instance == null)
                {
                    instance =
                        Resources.Load<TextureDictionaries>("UI/TextureDictionaries");
                    instance.LoadTextureDictionarie();
                }
                return instance;
            }
        }

        [SerializeField]
        Sprite[] textures;

        Dictionary<string, Sprite> texturesDictionary;

        private void LoadTextureDictionarie()
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