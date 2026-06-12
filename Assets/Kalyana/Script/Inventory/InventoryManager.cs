using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<string> ownedItemIDs = new();

    // add Item test
    // ===========================================
    void Start()
    {
        Debug.Log("InventoryManager is running");

        foreach (string itemID in ownedItemIDs)
        {
            AddItem(itemID);
        }
    }
    void Update()
    {
        if (Keyboard.current.iKey.wasPressedThisFrame) AddItem("Item0001");
        if (Keyboard.current.oKey.wasPressedThisFrame) RemoveItem("Item0001");
    }

    //=================================================================
    //=====================

    public void AddItem(string itemID)
    {
        if (!ownedItemIDs.Contains(itemID))
        {
            ownedItemIDs.Add(itemID);

            GlobalEventBus.Publish(new ItemAddedEvent(itemID));
        }
    }

    public void RemoveItem(string itemID)
    {
        if (ownedItemIDs.Contains(itemID))
        {
            ownedItemIDs.Remove(itemID);

            GlobalEventBus.Publish(new ItemRemovedEvent(itemID));
        }
    }

    public bool HasItem(string itemID)
    {
        return ownedItemIDs.Contains(itemID);
    }
}