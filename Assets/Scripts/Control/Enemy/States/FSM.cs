using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/// <summary>
/// 敌人状态
/// </summary>
public enum EnemyStateType
{
    Idle,Chase,Attack,Die
}
/// <summary>
/// 敌人参数，以后要用CharacterInfo派生类替代
/// </summary>
[Serializable]
public class Parameter
{
    public int health = 100;
    public float speed = 10f;
    public Transform target;
    public Animator animator;
    public NavMeshAgent agent;
    public float viewRange = 100f;
    public float attackRange = 5f;

}
public class FSM : MonoBehaviour
{
    public Parameter parameter;
/// <summary>当前状态/// </summary>
    private IState currentState;
    /// <summary>
    /// 敌人状态字典
    /// </summary>
    private Dictionary<EnemyStateType,IState> states = new Dictionary<EnemyStateType, IState>();


    void Start()
    {
        //添加状态
        states.Add(EnemyStateType.Idle, new IdleStates(this));
        states.Add(EnemyStateType.Chase, new ChaseStates(this));
        states.Add(EnemyStateType.Attack,new AttackStates(this));
        states.Add(EnemyStateType.Die, new DieStates(this));
        //寻找组件和主角
        parameter.animator = GetComponent<Animator>();
        parameter.agent = GetComponent<NavMeshAgent>();
        parameter.target = GameObject.FindWithTag("Player").transform;
        
        TransitionState(EnemyStateType.Idle);//设置初始状态

    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnUpdate();//执行对应状态的Update函数
    }
    /// <summary>
    /// 变更状态
    /// </summary>
    /// <param name="type">状态</param>
    public void TransitionState(EnemyStateType type)
    {
        if (currentState != null)
        {
            currentState.OnExit();//执行对应状态Exit函数
        }
        currentState = states[type];//字典查找状态
        currentState.OnEnter();//执行对应状态Enter函数
    }
/// <summary>
/// 根据目标位置改变朝向
/// </summary>
    public void FaceTo(Transform target)
    {
        if(target != null)
        {
            transform.LookAt(target);
        }
    }
}
