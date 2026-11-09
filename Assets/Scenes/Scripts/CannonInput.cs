using UnityEngine;
using UnityEngine.EventSystems;

// Routes every tap (each finger separately) to the nearest cannon under it. Unity's OnMouseDown only
// follows one pointer and stops at the first collider hit, so a second finger or a ball flying across
// a cannon could swallow a tap and rapid firing felt broken.
public class CannonInput : MonoBehaviour
{
    UnityEngine.Camera view;
    readonly RaycastHit[] hits = new RaycastHit[32];

    void Update()
    {
        if (CalmGameUI.InputBlocked) return;
        for (int i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began) Tap(touch.position, touch.fingerId);
        }
        // With no touches this is a real mouse click; touch-simulated clicks are already handled above.
        if (Input.touchCount == 0 && Input.GetMouseButtonDown(0)) Tap(Input.mousePosition, -1);
    }
    void Tap(Vector2 screen, int fingerId)
    {
        var events = EventSystem.current;
        if (events && (fingerId >= 0 ? events.IsPointerOverGameObject(fingerId) : events.IsPointerOverGameObject())) return;
        if (!view || !view.isActiveAndEnabled)
        {
            view = null;
            foreach (var camera in UnityEngine.Camera.allCameras)
                if (!camera.targetTexture) { view = camera; break; }
            if (!view) return;
        }
        int count = Physics.RaycastNonAlloc(view.ScreenPointToRay(screen), hits, 200f, ~0, QueryTriggerInteraction.Collide);
        Shootcolor best = null;
        float nearest = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            var cannon = hits[i].collider.GetComponentInParent<Shootcolor>();
            if (cannon && hits[i].distance < nearest) { best = cannon; nearest = hits[i].distance; }
        }
        if (best) best.Fire();
    }
}
