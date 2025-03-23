using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class SquareUIBuilder : MonoBehaviour
{
    public GameObject textPrefab;
    public GameObject vec2Prefab;
    public GameObject floatPrefab;
    public GameObject intPrefab;

    public void ClearUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i)?.gameObject);
        }
    }

    public void AddText(string text)
    {
        GameObject textObj = Instantiate(textPrefab, transform);
        textObj?.GetComponent<TMP_Text>()?.SetText(text);
    }

    public void AddVec2(string label, Vector2 value, UnityAction<Vector2> callback)
    {
        GameObject vec2Obj = Instantiate(vec2Prefab, transform);

        TMP_Text tmpText = vec2Obj?.transform?.GetChild(0)?.GetComponent<TMP_Text>();
        tmpText?.SetText(label);


        GameObject inputsParent = vec2Obj?.transform?.GetChild(1).gameObject;

        TMP_InputField xInput = inputsParent?.transform?.GetChild(0)?.GetComponent<TMP_InputField>();
        xInput?.SetTextWithoutNotify(value.x.ToString());
        TMP_InputField yInput = inputsParent?.transform?.GetChild(1)?.GetComponent<TMP_InputField>();
        yInput?.SetTextWithoutNotify(value.y.ToString());

        void localCallback(string data)
        {
            Vector2 localValue = Vector2.zero;
            if (float.TryParse(xInput.text, out localValue.x) && float.TryParse(yInput.text, out localValue.y))
            {
                callback(localValue);
            }
        }

        xInput?.onValueChanged.AddListener(localCallback);
        yInput?.onValueChanged.AddListener(localCallback);
    }

    public void AddFloat(string label, float value, UnityAction<float> callback)
    {
        GameObject floatObj = Instantiate(floatPrefab, transform);

        TMP_Text tmpText = floatObj?.transform?.GetChild(0)?.GetComponent<TMP_Text>();
        tmpText?.SetText(label);


        TMP_InputField floatInput = floatObj?.transform?.GetChild(1)?.GetComponent<TMP_InputField>();
        floatInput?.SetTextWithoutNotify(value.ToString());

        void localCallback(string data)
        {
            if (float.TryParse(data, out float dataValue))
            {
                callback(dataValue);
            }
        }

        floatInput?.onValueChanged.AddListener(localCallback);
    }

    public void AddInt(string label, int value, UnityAction<int> callback)
    {
        GameObject intObj = Instantiate(intPrefab, transform);

        TMP_Text tmpText = intObj?.transform?.GetChild(0)?.GetComponent<TMP_Text>();
        tmpText?.SetText(label);


        TMP_InputField intInput = intObj?.transform?.GetChild(1)?.GetComponent<TMP_InputField>();
        intInput?.SetTextWithoutNotify(value.ToString());

        void localCallback(string data)
        {
            if (int.TryParse(data, out int dataValue))
            {
                callback(dataValue);
            }
        }

        intInput?.onValueChanged.AddListener(localCallback);
    }
}
