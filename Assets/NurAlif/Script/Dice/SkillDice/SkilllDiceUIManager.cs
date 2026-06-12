using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkilllDiceUIManager : MonoBehaviour
{
    [SerializeField] private SkillDiceManager _skillDiceManager;
    private SkillDiceManagerPerDice _skillDiceManagerDice;
    [SerializeField] private CostUI _costUI;
    [SerializeField] private GameObject skillDiceMenu;
    [SerializeField] private Sprite upgradeCountdownSprite;
    [SerializeField] private Sprite upgradeDoneSprite;
    [SerializeField] private Sprite failedUpgradeSprite; //nanti diganti (masih belum)
    [SerializeField] private GameObject detailStats;
    [SerializeField] private GameObject notificationObj;
    private float showNotificationTime = 2f;
    public void HoverExpAmount(int faceDiceIndex)
    {
        _costUI.CostUIObject.SetActive(true);
        ShowDiceName(_skillDiceManagerDice.GetPickedDiceValue().skillType.ToString());
        if (faceDiceIndex < 6)
        {
            _costUI.CostUIText.text = "( - " + _skillDiceManagerDice.GetCost(faceDiceIndex).ToString() + " )";
        }
        else
        {
            _costUI.CostUIText.text = "( - " + _skillDiceManagerDice.GetTotalCost(_skillDiceManagerDice.GetPickedDiceValue()).ToString() + " )";
        }
    }

    public void SetViewSkillDiceCanvas(bool isActive)
    {
        skillDiceMenu.SetActive(isActive);
    }

    public void OutHoverExpAmount()
    {
        _costUI.CostUIObject.SetActive(false);
    }

    public void SetDiceIndex(int diceIndex)
    {
        _skillDiceManagerDice = _skillDiceManager._managerPerDice[diceIndex];
    }

    public void ShowDetailStats(bool isShow)
    {
        if (detailStats == null || skillDiceMenu == null) return;
        detailStats.SetActive(isShow);
        ShowCountDown(0.001f, 1);
        SetSkillDiceAlert("Long Press To Upgrade Dice");
    }

    public void SetSkillDiceAlert(string text)
    {
        TMP_Text alertText = detailStats.transform.Find("AlertText").gameObject.GetComponent<TMP_Text>();
        alertText.text = text;
    }

    public void ShowCountDown(float currentTimer, float maxTimer)
    {
        Image countDownImage = detailStats.transform.Find("CountDownBar").gameObject.GetComponent<Image>();
        countDownImage.sprite = currentTimer == 0 ? upgradeDoneSprite : upgradeCountdownSprite;
        countDownImage.type = currentTimer == 0 ? Image.Type.Simple : Image.Type.Filled;
        countDownImage.fillAmount = countDownImage.type == Image.Type.Filled ? 1 - currentTimer / maxTimer * 1f : 0;
    }

    public void SetCountdownImageFailed() //sementara, nanti diganti (masih belum)
    {
        detailStats.transform.Find("CountDownBar").gameObject.GetComponent<Image>().sprite = failedUpgradeSprite;
    }

    public IEnumerator ShowNotification(string text)
    {
        notificationObj.GetComponent<TMP_Text>().text = text;
        notificationObj.SetActive(true);

        yield return new WaitForSeconds(2f);

        notificationObj.SetActive(false);
    }

    public void DeactivateNotif() //nanti diganti biar nggk ngebug notifnya
    {
        notificationObj.SetActive(false);
    }

    public void ShowDiceName(string text)
    {
        detailStats.transform.Find("DiceFaceName").gameObject.GetComponent<TMP_Text>().text = text;
    }

    private void Start()
    {
        _skillDiceManager = FindFirstObjectByType<SkillDiceManager>();
        notificationObj.GetComponent<TMP_Text>().text = " ";
    }
}
