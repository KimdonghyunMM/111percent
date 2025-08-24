using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerMoveButton leftBtn, rightBtn;
    [SerializeField] private BoxCollider2D floor;

    private void Start()
    {
        leftBtn.AddDownListener(LeftMove);
        rightBtn.AddDownListener(RightMove);

        leftBtn.AddUpListener(() => player.ChangeState(STATE.ATTACK));
        rightBtn.AddUpListener(() => player.ChangeState(STATE.ATTACK));
    }

    private void LeftMove()
    {
        player.ChangeState(STATE.MOVE);
        player.LeftMove();
    }

    private void RightMove()
    {
        player.ChangeState(STATE.MOVE);
        player.RightMove();
    }
}
