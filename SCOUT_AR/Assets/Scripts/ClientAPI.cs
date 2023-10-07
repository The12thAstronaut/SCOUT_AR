using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using TMPro;

public class ClientAPI : MonoBehaviour
{
    public TelemetryStream telemetryStream;
    public string url;
    public TMP_InputField urlInputField;

    private IndicatorManager indicatorManager;

	void Start() {
        urlInputField.text = url;

        indicatorManager = FindObjectOfType<IndicatorManager>();
	}

	/*IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            StartCoroutine(Get(url));
        }
    }*/

	public void Connect() {
        url = urlInputField.text;
		
		StartCoroutine(GetTelemetry(url));
		//}
	}

    public IEnumerator GetTelemetry(string url)
    {
        while (true) {
            yield return new WaitForSeconds(1.0f);

            using (UnityWebRequest www = UnityWebRequest.Get(url)) {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError) {
                    Debug.Log(www.error);
                    indicatorManager.TelemetryActive(false);
                } else {
                    if (www.isDone) {
                        indicatorManager.TelemetryActive(true);

                        // handle the result
                        string result = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                        //Debug.Log(result);     //used to see if values are being registered 

                        result = "{\"result\":[" + result + "]}";

                        List<Suit> data = JsonHelper.ListFromJson<Suit>(result);

                        Suit suit_data = data[0];

                        telemetryStream.SetBpmText(suit_data.heart_bpm);
                        telemetryStream.SetPSubText(suit_data.p_sub);
                        telemetryStream.SetPSuitText(suit_data.p_suit);
                        telemetryStream.SetTSubText(suit_data.t_sub); // temperature
                        telemetryStream.SetVFanText(suit_data.v_fan);
                        telemetryStream.SetPO2Text(suit_data.p_o2);
                        telemetryStream.SetRO2Text(suit_data.rate_o2);
                        telemetryStream.SetPH2OGText(suit_data.p_h2o_g);
                        telemetryStream.SetPH2OLText(suit_data.p_h2o_l);
                        telemetryStream.SetPSOPText(suit_data.p_sop);
                        telemetryStream.SetBatLifeText(suit_data.t_battery);
                        telemetryStream.SetOxLifeText(suit_data.t_oxygen);
                        telemetryStream.SetH2OLifeText(suit_data.t_water);
                        telemetryStream.SetDateText(suit_data.create_date);
                        telemetryStream.SetEvaTimeText(suit_data.timer);
                        telemetryStream.SetPrimO2Text(suit_data.ox_primary);
                        telemetryStream.SetSecondaryO2Text(suit_data.ox_secondary);
                        telemetryStream.SetRSOPText(suit_data.rate_sop);
                        telemetryStream.SetBattCapText(suit_data.cap_battery);


                    } else {
                        //handle the problem
                        Debug.Log("Error! data couldn't get.");
                    }
                }
            }
        }
    }

    void TelemetryUpdate()
    {
        Debug.Log("Updated telemetry stream.");
    }
}

public struct Suit {
	public string heart_bpm;
	public string p_sub;
	public string p_suit;
	public string t_sub;
	public string v_fan;
	public string p_o2;
	public string rate_o2;
	public string cap_battery;
	public string p_h2o_g;
	public string p_h2o_l;
	public string p_sop;
	public string rate_sop;
	public string t_battery;
	public string t_oxygen;
	public string t_water;
	public string create_date;
	public string timer;
	public string ox_primary;
	public string ox_secondary;
}