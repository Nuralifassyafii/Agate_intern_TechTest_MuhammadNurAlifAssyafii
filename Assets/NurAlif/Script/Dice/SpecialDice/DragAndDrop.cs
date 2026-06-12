using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentTransform;
    private Image baseImage;
    private GameObject thisGameObject;
    private DropManager _dropManager;
    [SerializeField] private MaterialSpecialDice _materialSpecialDice;
    private SpecialDiceUIManager _specialDiceUI;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GetMaterialSpecialDice().amount > 0)
        {
            Debug.Log("Begin drag");
            parentTransform = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            baseImage.raycastTarget = false;
        }
        else
        {
            _specialDiceUI.SetAlertText("Material tidak cukup");
        }
    }

    public bool CheckIsParentEmptyMaterial()
    {
        bool isEmpty = false;
        if (_dropManager.transform.childCount == 0 && !_dropManager.GetIsCraft())
        {
            isEmpty = true;
        }

        return isEmpty;
    }

    public void ChangeParentInstantiateMaterial()
    {
        if (CheckIsParentEmptyMaterial())
        {
            thisGameObject = Instantiate(gameObject);
            thisGameObject.transform.SetParent(_dropManager.transform);
        }
    }

    public MaterialSpecialDice GetMaterialSpecialDice()
    {
        return _materialSpecialDice;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GetMaterialSpecialDice().amount > 0)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag");
        if (GetMaterialSpecialDice().amount > 0)
        {
            transform.SetParent(parentTransform);
            baseImage.raycastTarget = true;
            GetMaterialSpecialDice().amount--;
            if (parentTransform != _dropManager.transform) ChangeParentInstantiateMaterial();
            _specialDiceUI.SetMaterialAmount(GetMaterialSpecialDice().id, GetMaterialSpecialDice().amount.ToString());
            _specialDiceUI.SetAlertText("");
        }
    }

    private void Start()
    {
        baseImage = GetComponent<Image>();
        _dropManager = transform.parent.GetComponent<DropManager>();
        _specialDiceUI = FindFirstObjectByType<SpecialDiceUIManager>();
        _materialSpecialDice = ScriptableObject.Instantiate(_materialSpecialDice);
        _specialDiceUI.SetMaterialAmount(GetMaterialSpecialDice().id, GetMaterialSpecialDice().amount.ToString());
    }
}
