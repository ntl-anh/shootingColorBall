using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasNotice : MonoBehaviour
{
    public Button yesBtn;
    public Button noBtn;


   
    public void Init()
    {
        yesBtn.onClick.AddListener(ClickBtnYes);
        noBtn.onClick.AddListener(Hide);
    }

    void ClickBtnYes()
    {
        LevelProgression.ResetProgress();
        Hide();
    }

    public void Show()
    {
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
