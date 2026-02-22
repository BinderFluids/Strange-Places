using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using ScriptableVariables;
using UnityEngine;
using UnityEngine.Splines;

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
    [SerializeField] private IntVariable points; 
    [SerializeField] private float moveDuration;
    [SerializeField] private SplineContainer spline;

    [SerializeField] private Transform _transform;
    private Tween movementTween;
    [SerializeField] private float currentProgress = .5f; 
    
    public event Action onMoveComplete;
    
    private void Awake()
    {
        movementCharge.Value = 1;
        _transform ??= GetComponent<Transform>();
        
        EvaluateSpline(currentProgress);
    }

    private void Update()
    {
# if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
        {
            queuedMove.Value = 1;
            Move().Forget();
        } if (Input.GetKeyDown(KeyCode.L))
        {
            queuedMove.Value = -1;
            Move().Forget();
        }
#endif
    }

    public async UniTask Move()
    {
        int moveDir = queuedMove.Value > 0 ? 1 : -1;
        playerCamContainer.SetActive(false);
        
        machineMovingForwardCamera.SetActive(moveDir == 1);
        machineMovingBackwardCamera.SetActive(moveDir == -1);
        machineMoveCameraContainer.SetActive(true); 
        
        int projectedMove = Mathf.Abs(queuedMove.Value * movementCharge.Value);
        int pointsToWin = 5 - Mathf.Abs(points.Value);
        int actualMove = Mathf.Min(projectedMove, pointsToWin);
        
        for (int i = 0; i < actualMove; i++)
        {
            float newProgress = Mathf.InverseLerp(-5, 5, points.Value + moveDir);
            movementTween =  Tween.Custom(currentProgress, newProgress, moveDuration, onValueChange: t => {
                EvaluateSpline(t);
            });
            await movementTween;
            await UniTask.WaitForSeconds(1f);


            points.Value += moveDir; 
        }
        
        playerCamContainer.SetActive(true); 
        machineMoveCameraContainer.SetActive(false);
        queuedMove.Value = 0;
        movementCharge.Value = 1; 
    }

    void EvaluateSpline(float progress)
    {
        // Update position based on the 0-1 interpolation
        transform.position = spline.EvaluatePosition(progress);
        // Optionally update rotation to face the spline direction
        var look = Quaternion.LookRotation(spline.EvaluateTangent(progress));
        look.x = 0;
        look.z = 0;
        transform.rotation = look; 
        currentProgress = progress;
    }

    void OnMoveComplete()
    {
        onMoveComplete?.Invoke();
    }
}
