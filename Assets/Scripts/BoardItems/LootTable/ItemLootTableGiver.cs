using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

public class ItemLootTableGiver : Singleton<ItemLootTableGiver>, ISecondaryAction
{
    [SerializeField] private ItemLootTableData lootTable;
    [SerializeField] private BoardPlayer player;
    private Stack<BoardItem> givenItemsStack = new(); 
    
    public BoardItem GetRandomItem()
    {
        return lootTable.LootTable.Random().item; 
    }

    public void Execute()
    {
        var newItem = GetRandomItem(); 
        player.AddItem(newItem);    
        givenItemsStack.Push(newItem);
    }

    public void Undo()
    {
        player.RemoveItem(givenItemsStack.Pop());
    }
}