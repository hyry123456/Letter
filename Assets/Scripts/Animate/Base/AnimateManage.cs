using UnityEngine;

/// <summary>
/// 动画类基类，用来进行动画播放
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimateManage : MonoBehaviour
{
    /// <summary>  /// 动画控制类,用来切换主角动画  /// </summary>
    Animator animator;

    protected void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimate(AnimateType animateType)
    {
        if (animator == null) return;
        switch (animateType)
        {
            case AnimateType.Idle:
                //animator.SetBool("Idle", false);
                animator.SetBool("Move", false);
                //animator.SetBool("RotateLeft", false);
                //animator.SetBool("RotateRight", false);
                return;
            case AnimateType.Move:
                //animator.SetBool("Idle", false);
                animator.SetBool("Move", true);
                //animator.SetBool("RotateLeft", false);
                //animator.SetBool("RotateRight", false);
                return;
            case AnimateType.RotateLeft:
                //animator.SetBool("Idle", false);
                animator.SetBool("Move", false);
                //animator.SetBool("RotateLeft", true);
                //animator.SetBool("RotateRight", false);
                return;
            case AnimateType.RotateRight:
                //animator.SetBool("Idle", false);
                animator.SetBool("Move", false);
                //animator.SetBool("RotateLeft", false);
                //animator.SetBool("RotateRight", true);
                return;
            case AnimateType.Attack:
                animator.SetBool("Move", false);
                animator.Play("攻击");
                return;
            case AnimateType.Die:
                animator.SetBool("Move", false);
                animator.Play("死亡");
                return;
        }
    }

    public void CloseAnimate(AnimateType animateType)
    {
        if (animator == null) return;
        animator.SetBool(AnimateMap.AnimateTypeToName(animateType), false);
    }




}
