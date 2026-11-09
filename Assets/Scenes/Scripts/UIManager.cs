using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    PopupInfo popupInfo;
    CanvasNotice canvasNotice;
    [SerializeField] Canvas canvas;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
    }

    PopupInfo GetPopupMenu()
    {
        if (popupInfo == null)
        {
            GameObject obj = UtilityCode.GetGameObjInResource("UI/PanelInfo");
            popupInfo = Instantiate(obj.GetComponent<PopupInfo>(), Vector3.zero, Quaternion.identity, canvas.transform);
            obj.transform.localPosition = Vector3.zero;
            popupInfo.Init();
        }

        return popupInfo;
    }
    CanvasNotice GetPopupNotice()
    {
        if (canvasNotice == null)
        {
            GameObject obj = UtilityCode.GetGameObjInResource("UI/Notice");
            canvasNotice = Instantiate(obj.GetComponent<CanvasNotice>(), Vector3.zero, Quaternion.identity, canvas.transform);
            obj.transform.localPosition = Vector3.zero;
            canvasNotice.Init();
        }

        return canvasNotice;
    }
    public void ShowPopupMenu()
    {
        GetPopupMenu().Show();
    } public void ShowNotice()
    {
        GetPopupNotice().Show();
    }
}
