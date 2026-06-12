using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class QuestNotificationUI : MonoBehaviour
{
    public static QuestNotificationUI Instance;

    private TMP_Text notificationText;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayDuration = 1.7f;
    [SerializeField] private float moveDistance = 50f;

    private Vector2 startPos;

    private Queue<string> notificationQueue = new(); 

    private bool isShowingNotification = false;

    private void Awake()
    {
        Instance = this;

        notificationText = GetComponent<TMP_Text>();

        startPos = notificationText.rectTransform.anchoredPosition;

        notificationText.alpha = 0;
    }

    public void ShowNotification(string message)
    {
        notificationQueue.Enqueue(message);

        if (!isShowingNotification)
        {
            ShowNextNotification();
        }
    }

    private void ShowNextNotification()
    {
        if (notificationQueue.Count == 0)
        {
            isShowingNotification = false;
            return;
        }

        isShowingNotification = true;

        string message = notificationQueue.Dequeue();

        notificationText.text = message;

        RectTransform rect = notificationText.rectTransform;

        rect.anchoredPosition = startPos - new Vector2(0, moveDistance);
        notificationText.alpha = 0;

        Sequence seq = DOTween.Sequence();

        seq.Append(notificationText.DOFade(1, fadeDuration));
        seq.Join(rect.DOAnchorPos(startPos, fadeDuration));

        seq.AppendInterval(displayDuration);

        seq.Append(notificationText.DOFade(0, fadeDuration));
        seq.Join(rect.DOAnchorPos(startPos + new Vector2(0, moveDistance), fadeDuration));

        seq.OnComplete(() => {ShowNextNotification();});
    }
}
