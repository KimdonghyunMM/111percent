using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerMoveButton leftBtn, rightBtn;
    [SerializeField] private BoxCollider2D floor;
    [SerializeField] private SkillButton[] skillBtns;

    private void Start()
    {
        leftBtn.AddDownListener(LeftMove);
        rightBtn.AddDownListener(RightMove);

        leftBtn.AddUpListener(() => player.ChangeState(STATE.ATTACK));
        rightBtn.AddUpListener(() => player.ChangeState(STATE.ATTACK));

        for (var i = 0; i < player.equippedSkills.Length; i++)
        {
            skillBtns[i].SetSkillData(player.equippedSkills[i], player);
        }
    }

    private void LeftMove()
    {
        if (player.currentState == STATE.SKILL)
            return;

        player.ChangeState(STATE.MOVE);
        player.LeftMove();
    }

    private void RightMove()
    {
        if (player.currentState == STATE.SKILL)
            return;

        player.ChangeState(STATE.MOVE);
        player.RightMove();
    }
}
