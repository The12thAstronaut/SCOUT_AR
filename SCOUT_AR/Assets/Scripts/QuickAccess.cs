using Microsoft.MixedReality.GraphicsTools;
using Microsoft.MixedReality.Toolkit.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickAccess : MonoBehaviour
{
	public GameObject reference;
	[Tooltip("Seconds until quick access bar automatically closes")]
	public float closeTime = 10;

	private RectTransform rt;
	private float openTime;
	public bool opened { get; set; }

	// Start is called before the first frame update
	void Start()
    {
		rt = gameObject.GetComponent<RectTransform>();
		rt.localPosition = new Vector3(reference.GetComponent<RectTransform>().localPosition.x, rt.localPosition.y, rt.localPosition.z);
	}

    // Update is called once per frame
    void Update()
    {
        if (opened && Time.time - openTime >= closeTime) {
			reference.GetComponent<PressableButton>().ForceSetToggled(false);
		}
    }

	public void SetOpenTime() {
		openTime = Time.time;
	}

	public void DisableAnimator() {
		gameObject.GetComponent<Animator>().enabled = false;
	}

	public void DisableGameObject() {
		gameObject.SetActive(false);
	}

	public void FixGraphics() {
		gameObject.GetComponent<CanvasElementRoundedRect>().Radius = 12.9f;
	}

	public void FixGraphics2() {
		gameObject.GetComponent<CanvasElementRoundedRect>().Radius = 13f;
	}
}
