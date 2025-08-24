using UnityEngine;
using Spine;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Events;
using Unity.VisualScripting;
using System.Collections.Generic;

public enum STATE
{
    IDLE,   //플레이 버튼 누르기 전에만 동작함
    ATTACK,
    MOVE,
    SKILL,
    DEAD,
}

public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected SkeletonAnimation spineAnim;
    [SerializeField] protected Unit target;
    [SerializeField] private Transform muzzle;
    [SerializeField] private BoxCollider2D myFloor;

    protected COMMON.UserData data;
    protected float unitSpeed;
    protected ReactiveProperty<STATE> stateProperty = new ReactiveProperty<STATE>();
    protected Rigidbody2D rigidBody;
    protected float minX;
    protected float maxX;
    protected CancellationTokenSource stateCts, buffCts;
    protected List<Buff> buffList = new List<Buff>();

    public UnityAction leftLimitAction, rightLimitAction;
    public STATE currentState => stateProperty.Value;
    public Transform hitPoint;

    protected virtual void Awake()
    {
        unitSpeed = 3f;
        minX = myFloor.transform.position.x - myFloor.bounds.size.x * 0.5f;
        maxX = myFloor.transform.position.x + myFloor.bounds.size.x * 0.5f;

        rigidBody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        stateProperty.TakeUntilDestroy(this).Subscribe(StateChangeCallBack);
        spineAnim.state.Event += AttackEvent;

        GameEventHandler.PostNotification(GameEventType.HP_INIT, this, GetUnitType(), data.unitData.hp);
    }

    public void ChangeState(STATE state)
    {
        if (currentState == STATE.DEAD)
            return;

        ForcedChangeState(state);
    }

    public void ForcedChangeState(STATE state) => stateProperty.Value = state;
    private void StateChangeCallBack(STATE state) => ChangeStateAsync(state).Forget();

    private async UniTask ChangeStateAsync(STATE state)
    {
        stateCts?.Cancel();
        stateCts = new CancellationTokenSource();
        switch (state)
        {
            case STATE.IDLE:
                await IdleAsync();
                break;
            case STATE.ATTACK:
                await AttackAsync();
                break;
            case STATE.MOVE:
                await MoveAsync();
                break;
            case STATE.SKILL:
                await SkillAsync();
                break;
            case STATE.DEAD:
                await DeadAsync();
                break;
        }
    }

    protected virtual async UniTask IdleAsync()
    {
        spineAnim.AnimationState.SetAnimation(0, "idle_3", true);
    }
    protected virtual async UniTask AttackAsync()
    {
        spineAnim.AnimationState.SetAnimation(0, "sword_attack", true);
        if (target.currentState == STATE.DEAD)
            ChangeState(STATE.IDLE);
    }
    protected virtual async UniTask MoveAsync()
    {
        spineAnim.AnimationState.SetAnimation(0, "run_shield", true);
        if (target.currentState == STATE.DEAD)
            ChangeState(STATE.IDLE);
    }
    protected virtual async UniTask SkillAsync()
    {
        if (target.currentState == STATE.DEAD)
            ChangeState(STATE.IDLE);
    }
    protected virtual async UniTask DeadAsync()
    {
        spineAnim.AnimationState.SetAnimation(0, "dead", false);
    }

    private void AttackEvent(TrackEntry track, Spine.Event e)
    {
        if (e.Data.Name == "sword_attack")
        {
            var proj = ObjectPool.instance.GetPoolingItem("Sword") as Pooling_Projectile;
            proj.Init(GetProjData);
            proj.rigidBody.position = muzzle.position;
            proj.ThrowProjectile(target.hitPoint.position);
        }
    }

    private PoolingProjectileData GetProjData()
    {
        var projData = new PoolingProjectileData();
        projData.name = "Sword";
        projData.speed = 6f;
        projData.damage = data.unitData.atk;
        projData.unitType = data.unitType;
        return projData;
    }

    public void RightMove()
    {
        if (currentState == STATE.DEAD)
            return;

        transform.localScale = Vector3.one;
        rigidBody.position += Vector2.right * unitSpeed * Time.deltaTime;
        if (transform.position.x >= maxX)
            rightLimitAction?.Invoke();
    }

    public void LeftMove()
    {
        if (currentState == STATE.DEAD)
            return;

        transform.localScale = new Vector3(-1, 1, 1);
        rigidBody.position += Vector2.left * unitSpeed * Time.deltaTime;
        if (transform.position.x <= minX)
            leftLimitAction?.Invoke();
    }

    public void GetDamage(int value, Define.DamageType damageType = Define.DamageType.Damage)
    {
        switch (damageType)
        {
            case Define.DamageType.Damage:
                data.unitData.hp -= value;
                break;
            case Define.DamageType.Heal:
                data.unitData.hp += value;
                break;
        }

        if (data.unitData.hp <= 0)
            ChangeState(STATE.DEAD);

        HPChangeText(damageType, value);

        GameEventHandler.PostNotification(GameEventType.HP_REFRESH, this, GetUnitType(), data.unitData.hp);
    }

    private void HPChangeText(Define.DamageType type, int value)
    {
        var textObj = ObjectPool.instance.GetPoolingItem("DamageText") as Pooling_Text;
        var textData = new PoolingTextData();
        textData.damageType = type;
        textData.value = value;
        textObj.Init(textData);
        textObj.transform.position = rigidBody.position + Vector2.up;
        textObj.Play();
    }

    private async UniTask DOBuff(Buff buff)
    {
        buffCts = new CancellationTokenSource();

        var timeCount = 0f;
        var termCount = 0f;
        while (timeCount <= buff.duration)
        {
            var time = Time.deltaTime;
            timeCount += time;
            switch (buff.buffType)
            {
                case Define.BuffType.DotHeal:
                case Define.BuffType.DotDamage:
                    {
                        termCount += time;
                        if (termCount >= buff.term)
                        {
                            var value = Mathf.RoundToInt(data.unitData.atk * buff.incValue);
                            if (buff.buffType == Define.BuffType.DotHeal)
                                GetDamage(value, Define.DamageType.Heal);
                            else
                                GetDamage(value);

                            termCount = 0f;
                        }
                        break;
                    }
                case Define.BuffType.Buff:
                    break;
                case Define.BuffType.DeBuff:
                    break;
            }

            await UniTask.Delay(0, cancellationToken: buffCts.Token);
        }
    }

    public void EquipSkill(int index, string skillName)
    {

    }

    public Define.UnitType GetUnitType() => data.unitType;
    protected void SetRightLimitMoveAction(UnityAction act) => rightLimitAction = act;
    protected void SetLeftLimitMoveAction(UnityAction act) => leftLimitAction = act;
}
