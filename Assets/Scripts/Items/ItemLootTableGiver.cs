using UnityEngine;
using UnityUtils;

public class ItemLootTableGiver : MonoBehaviour
{
    [SerializeField] private ItemLootTableData lootTable;
    public BoardItem GetRandomItem()
    {
        return lootTable.LootTable.Random().item; 
    }
}