using System;
using UnityEngine;

public class StairsGenerator : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int count;
    [SerializeField] private float height = 2.7f;

    public void Generate()
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(prefab, transform).transform.localPosition = new Vector3(0, i * height, 0);
        }
    }
}
