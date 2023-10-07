using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUI : MonoBehaviour
{
    public RectTransform reference;
	private RectTransform rt;
    public float spacing = 2f;
    public float offset = 0f;
    public int right = 1;

    private int posingTry = 0;

    // Start is called before the first frame update
    void Start()
    {
		rt = gameObject.GetComponent<RectTransform>();
		rt.localPosition = new Vector3(right * (reference.sizeDelta.x / 2 + rt.sizeDelta.x / 2 + spacing - offset), rt.localPosition.y, rt.localPosition.z);
	}

	// Update is called once per frame
	void Update()
    {

	}

	private void LateUpdate() {
		if (posingTry++ < 2) {
			rt.localPosition = new Vector3(right * (reference.sizeDelta.x / 2 + rt.sizeDelta.x / 2 + spacing + offset), rt.localPosition.y, rt.localPosition.z);
		}
	}
}
