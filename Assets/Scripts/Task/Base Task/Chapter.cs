
using System.Collections.Generic;
using Interaction;

namespace Task
{
    /// <summary>
    /// �½��࣬Ҳ��������ϵͳ��ÿһϵ�����������
    /// </summary>
    public abstract class Chapter
    {
        /// <summary>        /// �½����ƣ���ʽʱ��Ҫ�õ�        /// </summary>
        public string chapterName;
        /// <summary>        /// �½ڱ��⣬���������Ϸ��������ʾ����������        /// </summary>
        public string chapterTitle;
        /// <summary>        /// ���½�����        /// </summary>
        public int taskPartCount;
        /// <summary>        /// ��ǰ���½�        /// </summary>
        protected ChapterPart part;
        /// <summary>        /// �½ڱ��        /// </summary>
        public int chapterID;
        /// <summary>        /// ��ǰ��ɵ����½ڱ��        /// </summary>
        public int nowCompletePartId;
        /// <summary>        /// ���������Ҫ�ı��ļ����ø�·���洢�ļ�        /// </summary>
        public string chapterSavePath;
        /// <summary>        /// �ı���ȡ��Ĵ洢λ��        /// </summary>
        private List<string> readData;

        /// <summary>
        /// ���С�������������������ʱʱ��飬�ж��Ƿ���Խ�����һ������״̬
        /// </summary>
        /// <param name="info">������Ϣ</param>
        public abstract void CheckTask(InteracteInfo info);
        /// <summary>
        /// �����½��Ƿ��������������ʱ�Լ����ü��ط�����
        /// �÷���ֻ����δ�����δ��ȡʱ�Ż����
        /// </summary>
        public abstract void CheckAndLoadChapter();

        /// <summary>
        /// �ı�����С��ʱ����
        /// </summary>
        public abstract void ChangeTask();

        /// <summary>
        /// ���½ڿ���ʱ���õķ�����Ҳ�����½ڵ�׼������������������Դ���ʱ����
        /// </summary>
        public abstract void BeginChapter();

        /// <summary>        /// ��������С�½ڵ�ͬʱ���������½�        /// </summary>
        public abstract void SetNowTaskPart(int nowPart);

        /// <summary>
        /// �˳��½�ʱ�䣬�����Ҫ����һϵ�е��˳���Ϊ��
        /// ���Խ������鵽Э�̿������ϣ��������Լ�����Э��
        /// </summary>
        public abstract void ExitChapter();

        /// <summary>
        /// ���½����ʱ���еķ�����������ء�ɾ������֮��ģ�
        /// Ŀǰ�ȷ��������ȣ�֮��������λ��
        /// </summary>
        public abstract void CompleteChapter();

        /// <summary>        /// ������½�����        /// </summary>
        public virtual string GetPartName()
        {
            return part.partName;
        }

        /// <summary>        /// ������½�����        /// </summary>
        public virtual string GetPartDescribe()
        {
            return part.partDescribe;
        }

        /// <summary>
        /// ���ضԻ��������ı�·����ȡ���ݣ��洢�������У�
        /// �����ı�����ȡһ��
        /// </summary>
        /// <param name="part">��ȡ�ڼ�����</param>
        /// <returns>�ò��ֵ��ı�</returns>
        public string GetDiglogText(int part)
        {
            if(readData == null)
            {
                readData = Common.FileReadAndWrite.ReadFileByAngleBrackets(chapterSavePath);
            }
            return readData[part];
        }
    }
}