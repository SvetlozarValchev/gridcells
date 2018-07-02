using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UITooltip : MonoBehaviour {
    public GameObject container;

    public GameObject prefabText;
    public GameObject prefabButton;
    public GameObject prefabSlider;
    
    List<GameObject> elements = new List<GameObject>();


    float totalOffset = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    void SetOffset(GameObject element)
    {
        Vector2 position = element.transform.position;

        position.y -= totalOffset;

        element.transform.position = position;

        totalOffset += element.GetComponent<RectTransform>().rect.height;
    }

    public void Clear()
    {
        foreach (GameObject element in elements)
        {
            Destroy(element);
        }

        elements.Clear();
        totalOffset = 0f;
    }

    public Text SetText(string key, string text)
    {
        GameObject textObject = Instantiate(prefabText, container.transform);
        Text textComponent = textObject.GetComponent<Text>();

        textComponent.text = text;

        SetOffset(textObject);

        elements.Add(textObject);

        return textComponent;
    }

    public Button SetButton(string key, string title, UnityAction onClick)
    {
        GameObject buttonObject = Instantiate(prefabButton, container.transform);
        Button buttonComponent = buttonObject.GetComponent<Button>();

        buttonObject.GetComponentInChildren<Text>().text = title;
        buttonComponent.onClick.AddListener(onClick);

        SetOffset(buttonObject);

        elements.Add(buttonObject);

        return buttonComponent;
    }

    public Slider SetSlider(string key, float startingValue = 0.0f, float minValue = 0.0f, float maxValue = 1.0f)
    {
        GameObject sliderObject = Instantiate(prefabSlider, container.transform);
        Slider sliderComponent = sliderObject.GetComponent<Slider>();

        sliderComponent.minValue = minValue;
        sliderComponent.maxValue = maxValue;
        sliderComponent.value = startingValue;

        SetOffset(sliderObject);

        elements.Add(sliderObject);

        return sliderComponent;
    }
}
