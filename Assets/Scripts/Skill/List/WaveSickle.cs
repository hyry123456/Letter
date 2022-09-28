using UnityEngine;


namespace Skill
{

    /// <summary>  
    /// 主角的近战攻击技能，挥舞一下球  
    /// </summary>
    public class WaveSickle : SkillBase
    {
        GameObject origin;  //根据的原对象
        Vector3 begin, end;
        float nowRadio = 0;
        Info.CharacterInfo character;
        Sphere_Pooling useObj;  //实际使用的对象
        Transform manaTran;
        Camera cam;

        public WaveSickle()
        {
            expendSP = 0;
            nowCoolTime = 0;
            coolTime = 0.5f;
            skillName = "Wave Sickle";
            skillType = SkillType.NearDisAttack;
        }

        public override void OnSkillRelease(SkillManage mana)
        {
            if (origin == null)
                origin = Resources.Load<GameObject>("Prefab/Sphere_Pooling");
            if (character == null)
                character = mana.GetComponent<Info.CharacterInfo>();
            cam = Camera.main;
            if (cam == null) return;
            manaTran = mana.transform;

            begin = mana.transform.position + (cam.transform.forward +
                cam.transform.right + cam.transform.up) * character.nearAttackDistance;
            useObj = (Sphere_Pooling)Common.SceneObjectPool.Instance.GetObject(
                "Sphere_Pooling", origin, begin, manaTran.position);        //朝向我们这边，方便碰撞检测
            useObj.collsionEnter = (Collision collision) =>
            {
                Info.CharacterInfo character = collision.gameObject.GetComponent<Info.CharacterInfo>();
                Debug.Log("Attack");
                if (character == null) return;
                character.modifyHp(-10);
            };

            end = mana.transform.position + (cam.transform.forward + 
                -cam.transform.right + -cam.transform.up) * character.nearAttackDistance;
            nowRadio = 0;
            Common.SustainCoroutine.Instance.AddCoroutine(WaveSickleSustain);
        }

        bool WaveSickleSustain()
        {
            nowRadio += Time.deltaTime * 5;
            if(!useObj.gameObject.activeSelf)
            {
                //useObj.CloseObject();
                return true;
            }
            else if(nowRadio > 1 || cam == null)
            {
                useObj.CloseObject();
                return true;
            }

            begin = manaTran.position + (cam.transform.forward +
                cam.transform.right + cam.transform.up) * character.nearAttackDistance;
            end = manaTran.position + (cam.transform.forward +
                -cam.transform.right + -cam.transform.up) * character.nearAttackDistance;

            useObj.transform.position = Vector3.Lerp(begin, end, nowRadio);
            return false;
        }
    }
}