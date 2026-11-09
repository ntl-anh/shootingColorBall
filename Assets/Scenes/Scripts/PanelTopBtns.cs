using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelTopBtns : MonoBehaviour
{
    [SerializeField] Button btnBack;
    [SerializeField] Button btnBackOne;
    [SerializeField] Button btnBackHome;
    [SerializeField] TextMeshProUGUI txtNumberBack;
    public static PanelTopBtns instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        btnBackOne.onClick.AddListener(OnBackOneButtonClicked);
        btnBack.onClick.AddListener(OnBackButtonClicked);
        btnBackHome.onClick.AddListener(ClickBtnBackHome);
    }

    void OnBackOneButtonClicked()
    {
        //if (GameManager.instance.MaxUndoCount > 0)
        //{
        //    SetTextValue(GameManager.instance.MaxUndoCount);
        //}
        //else
        //{
        //    Debug.Log("het ");
        //    SetTextValue(0);
        //}
        GameManager.instance.UndoLastAction();
    }

    void OnBackButtonClicked()
    {
        GameManager.instance.ResetLevel();  // Reset lại màn chơi
    }
    public void ClickBtnBackHome()
    {
        PlayerPrefs.Save();

        CalmGameUI.Load("MainMenu");
    }
    public void SetTextValue(int value)
    {
        txtNumberBack.text = value.ToString();
    }
    // Update is called once per frame
    void Update()
    {
    }
}
