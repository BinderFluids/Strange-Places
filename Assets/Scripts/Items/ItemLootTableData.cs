using System.Collections.Generic;
using UnityEngine;

public class ItemLootTableData : ScriptableObject
{
    [SerializeField] private List<ItemLootData> lootTable;
    public IReadOnlyList<ItemLootData> LootTable => lootTable;
}

public struct ItemLootData
{
    public BoardItem item;
    public float weight;
}