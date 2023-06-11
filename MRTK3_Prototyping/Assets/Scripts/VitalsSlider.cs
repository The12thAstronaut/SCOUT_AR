using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using Slider = Microsoft.MixedReality.Toolkit.UX.Slider;

[RequireComponent(typeof(Slider))]
[RequireComponent(typeof(RectTransform))]
public class VitalsSlider : MonoBehaviour
{
	public float nominalMax { get; set; }
    public float nominalMin { get; set; }
	public float errorMax { get; set; }
	public float errorMin { get; set; }
    public Color errorColor { get; set; }
	public float maxVal { get; set; }
	public float minVal { get; set; }

	private Material mat;
    private Color originalColor;

	private float normalizedValue;
    private Slider slider;

	public float value;

	// Start is called before the first frame update
	void Start()
    {
		slider = GetComponent<Slider>();
        mat = Instantiate<Material>(gameObject.GetNamedChild("SliderTrack").transform.GetChild(0).GetComponent<RawImage>().material);
        gameObject.GetNamedChild("SliderTrack").transform.GetChild(0).GetComponent<RawImage>().material = mat;

		originalColor = mat.GetColor("_Color");
		UpdateSlider();
	}

    // Update is called once per frame
    void Update()
    {
        UpdateSlider();
    }

    private void ClampVal() {
        if (value > maxVal) {
            value = maxVal;
        } else if (value < minVal) {
            value = minVal;
        }
    }

    private void UpdateSlider() {
		ClampVal();
		normalizedValue = (value - minVal) / (maxVal - minVal);
		slider.Value = normalizedValue;

        if (nominalMax > errorMax && value <= errorMax) {
            mat.SetColor("_Color", errorColor);
        } else if (nominalMax < errorMax && value >= errorMin) {
			mat.SetColor("_Color", errorColor);
		} else {
			mat.SetColor("_Color", originalColor);
		}
	}
}
