using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerMoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityAction pointerDownAction;
    public UnityAction pointerUpAction;

    private CancellationTokenSource clickCts;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Click");
        clickCts = new CancellationTokenSource();
        ClickActionAsync().Forget();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUpAction();
    }

    private async UniTask ClickActionAsync()
    {
        while (true)
        {
            pointerDownAction?.Invoke();
            await UniTask.Yield(PlayerLoopTiming.LastUpdate, cancellationToken: clickCts.Token);
        }
    }

    private void PointerUpAction()
    {
        Debug.Log("Up");
        clickCts?.Cancel();
        pointerUpAction?.Invoke();
    }

    public void AddDownListener(UnityAction action) => pointerDownAction += action;
    public void AddUpListener(UnityAction action) => pointerUpAction += action;
}
