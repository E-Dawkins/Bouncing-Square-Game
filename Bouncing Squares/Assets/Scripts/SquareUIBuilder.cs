using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SquareUIBuilder : MonoBehaviour
{
    public GameObject textPrefab;
    public GameObject vec2Prefab;
    public GameObject floatPrefab;
    public GameObject intPrefab;
    public GameObject dropdownPrefab;
    public GameObject componentButtonPrefab;

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

    public void AddDropdown(string label, List<string> options, UnityAction<int> callback)
    {
        GameObject dropdownObj = Instantiate(dropdownPrefab, transform);

        TMP_Text tmpText = dropdownObj?.transform?.GetChild(0)?.GetComponent<TMP_Text>();
        tmpText?.SetText(label);


        TMP_Dropdown dropdownInput = dropdownObj?.transform?.GetChild(1)?.GetComponent<TMP_Dropdown>();
        dropdownInput?.ClearOptions();
        dropdownInput?.AddOptions(options);
        dropdownInput?.SetValueWithoutNotify(0);

        dropdownInput?.onValueChanged.AddListener(callback);
    }

    public void AddModifierButton(string label, BouncingSquare square, UnityAction<string> callback)
    {
        GameObject parentObj = Instantiate(componentButtonPrefab, transform);


        GameObject dropdownObj = parentObj?.transform?.GetChild(0)?.gameObject;

        TMP_Dropdown dropdownComp = dropdownObj?.GetComponent<TMP_Dropdown>();
        List<string> options = new List<string>();
        foreach (Type type in ModifierAttribs.Modifiers) // only add options that square doesn't have already
        {
            IModifier modifierInst = (Activator.CreateInstance(type) as IModifier);
            int foundIndex = square.modifiers.FindIndex((modifier) => { return modifier.displayName == modifierInst.displayName; });

            if (foundIndex == -1)
            {
                options.Add(modifierInst.displayName);
            }
        }
        dropdownComp?.AddOptions(options);


        GameObject buttonObj = parentObj?.transform?.GetChild(1)?.gameObject;

        TMP_Text textComp = buttonObj?.transform?.GetChild(0)?.GetComponent<TMP_Text>();
        textComp?.SetText(label);

        Button buttonComp = buttonObj?.GetComponent<Button>();
        buttonComp?.onClick.AddListener(() => 
        {
            // update square modifiers
            callback.Invoke(dropdownComp?.options[dropdownComp.value].text);

            // update dropdown options
            List<TMP_Dropdown.OptionData> newOptions = new (dropdownComp?.options);
            newOptions.RemoveAt(dropdownComp.value);
            dropdownComp?.ClearOptions();
            dropdownComp?.AddOptions(newOptions);

            // update button child index
            parentObj.transform.SetAsLastSibling();
        });
    }
}
