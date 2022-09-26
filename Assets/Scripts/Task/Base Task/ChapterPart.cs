

using System;
using UnityEngine;

namespace Task
{
    public abstract class ChapterPart
    {
        public string partName;
        public string partDescribe;

        /// <summary>
        /// 进入一章任务的一部分，该方法是初始化方法，在任务时会调用，
        /// 分第一次进入和已经进入过在加载时进入，用来区分任务的一些内容是否加载
        /// </summary>
        /// <param name="chapter">所属章节</param>
        /// <param name="isLoaded">是否加载过，false就是第一次加载</param>
        public abstract void EnterTaskEvent(Chapter chapter, bool isLoaded);
        /// <summary>
        /// 是否任务完成任务，用于检测任务是否完成，一般只有在传入一个任务交互，同时该交互的对应章节ID
        /// 就是该任务时才会进行使用，一般不用担心是否会交互出错，也就是被其他任务调用了
        /// </summary>
        /// <param name="chapter">章节名称</param>
        /// <param name="interactionInfo">交互信息</param>
        /// <returns>是否完成任务</returns>
        public abstract bool IsCompleteTask(Chapter chapter, Interaction.InteracteInfo info);
        /// <summary>
        /// 章节退出事件，由IsCompleteTask返回任务完成(true)后由ChangeTask调用，
        /// 用来表示该小节结束，进入下一小节
        /// </summary>
        /// <param name="chapter"></param>
        public abstract void ExitTaskEvent(Chapter chapter);

        /// <summary>
        /// 一个封装好的函数，用于对于一些刚刚获取的物体，进行交互数据清空
        /// </summary>
        /// <param name="gameObject">需要进行清空的物体</param>
        public void DestoryObjAllInteracte(GameObject gameObject)
        {
            Interaction.InteractionBase[] interactionInfos = gameObject.GetComponentsInParent<Interaction.InteractionBase>();
            for(int i=0; i<interactionInfos.Length; i++)
            {
                GameObject.Destroy(interactionInfos[i]);
            }
        }

    }
}