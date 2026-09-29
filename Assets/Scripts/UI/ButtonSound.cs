using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Put on any button to give it the click and hover sounds.
[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => AudioManager.Instance?.PlayClick());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // no hover on a touch screen, this is only for testing with a mouse in the editor
        if (eventData.pointerId < 0) AudioManager.Instance?.PlayHover();
    }
}
