using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropManager : MonoBehaviour, IDropHandler
{
    [SerializeField] private bool isCraftSlot;
    [SerializeField] private int slotNumber;
    [SerializeField] private MaterialSpecialDice _pickedMaterial;
    private SpecialDiceMaker _specialDiceMaker;
    private SpecialDiceUIManager _specialDiceUI;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DragAndDrop dragItem = dropped.GetComponent<DragAndDrop>();
        if (transform.childCount == 0 && isCraftSlot && dragItem.GetMaterialSpecialDice().amount > 0)
        {
            dragItem.parentTransform = transform;
            _pickedMaterial = dragItem.GetMaterialSpecialDice();
            _specialDiceMaker.AddListMaterialSpecialDice(this);
        }
        else
        {
            _specialDiceUI.SetAlertText("Material tidak cukup");
        }
    }

    public int getTransformChildCount()
    {
        return transform.childCount;
    }

    public bool GetIsCraft()
    {
        return isCraftSlot;
    }

    public int GetSlotNumber()
    {
        return slotNumber;
    }

    public MaterialSpecialDice GetPickedMaterial()
    {
        return _pickedMaterial;
    }

    public void DestroyMaterial()
    {
        if (transform.childCount == 1)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        else
        {
            //ui alert
            _specialDiceUI.SetAlertText("ada kesalahan saat menghancurkan material, child lebih dari 1");
        }
    }

    private void Start()
    {
        _specialDiceMaker = FindObjectOfType<SpecialDiceMaker>();
        _specialDiceUI = FindFirstObjectByType<SpecialDiceUIManager>();
    }
}
