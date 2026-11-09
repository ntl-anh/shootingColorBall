using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupInfo : MonoBehaviour
{
    [SerializeField] Button btnClose;
    [SerializeField] RectTransform popupTransform; 
    private Vector3 hiddenPosition; 
    private Vector3 shownPosition; 

    private void Awake()
    {
        hiddenPosition = new Vector3(0, Screen.height, 0); 
        shownPosition = Vector3.zero; 
        popupTransform.anchoredPosition = hiddenPosition; 
    }

    public void Init()
    {
        btnClose.onClick.AddListener(Hide);
    }

    public void Show()
    {
        gameObject.SetActive(true); 
        transform.localPosition = Vector3.zero;

        StartCoroutine(SlideToPosition(shownPosition, 0.5f)); 
    }

    public void Hide()
    {
        StartCoroutine(SlideToPosition(hiddenPosition, 0.5f, () =>
        {
            gameObject.SetActive(false); 
        }));
    }

    private IEnumerator SlideToPosition(Vector3 targetPosition, float duration, System.Action onComplete = null)
    {
        Vector3 startPosition = popupTransform.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            popupTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        popupTransform.anchoredPosition = targetPosition;
        onComplete?.Invoke(); 
    }
}
