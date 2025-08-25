using UnityEngine;
using Spine;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using UnityEngine.Events;

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
    [SerializeField] private BoxCollider2D myFloor;

    protected float unitSpeed;
    protected ReactiveProperty<STATE> stateProperty = new ReactiveProperty<STATE>();
    protected Rigidbody2D rigidBody;
    protected float minX;
    protected float maxX;
    protected CancellationTokenSource stateCts, buffCts;
    protected Pooling_Skill useSkill;

    public PoolingSkillData[] equippedSkills = new PoolingSkillData[5];
    public COMMON.UserData data;
    public UnityAction leftLimitAction, rightLimitAction;
    public STATE currentState => stateProperty.Value;
    public Transform muzzle;
    public Transform hitPoint;

    private bool isDuration = false;

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
        spineAnim.AnimationState.Complete += TrackEntryDelegateMethod;

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

    protected virtual async UniTask AutoSkill()
    {
        var coolTimeArr = new float[equippedSkills.Length];
        for (var i = 0; i < coolTimeArr.Length; i++)
            coolTimeArr[i] = equippedSkills[i].coolTime;

        while (true)
        {
            for (var i = 0; i < equippedSkills.Length; i++)
            {
                if (equippedSkills[i].name == string.Empty)
                    continue;

                coolTimeArr[i] += Time.deltaTime;

                if (isDuration)
                    break;

                if (coolTimeArr[i] >= equippedSkills[i].coolTime)
                {
                    SkillDuration();

                    var skill = COMMON.GetSkill(equippedSkills[i].name);
                    skill.Init(equippedSkills[i]);
                    SetUseSkill(skill);
                    ChangeState(STATE.SKILL);

                    coolTimeArr[i] = 0f;
                }
            }

            await UniTask.Delay(0);
        }
    }

    private void SkillDuration() => SkillDurationAsync().Forget();

    private async UniTask SkillDurationAsync()
    {
        isDuration = true;
        var timeCount = 0f;
        while (timeCount <= 1f)
        {
            timeCount += Time.deltaTime;
            await UniTask.Delay(0);
        }
        isDuration = false;
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
        spineAnim.AnimationState.SetAnimation(0, "shield_attack", false);
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
            var proj = COMMON.GetProjectile("Sword");
            proj.Init(GetProjData);
            proj.rigidBody.position = muzzle.position;
            proj.ThrowProjectile(GetTargetPos());
        }
        else if (e.Data.Name == "shield_attack")
        {
            useSkill.UseSkill(this).Forget();
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

    public void GetDamage(float value, Define.DamageType damageType = Define.DamageType.Damage) => GetDamage((int)value, damageType);

    private void HPChangeText(Define.DamageType type, int value)
    {
        var key = "DamageText";
        var textObj = ObjectPool.instance.GetPoolingItem(key) as Pooling_Text;
        var textData = new PoolingTextData();
        textData.name = key;
        textData.damageType = type;
        textData.value = value;
        textObj.Init(textData);
        textObj.transform.position = rigidBody.position + Vector2.up;
        textObj.Play();
    }

    public async UniTask DOBuff(Buff buff)
    {
        if (buff.percent == 0)
            return;

        //buffCts = new CancellationTokenSource();

        var rnd = Random.Range(0f, 1f);
        if (buff.percent > rnd)
            return;

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

            await UniTask.Delay(0/*, cancellationToken: buffCts.Token*/);
        }
    }

    public void EquipSkill(int index, string skillName)
    {
        equippedSkills[index] = COMMON.GetSkillData(skillName);
    }

    public Vector2 GetTargetPos() => target.hitPoint.position;
    public void SetUseSkill(Pooling_Skill skill) => useSkill = skill;
    public Define.UnitType GetUnitType() => data.unitType;
    protected void SetRightLimitMoveAction(UnityAction act) => rightLimitAction = act;
    protected void SetLeftLimitMoveAction(UnityAction act) => leftLimitAction = act;
    protected virtual void TrackEntryDelegateMethod(TrackEntry entry)
    {
        if (stateProperty.Value == STATE.SKILL && entry.Animation.Name == "shield_attack")
            ChangeState(STATE.ATTACK);
    }
}
