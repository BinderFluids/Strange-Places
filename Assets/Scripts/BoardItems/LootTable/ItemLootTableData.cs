using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create ItemLootTableData", fileName = "ItemLootTableData", order = 0)]
public class ItemLootTableData : ScriptableObject
{
    [SerializeField] private List<ItemLootData> lootTable;
    public IReadOnlyList<ItemLootData> LootTable => lootTable;
}

[System.Serializable]
public struct ItemLootData
{
    public BoardItem item;
    public float weight;
}