using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button btnStart;
    [SerializeField] Button btnMenu;
    [SerializeField] GameObject panelMenu;
    [SerializeField] Button btnMusic;
    [SerializeField] Button btnInfo;
    [SerializeField] Button btnReset;
    [SerializeField] Button btnBigClose;
    [SerializeField] Image btnOffMusic;

    private bool isSoundOn = true;

    void Start()
    {
        btnStart.onClick.AddListener(clickStart);
        btnMenu.onClick.AddListener(clickBtnMenu);
        btnMusic.onClick.AddListener(clickBtnMusic);
        btnReset.onClick.AddListener(clickBtnReset);
        btnInfo.onClick.AddListener(clickBtnInfo);
        btnBigClose.onClick.AddListener(ClosePanelMenu);

        // Lấy trạng thái âm thanh từ PlayerPrefs
        isSoundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        btnOffMusic.gameObject.SetActive(!isSoundOn);
    }


    void clickStart()
    {
        ////xoa
        //SceneManager.LoadScene("14");
        //PlayerPrefs.SetInt("level", 14);

        LevelProgression.Continue();

    }

    void clickBtnMenu()
    {
        panelMenu.SetActive(!panelMenu.activeSelf);
        btnOffMusic.gameObject.SetActive(!isSoundOn);
    }
    void ClosePanelMenu()
    {
        panelMenu.SetActive(false);
    }
    void clickBtnMusic()
    {
        isSoundOn = !isSoundOn;

        // Lưu trạng thái âm thanh
        PlayerPrefs.SetInt("SoundOn", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();

        // Áp dụng trạng thái âm thanh
        ApplySoundState();
    }

    void ApplySoundState()
    {
        if (soundmanager.instance && soundmanager.instance._As) soundmanager.instance._As.mute = !isSoundOn;
        btnOffMusic.gameObject.SetActive(!isSoundOn);
    }
    void clickBtnReset()
    {
        UIManager.instance.ShowNotice();

    }

    void clickBtnInfo()
    {
        UIManager.instance.ShowPopupMenu();
    }
}
