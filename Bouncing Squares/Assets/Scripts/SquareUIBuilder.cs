using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SquareUIBuilder : MonoBehaviour
{
    public GameObject textPrefab;
    public GameObject vec2Prefab;

    public void BuildUI(BouncingSquare square)
    {
        AddText(square.name);
        AddVec2(square.transform.position.x, square.transform.position.y, square.PositionChangedCallback);

        foreach (IModifier modifier in square.modifiers)
        {
            AddText(modifier.displayName);
            AddText(modifier.hint);
        }
    }

    public void ClearUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i)?.gameObject);
        }
    }

    private void AddText(string text)
    {
        GameObject textObj = Instantiate(textPrefab, transform);
        textObj?.GetComponent<TMP_Text>()?.SetText(text);
    }

    private void AddVec2(float x, float y, UnityAction<bool, string> callback)
    {
        GameObject vec2Obj = Instantiate(vec2Prefab, transform);
        vec2Obj?.GetComponent<TMP_Text>()?.SetText("Position");

        TMP_InputField xInput = vec2Obj?.transform?.GetChild(0)?.GetComponent<TMP_InputField>();
        xInput?.SetTextWithoutNotify(x.ToString());
        xInput?.onValueChanged.AddListener((data) => { callback.Invoke(true, data); });

        TMP_InputField yInput = vec2Obj?.transform?.GetChild(1)?.GetComponent<TMP_InputField>();
        yInput?.SetTextWithoutNotify(y.ToString());
        yInput?.onValueChanged.AddListener((data) => { callback.Invoke(false, data); });
    }
}
