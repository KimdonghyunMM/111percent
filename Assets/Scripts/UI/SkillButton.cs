using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IEventListener
{
    [HideInInspector] public PoolingSkillData skillData;

    [SerializeField] private Button skillBtn;
    [SerializeField] private Image coolTimeImage, durationTimeImage;
    [SerializeField] private Text skillName;

    private Player player;

    private void Awake()
    {
        GameEventHandler.AddListener(GameEventType.USE_SKILL, this);
    }

    private void Start()
    {
        skillBtn.onClick.AddListener(UseSkill);
    }
    private void UseSkill()
    {
        if (skillData.name == string.Empty)
            return;

        CoolTimeAsync().Forget();
        var skill = COMMON.GetSkill(skillData.name);
        skill.Init(skillData);
        player.SetUseSkill(skill);
        player.ChangeState(STATE.SKILL);
        GameEventHandler.PostNotification(GameEventType.USE_SKILL, this);
    }
    public void SetSkillData(PoolingSkillData skillData, Player player)
    {
        this.skillData = skillData;
        this.player = player;
        skillName.text = skillData.name;
    }
    private async UniTask CoolTimeAsync()
    {
        coolTimeImage.gameObject.SetActive(true);
        coolTimeImage.fillAmount = 1;
        var coolTime = skillData.coolTime;
        while (coolTimeImage.fillAmount > 0)
        {
            coolTimeImage.fillAmount -= Time.deltaTime / coolTime;
            await UniTask.Delay(0);
        }
        coolTimeImage.gameObject.SetActive(false);
    }

    public async UniTask DurationTimeAsync()
    {
        durationTimeImage.gameObject.SetActive(true);
        durationTimeImage.fillAmount = 1;
        while (durationTimeImage.fillAmount > 0)
        {
            durationTimeImage.fillAmount -= Time.deltaTime / 2f;
            await UniTask.Delay(0);
        }
        durationTimeImage.gameObject.SetActive(false);
    }

    public void OnEvent(GameEventType eType, Component sender, params object[] args)
    {
        switch (eType)
        {
            case GameEventType.USE_SKILL:
                DurationTimeAsync().Forget();
                break;
        }
    }
}
