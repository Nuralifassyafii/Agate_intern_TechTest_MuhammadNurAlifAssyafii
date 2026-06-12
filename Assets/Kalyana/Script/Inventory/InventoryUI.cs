using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Data")]
    public InventoryManager inventoryManager;
    public ItemDatabase itemDatabase;

    [Header("Icons")]
    public GameObject openButton;
    public GameObject closeButton;

    [Header("Panels")]
    public GameObject inventoryPanel;
    public GameObject itemDetailPanel; 

    [Header("Item List UI")]
    public Transform itemListContainer;
    public GameObject itemNamePrefab;

    [Header("Item Detail UI")]
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    void Start()
    {
        inventoryPanel.SetActive(false);
        itemDetailPanel.SetActive(false);

        openButton.SetActive(true);
        closeButton.SetActive(false);
    }

    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        itemDetailPanel.SetActive(false);

        openButton.SetActive(false);
        closeButton.SetActive(true);

        RefreshItemList();
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        itemDetailPanel.SetActive(false);

        openButton.SetActive(true);
        closeButton.SetActive(false);
    }

    void RefreshItemList()
    {
        foreach (Transform child in itemListContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (string itemID in inventoryManager.ownedItemIDs)
        {
            ItemData item = itemDatabase.GetItem(itemID);

            if (item == null) continue;

            GameObject obj = Instantiate(itemNamePrefab, itemListContainer);

            TMP_Text text = obj.GetComponentInChildren<TMP_Text>();
            text.text = item.itemName;

            Button btn = obj.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => ShowItemDetail(item));
        }
    }

    public void ShowItemDetail(ItemData item)
    {
        itemDetailPanel.SetActive(true);

        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.itemDescription;
    }

}
