using UnityEngine;
using Spine;
using UniRx;
using System.Threading;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using static UnityEngine.GraphicsBuffer;
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
    [SerializeField] protected Transform target;
    [SerializeField] private Transform muzzle;
    [SerializeField] private BoxCollider2D myFloor;

    protected float unitSpeed;
    protected ReactiveProperty<STATE> stateProperty = new ReactiveProperty<STATE>();
    protected Rigidbody2D rigidBody;
    protected float minX;
    protected float maxX;
    protected CancellationTokenSource stateCts;

    public UnityAction leftLimitAction, rightLimitAction;

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
    }

    public void ChangeState(STATE state)
    {
        stateProperty.Value = state;
    }

    private void StateChangeCallBack(STATE state)
    {
        ChangeStateAsync(state).Forget();
    }

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
    }
    protected virtual async UniTask MoveAsync()
    {
        spineAnim.AnimationState.SetAnimation(0, "run_shield", true);
    }
    protected virtual async UniTask SkillAsync()
    {

    }
    protected virtual async UniTask DeadAsync()
    {
        spineAnim.AnimationState.SetAnimation(0, "dead", true);
    }

    private void AttackEvent(TrackEntry track, Spine.Event e)
    {
        if (e.Data.Name == "sword_attack")
        {
            var proj = ObjectPool.instance.GetPoolingItem("Sword") as Pooling_Projectile;
            proj.Init(GetProjData);
            proj.transform.position = muzzle.position;
            proj.ThrowProjectile(target.position);
        }
    }

    private PoolingProjectileData GetProjData()
    {
        var projData = new PoolingProjectileData();
        projData.name = "Sword";
        projData.speed = 6f;
        projData.damage = this is Player ? COMMON.unitSo.playerData.atk : COMMON.unitSo.enemyData.atk;
        return projData;
    }

    public void RightMove()
    {
        transform.localScale = Vector3.one;
        rigidBody.position += Vector2.right * unitSpeed * Time.deltaTime;
        if (transform.position.x >= maxX)
            rightLimitAction?.Invoke();
    }

    public void LeftMove()
    {
        transform.localScale = new Vector3(-1, 1, 1);
        rigidBody.position += Vector2.left * unitSpeed * Time.deltaTime;
        if (transform.position.x <= minX)
            leftLimitAction?.Invoke();
    }

    protected void SetRightLimitMoveAction(UnityAction act) => rightLimitAction = act;
    protected void SetLeftLimitMoveAction(UnityAction act) => leftLimitAction = act;
}
