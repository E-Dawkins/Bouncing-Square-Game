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
    public GameObject modifierLabelPrefab;

    private GameObject componentButtonInstance;

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

    public void AddVec2(string label, Vector2 value, UnityAction<Vector2> callback, Vector2? clampedMin = null, Vector2? clampedMax = null)
    {
        // initialize default range
        if (clampedMin == null) clampedMin = Vector2.negativeInfinity;
        if (clampedMax == null) clampedMax = Vector2.positiveInfinity;

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
                // clamp the localValue before invoking callback
                Vector2 clampedValue = localValue;
                clampedValue = Vector2.Max(clampedValue, clampedMin.Value);
                clampedValue = Vector2.Min(clampedValue, clampedMax.Value);
                if (clampedValue != localValue)
                {
                    xInput.text = clampedValue.x.ToString();
                    yInput.text = clampedValue.y.ToString();
                }

                callback(clampedValue);
            }
        }

        xInput?.onValueChanged.AddListener(localCallback);
        yInput?.onValueChanged.AddListener(localCallback);
    }

    public void AddFloat(string label, float value, UnityAction<float> callback, Vector2? clampedRange = null)
    {
        // initialize default range
        if (clampedRange == null)
            clampedRange = new Vector2(Mathf.NegativeInfinity, Mathf.Infinity);

        GameObject floatObj = Instantiate(floatPrefab, transform);

        TMP_Text tmpText = floatObj?.transform?.GetChild(0)?.GetComponent<TMP_Text>();
        tmpText?.SetText(label);


        TMP_InputField floatInput = floatObj?.transform?.GetChild(1)?.GetComponent<TMP_InputField>();
        floatInput?.SetTextWithoutNotify(value.ToString());

        void localCallback(string data)
        {
            if (float.TryParse(data, out float dataValue))
            {
                // clamp the dataValue before invoking callback
                float clampedValue = Mathf.Clamp(dataValue, clampedRange.Value.x, clampedRange.Value.y);
                if (dataValue != clampedValue) floatInput.SetTextWithoutNotify(clampedValue.ToString());

                callback(clampedValue);
            }
        }

        floatInput?.onValueChanged.AddListener(localCallback);
    }

    public void AddInt(string label, int value, UnityAction<int> callback, Vector2? clampedRange = null)
    {
        // initialize default range
        if (clampedRange == null)
            clampedRange = new Vector2(Mathf.NegativeInfinity, Mathf.Infinity);

        GameObject intObj = Instantiate(intPrefab, transform);

        TMP_Text tmpText = intObj?.transform?.GetChild(0)?.GetComponent<TMP_Text>();
        tmpText?.SetText(label);


        TMP_InputField intInput = intObj?.transform?.GetChild(1)?.GetComponent<TMP_InputField>();
        intInput?.SetTextWithoutNotify(value.ToString());

        void localCallback(string data)
        {
            if (int.TryParse(data, out int dataValue))
            {
                // clamp the dataValue before invoking callback
                int clampedValue = (int)Mathf.Clamp(dataValue, clampedRange.Value.x, clampedRange.Value.y);
                if (dataValue != clampedValue) intInput.SetTextWithoutNotify(clampedValue.ToString());

                callback(clampedValue);
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

        dropdownInput?.onValueChanged.AddListener((i) => { callback(i); });
    }

    public void AddModifierButton(string label, BouncingSquare square, UnityAction<string> callback)
    {
        GameObject parentObj = Instantiate(componentButtonPrefab, transform);
        componentButtonInstance = parentObj;


        UpdateComponentButton(square);


        GameObject buttonObj = parentObj?.transform?.GetChild(1)?.gameObject;

        TMP_Text textComp = buttonObj?.transform?.GetChild(0)?.GetComponent<TMP_Text>();
        textComp?.SetText(label);

        Button buttonComp = buttonObj?.GetComponent<Button>();
        buttonComp?.onClick.AddListener(() => 
        {
            // update square modifiers
            GameObject dropdownObj = parentObj?.transform?.GetChild(0)?.gameObject;
            TMP_Dropdown dropdownComp = dropdownObj?.GetComponent<TMP_Dropdown>();
            callback.Invoke(dropdownComp?.options[dropdownComp.value].text);

            // update dropdown options
            UpdateComponentButton(square);

            // update button child index
            parentObj.transform.SetAsLastSibling();
        });
    }

    public void AddModifierLabel(IModifier modifier, BouncingSquare square)
    {
        GameObject parentObj = Instantiate(modifierLabelPrefab, transform);

        TMP_Text textComp = parentObj?.transform.GetChild(0)?.GetComponent<TMP_Text>();
        textComp?.SetText(modifier.displayName);

        Button buttonComp = parentObj?.transform.GetChild(1)?.GetComponent<Button>();
        buttonComp?.onClick.AddListener(() => 
        {
            // destroy this label and every object after it, until next label
            Destroy(parentObj);
            for (int i = parentObj.transform.GetSiblingIndex() + 1; i <= transform.childCount; i++)
            {
                GameObject childObj = transform.GetChild(i).gameObject;

                if (childObj.name == (modifierLabelPrefab.name + "(Clone)") ||
                    childObj.name == (componentButtonPrefab.name + "(Clone)"))
                {
                    break;
                }

                Destroy(childObj);
            }

            // remove this modifiers' ui elements
            for (int i = 0; i < square.modifiers.Count; i++)
            {
                if (square.modifiers[i].displayName == modifier.displayName)
                {
                    square.modifiers.RemoveAt(i);
                    break;
                }
            }

            UpdateComponentButton(square);
        });
    }

    private void UpdateComponentButton(BouncingSquare square)
    {
        GameObject dropdownObj = componentButtonInstance?.transform?.GetChild(0)?.gameObject;

        TMP_Dropdown dropdownComp = dropdownObj?.GetComponent<TMP_Dropdown>();
        dropdownComp?.ClearOptions();

        List<string> options = new List<string>();
        foreach (Type type in ModifierAttribs.Modifiers) // only add options that square doesn't have already
        {
            IModifier modifierInst = (Activator.CreateInstance(type, new object[] { square }) as IModifier);
            int foundIndex = square.modifiers.FindIndex((modifier) => { return modifier.displayName == modifierInst.displayName; });

            if (foundIndex == -1)
            {
                options.Add(modifierInst.displayName);
            }
        }
        dropdownComp?.AddOptions(options);
    }
}
