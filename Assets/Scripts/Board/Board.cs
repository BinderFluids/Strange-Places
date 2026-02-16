using System;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Min(1), SerializeField] private int gridWidth;
    [Min(1), SerializeField] private int gridHeight; 
    
    private Grid<BoardNode> grid;
    public Grid<BoardNode> Grid => grid;

    private void Awake()
    {
        grid = new Grid<BoardNode>(gridWidth, gridHeight);
    }
}