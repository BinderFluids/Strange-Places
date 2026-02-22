using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class LookAtCenterOfScreen : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float stepForward;
    [SerializeField] private float followSpeed; 
    void Update()
    {
        // Create a ray from the center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Pick a point some distance forward (e.g., 100 units)
        Vector3 targetPoint = ray.GetPoint(stepForward);

        Vector3 lerpPoint = Vector3.Lerp(transform.position, targetPoint, followSpeed * Time.deltaTime);
        transform.LookAt(lerpPoint);
    }
}
