using UnityEngine;

public class Player_Hp : MonoBehaviour, IEventListener
{
    private void Start()
    {
        GameEventHandler.AddListener(GameEventType.HP_REFRESH, this);
    }

    public void OnEvent(GameEventType eType, Component sender, object param = null)
    {
        switch (eType)
        {
            case GameEventType.HP_REFRESH:
                Debug.Log("체력 깎는 이벤트 발송 완료");
                break;
        }
    }
}
