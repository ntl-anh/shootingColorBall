using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Unscaled feedback remains responsive inside settings; no layout or physics changes.
public class JellyPopPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    Vector3 rest;
    bool down;
    void Awake() { rest = transform.localScale; }
    public void OnPointerDown(PointerEventData data)
    {
        var button = GetComponent<Button>();
        down = button && button.IsInteractable();
    }
    public void OnPointerUp(PointerEventData data) { down = false; }
    public void OnPointerExit(PointerEventData data) { down = false; }
    void Update()
    {
        var target = rest * (down && GamePreferences.Effects ? .94f : 1);
        transform.localScale = Vector3.Lerp(transform.localScale, target, 1 - Mathf.Exp(-24 * Time.unscaledDeltaTime));
    }
    void OnDisable() { down = false; transform.localScale = rest; }
}
