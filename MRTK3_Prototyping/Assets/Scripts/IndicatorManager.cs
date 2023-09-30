using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorManager : MonoBehaviour
{
    public Color activeColor = Color.white;
    public Color deactiveColor = Color.gray;

	public TextMeshProUGUI dateTimeLabel;

    public TextMeshProUGUI networkIndicator;
    public TextMeshProUGUI markersIndicator;
    public TextMeshProUGUI audioIndicator;
    public TextMeshProUGUI speechIndicator;
    public Image alertIndicator;
    public TextMeshProUGUI telemetryIndicator;

    // Start is called before the first frame update
    void Start()
    {
		networkIndicator.color = deactiveColor;
		markersIndicator.color = activeColor;
		audioIndicator.color = activeColor;
		speechIndicator.color = deactiveColor;
		alertIndicator.color = deactiveColor;
		telemetryIndicator.color = deactiveColor;
	}

    // Update is called once per frame
    void Update()
    {
		dateTimeLabel.text = System.DateTime.Now.ToString();
    }

    public void NetworkActive(bool active) {
        if (active) {
            networkIndicator.color = activeColor;
        } else {
            networkIndicator.color = deactiveColor;
        }
    }

	public void MarkersActive(bool active) {
		if (active) {
			markersIndicator.color = activeColor;
		} else {
			markersIndicator.color = deactiveColor;
		}
	}

	public void AudioActive(bool active) {
		if (active) {
			audioIndicator.color = activeColor;
		} else {
			audioIndicator.color = deactiveColor;
		}
	}

	public void SpeechActive(bool active) {
		if (active) {
			speechIndicator.color = activeColor;
		} else {
			speechIndicator.color = deactiveColor;
		}
	}

	public void AlertActive(bool active) {
		if (active) {
			alertIndicator.color = activeColor;
		} else {
			alertIndicator.color = deactiveColor;
		}
	}

	public void TelemetryActive(bool active) {
		if (active) {
			telemetryIndicator.color = activeColor;
		} else {
			telemetryIndicator.color = deactiveColor;
		}
	}
}
