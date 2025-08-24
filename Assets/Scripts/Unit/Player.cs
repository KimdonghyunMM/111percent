using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Player : Unit
{
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            var proj = ObjectPool.instance.GetPoolingItem("Sword") as Pooling_Projectile;
            proj.transform.position = rigidBody.position;
            proj.ThrowProjectile(target.position);
            *//*proj.transform.DOJump(target.position, 5f, 1, 10f).SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    //var nextPos =    
                })
                .OnComplete(() =>
                {
                    proj.Release();
                });*//*
        }
    }*/

    protected override void Start()
    {
        base.Start();
        SetLeftLimitMoveAction(() => rigidBody.position = Vector2.right * minX);
        SetRightLimitMoveAction(() => rigidBody.position = Vector2.right * maxX);
    }

    protected override async UniTask AttackAsync()
    {
        await base.AttackAsync();

        transform.localScale = Vector3.one;
    }
}
