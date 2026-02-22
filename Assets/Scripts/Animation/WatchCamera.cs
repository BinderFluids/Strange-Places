using System;
using UnityEngine;

public class WatchCamera : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private Transform target;

    private void Update()
    {
        Vector3 direction = target.position - transform.position;

        Quaternion targetRot = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * speed
        );
    }
}
