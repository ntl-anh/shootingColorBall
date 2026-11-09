using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class InGameUi : MonoBehaviour
{

    public static InGameUi instance;
    public TextMeshProUGUI levelNumber;

    public GameObject levelCompletePannel;
    public GameObject levelCompleteParticle;
    public GameObject TopPanel;
    public Texture[] bgs;
    public Mesh[] newModels;
    GameObject ground;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }
    private void Start()
    {
        levelNumber.text = "LEVEL " + SceneManager.GetActiveScene().name;
        var preview = GameObject.Find("Main CameraPrivew");
        if (preview && preview.TryGetComponent<AudioListener>(out var listener)) listener.enabled = false;

        ground = GameObject.Find("Ground");
        var legacyPreview = GameObject.Find("new preview_ui!!");
        if (legacyPreview) legacyPreview.SetActive(false);
    }
    public void OnNextButtonPress()
    {
        LevelProgression.Next();
    }
    public void OnRetryButtonPress()
    {
        if (GameManager.instance) GameManager.instance.ResetLevel();
    }
    public void LevelCompleted()
    {
        if (CalmGameUI.Instance) CalmGameUI.Instance.ShowCompleted();
        else levelCompletePannel.SetActive(true);
        if (TopPanel) TopPanel.SetActive(false);
    }
}
