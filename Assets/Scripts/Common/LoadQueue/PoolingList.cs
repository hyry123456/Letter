namespace Common
{
    /// <summary>
    /// 池化数组案例，较快的插入删除的数组
    /// </summary>
    public class PoolingList<T>
    {
        private T[] list = new T[1];
        private int size = 0;
        public int Count => size;
        /// <summary>
        /// 获取数值中的第index个元素
        /// </summary>
        public T GetValue(int index)
        {
            return list[index];
        }

        public void Add(T node)
        {
            if(size == list.Length)
            {
                T[] newList = new T[(size + 1) * 2];
                for(int i=0; i<size; i++)
                    newList[i] = list[i];
                list = newList;
            }
            list[size] = node;
            size++;
        }

        public void Remove(int removeIndex)
        {
            if (removeIndex >= size) return;
            //将最后一个替换要删除的一个，达到复杂度为1的删除
            list[removeIndex] = list[size - 1];
            size--;
        }

        /// <summary>        /// 移除单个节点        /// </summary>
        /// <param name="node">移除的根据点</param>
        public void Remove(T node)
        {
            int reIndex = 0;
            for (; reIndex < size; reIndex++)
            {
                if (node.Equals(list[reIndex]))
                    break;
            }
            if (reIndex >= size) return;
            list[reIndex] = list[size - 1];
            size--;
        }
        public void RemoveAll()
        {
            size = 1;
        }

    }
}