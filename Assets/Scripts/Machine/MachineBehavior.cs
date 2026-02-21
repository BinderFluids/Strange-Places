using System;
using Cysharp.Threading.Tasks;
using EventBus;
using PrimeTween;
using ScriptableVariables;
using UnityEditor.Callbacks;
using UnityEngine;

public class MachineBehavior : MonoBehaviour
{
    [SerializeField] private GameObject playerCamContainer;
    [SerializeField] private GameObject machineMoveCameraContainer;
    [SerializeField] private GameObject machineMovingForwardCamera;
    [SerializeField] private GameObject machineMovingBackwardCamera;
    [SerializeField] private BoardActor actor; 
    [SerializeField] private BoardActor opponent;
    [SerializeField] private IntVariable queuedMove;
    [SerializeField] private IntVariable movementCharge;
    [SerializeField] private float moveDistance;
    [SerializeField] private float moveDuration;

    [SerializeField] private Transform _transform;
    private Tween movementTween;

    public event Action onMoveComplete;
    
    private void Awake()
    {
        movementCharge.Value = 1;
        
        _transform ??= GetComponent<Transform>();
    }
    
    public async UniTask Move()
    {
        int moveDir = queuedMove.Value > 0 ? 1 : -1;
        playerCamContainer.SetActive(false);
        
        machineMovingForwardCamera.SetActive(moveDir == 1);
        machineMovingBackwardCamera.SetActive(moveDir == -1);
        machineMoveCameraContainer.SetActive(true); 
        
        for (int i = 0; i < Mathf.Abs(queuedMove.Value); i++)
        {
            float moveVector = moveDistance * movementCharge.Value * moveDir;
            movementTween = Tween
                .PositionX(_transform, _transform.position.x + moveVector, moveDuration * movementCharge.Value)
                .OnComplete(OnMoveComplete);
            
            await movementTween;
            await UniTask.WaitForSeconds(1f);
        }
        
        playerCamContainer.SetActive(true); 
        machineMoveCameraContainer.SetActive(false);
        queuedMove.Value = 0;
        movementCharge.Value = 1; 
    }

    void OnMoveComplete()
    {
        onMoveComplete?.Invoke();
    }
}
