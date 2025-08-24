using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : Unit
{
    protected override void Start()
    {
        base.Start();
        data = new COMMON.UserData(Define.UnitType.Player);

        SetLeftLimitMoveAction(() => rigidBody.position = Vector2.right * minX);
        SetRightLimitMoveAction(() => rigidBody.position = Vector2.right * maxX);
    }

    protected override async UniTask AttackAsync()
    {
        await base.AttackAsync();

        transform.localScale = Vector3.one;
    }
}
