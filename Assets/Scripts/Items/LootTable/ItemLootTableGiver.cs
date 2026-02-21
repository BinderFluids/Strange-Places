using System.Collections.Generic;
using UnityEngine;
using UnityUtils;

public class ItemLootTableGiver : Singleton<ItemLootTableGiver>, ISecondaryAction
{
    [SerializeField] private Transform itemAnchor; 
    [SerializeField] private ItemLootTableData lootTable;
    [SerializeField] private BoardPlayer player;
    private Stack<BoardItem> givenItemsStack = new(); 
    
    public BoardItem GetRandomItem()
    {
        return lootTable.LootTable.Random().item; 
    }

    public void Execute()
    {
        BoardItem newItem = Instantiate(GetRandomItem(), itemAnchor); 
        newItem.gameObject.SetActive(false); 
        
        player.AddItem(newItem);    
        givenItemsStack.Push(newItem);
    }

    public void Undo()
    {
        if (givenItemsStack.Count == 0) return;
        BoardItem item = givenItemsStack.Pop();
        player.RemoveItem(item);
        Destroy(item.gameObject);
    }
}