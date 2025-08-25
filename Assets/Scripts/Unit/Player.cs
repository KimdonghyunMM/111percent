using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : Unit
{
    protected override void Start()
    {
        data = new COMMON.UserData(Define.UnitType.Player);
        base.Start();

        SetLeftLimitMoveAction(() => rigidBody.position = Vector2.right * minX);
        SetRightLimitMoveAction(() => rigidBody.position = Vector2.right * maxX);

        EquipSkill(0, "FireShot");
        EquipSkill(1, "RainArrow");
        EquipSkill(2, "Heal");
    }

    protected override async UniTask AttackAsync()
    {
        await base.AttackAsync();

        transform.localScale = Vector3.one;
    }
}
