using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ÏÐÖÃ×´Ì¬
/// </summary>
public class IdleStates : IState
{
    private FSM manager;

    public IdleStates(FSM manager)
    {
        this.manager = manager;
    }

    public void OnEnter()
    {
        manager.parameter.animator.Play("Idle");
    }

    public void OnExit()
    {
        
    }

    public void OnUpdate()
    {
        Vector3 delta = manager.parameter.target.position - manager.transform.position;//×ø±ê²î
        float distance = delta.sqrMagnitude;//¾àÀëµÄÆ½·½
        if(distance < manager.parameter.viewRange)//ÔÚÊÓÒ°·¶Î§ÄÚ
        {
            manager.TransitionState(EnemyStateType.Chase);//×·¸ÏÍæ¼Ò
        }
    }
    
}
/// <summary>
/// ×·»÷×´Ì¬
/// </summary>
public class ChaseStates : IState
{
    private FSM manager;
    
    public ChaseStates(FSM manager)
    {
        this.manager = manager;
        
    }

    public void OnEnter()
    {
        manager.parameter.animator.Play("Chase");
    }

    public void OnExit()
    {
        manager.parameter.agent.destination = manager.transform.position;
    }

    public void OnUpdate()
    {
        manager.parameter.agent.destination = manager.parameter.target.position;
        Vector3 delta = manager.parameter.target.position - manager.transform.position;//×ø±ê²î
        float distance = delta.sqrMagnitude;//¾àÀëµÄÆ½·½
        if( distance > manager.parameter.viewRange)
        {
            manager.TransitionState(EnemyStateType.Idle);
        }
        if (distance < manager.parameter.attackRange)
        {
            manager.TransitionState(EnemyStateType.Attack);
        }
    }

}
/// <summary>
/// ¹¥»÷×´Ì¬
/// </summary>
public class AttackStates : IState
{
    private FSM manager;
    
    public AttackStates(FSM manager)
    {
        this.manager = manager;
        
    }

    public void OnEnter()
    {
        manager.parameter.animator.Play("Attack");
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        Vector3 delta = manager.parameter.target.position - manager.gameObject.transform.position;//×ø±ê²î
        float distance = delta.sqrMagnitude;//¾àÀëµÄÆ½·½
        if (distance > manager.parameter.attackRange)
        {
            manager.TransitionState(EnemyStateType.Chase);
        }
    }

    public void Attack()
    {
        manager.parameter.animator.Play("Attack");
    }
}
/// <summary>
/// ËÀÍö×´Ì¬
/// </summary>
public class DieStates : IState
{
    private FSM manager;

    private int Timer = 0;
    
    public DieStates(FSM manager)
    {
        this.manager = manager;
        
    }

    public void OnEnter()
    {
        manager.parameter.animator.Play("Die");
        manager.parameter.target = null;

    }

    public void OnExit()
    {
        throw new System.Exception("Illegal State Change");
    }

    public void OnUpdate()
    {
        Timer++;
        if(Timer > 100)
        {
            GameObject.Destroy(manager.gameObject);//¶¨Ê±Ïú»Ù
        }
    }

}
