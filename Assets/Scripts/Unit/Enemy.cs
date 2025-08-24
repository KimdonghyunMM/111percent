using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : Unit
{
    private UnityAction moveAct;

    protected override void Start()
    {
        base.Start();
        data = new COMMON.UserData(Define.UnitType.Enemy);

        spineAnim.AnimationState.Complete += delegate
        {
            var rnd = Random.Range(0f, 1f);
            if (stateProperty.Value == STATE.ATTACK)
            {
                if (rnd >= 0.5f)
                    ChangeState(STATE.MOVE);
            }
        };

        SetLeftLimitMoveAction(() => moveAct = RightMove);
        SetRightLimitMoveAction(() => moveAct = LeftMove);

        moveAct = RightMove;
        ChangeState(STATE.MOVE);
    }

    protected override async UniTask MoveAsync()
    {
        await base.MoveAsync();
        var count = 0f;
        var rnd = RandomRatio();
        while (true)
        {
            count += Time.deltaTime;
            moveAct?.Invoke();
            if (count >= rnd)
            {
                count = 0;
                ChangeState(STATE.ATTACK);
                transform.localScale = new Vector3(-1, 1, 1);
                break;
            }
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken: stateCts.Token);
        }
    }

    private float RandomRatio() => Random.Range(0.5f, 1f);
}
