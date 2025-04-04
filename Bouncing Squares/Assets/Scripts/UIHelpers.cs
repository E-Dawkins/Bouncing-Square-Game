using UnityEngine;

public class UIHelpers : MonoBehaviour
{
    public void ToggleObjectActiveState() => gameObject.SetActive(!gameObject.activeSelf);
}
