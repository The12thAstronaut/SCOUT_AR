using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using Slider = Microsoft.MixedReality.Toolkit.UX.Slider;

public class VitalInfoCard : MonoBehaviour
{
	public float nominalMax { get; set; }
	public float nominalMin { get; set; }
	public float errorMax { get; set; }
	public float errorMin { get; set; }
	public string decimalFormat { get; set; }
	public string vitalName { get; set; }
	public Color errorColor { get; set; }
	public Color goodColor { get; set; }
	public float maxVal { get; set; }
	public float minVal { get; set; }

	private Material fillMat;
	private Color originalColor;

	private float normalizedValue;

	public Slider slider;
	public TextMeshProUGUI minText;
	public TextMeshProUGUI maxText;
	public TextMeshProUGUI errorText;
	public TextMeshProUGUI currentText;
	public TextMeshProUGUI valueText;
	public TextMeshProUGUI nameText;
	

	private float sliderWidth;
	private RectTransform valueRT;

	public float value;

	// Start is called before the first frame update
	void Start()
    {
		sliderWidth = slider.gameObject.GetComponent<RectTransform>().sizeDelta.x;
		valueRT = currentText.gameObject.GetComponent<RectTransform>();

		minText.text = minVal.ToString(decimalFormat);
		maxText.text = maxVal.ToString(decimalFormat);
		currentText.text = value.ToString(decimalFormat);
		valueText.text = currentText.text;
		nameText.text = vitalName;

		RectTransform errorRT = errorText.gameObject.GetComponent<RectTransform>();
		if (nominalMax > errorMax) {
			errorText.text = errorMax.ToString(decimalFormat);
			errorRT.localPosition = new Vector3(((errorMax - minVal) / (maxVal - minVal) - 0.5f) * sliderWidth, errorRT.localPosition.y, errorRT.localPosition.z);

			maxText.color = goodColor;
			maxText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = goodColor;
			minText.color = errorColor;
			minText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = errorColor;
		} else {
			errorText.text = errorMin.ToString(decimalFormat);
			errorRT.localPosition = new Vector3(((errorMin - minVal) / (maxVal - minVal) - 0.5f) * sliderWidth, errorRT.localPosition.y, errorRT.localPosition.z);
		}

		valueRT.localPosition = new Vector3(((value - minVal) / (maxVal - minVal) - 0.5f) * sliderWidth, valueRT.localPosition.y, valueRT.localPosition.z);

		fillMat = Instantiate<Material>(slider.gameObject.GetNamedChild("SliderTrack").transform.GetChild(0).GetComponent<RawImage>().material);
		slider.gameObject.GetNamedChild("SliderTrack").transform.GetChild(0).GetComponent<RawImage>().material = fillMat;

		originalColor = fillMat.GetColor("_Color");
		UpdateSlider();
	}

	// Update is called once per frame
	void Update() {
		valueRT.localPosition = new Vector3(((value - minVal) / (maxVal - minVal) - 0.5f) * sliderWidth, valueRT.localPosition.y, valueRT.localPosition.z);
		currentText.text = value.ToString(decimalFormat);
		valueText.text = currentText.text;
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
			fillMat.SetColor("_Color", errorColor);
		} else if (nominalMax < errorMax && value >= errorMin) {
			fillMat.SetColor("_Color", errorColor);
		} else {
			fillMat.SetColor("_Color", originalColor);
		}
	}
}
