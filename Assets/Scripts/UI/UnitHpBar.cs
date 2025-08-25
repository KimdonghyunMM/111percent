using UnityEngine;
using UnityEngine.UI;

public class UnitHpBar : MonoBehaviour, IEventListener
{
    [SerializeField] private Define.UnitType unitType;

    private Slider hpBar;

    private void Awake()
    {
        hpBar = GetComponent<Slider>();
        GameEventHandler.AddListener(GameEventType.HP_REFRESH, this);
        GameEventHandler.AddListener(GameEventType.HP_INIT, this);
    }

    public void OnEvent(GameEventType eType, Component sender, params object[] args)
    {
        switch (eType)
        {
            case GameEventType.HP_REFRESH:
                {
                    var type = (Define.UnitType)args[0];
                    if (unitType != type)
                        break;

                    hpBar.value = (int)args[1];
                    break;
                }

            case GameEventType.HP_INIT:
                {
                    var type = (Define.UnitType)args[0];
                    if (unitType != type)
                        break;

                    hpBar.minValue = 0;
                    hpBar.maxValue = (int)args[1];
                    break;
                }
        }
    }
}
