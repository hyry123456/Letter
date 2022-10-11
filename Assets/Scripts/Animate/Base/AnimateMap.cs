
public enum AnimateType
{
    Idle = 0,           //待机
    Move = 1,           //移动
    RotateLeft = 2,     //左转
    RotateRight = 3,    //右转
    Attack = 4,         //攻击
    Die = 5,            //死亡
}

/// <summary>
/// 动画映射类，用类型来指定动画
/// </summary>
public static class AnimateMap
{
    public static string AnimateTypeToName(AnimateType type)
    {
        return type.ToString();
    }
}
