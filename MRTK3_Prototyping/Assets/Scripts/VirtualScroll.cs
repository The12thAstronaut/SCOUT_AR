using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class VirtualScroll : MonoBehaviour
{
    private Vector3 handStartPos = Vector3.zero;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isScrolling = false;
    private IXRSelectInteractor activeInteractor;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform parentRT = transform.parent.GetComponent<RectTransform>();

		startPos = transform.parent.localPosition + new Vector3(-parentRT.sizeDelta.x, parentRT.sizeDelta.y, 0);
		endPos = transform.parent.localPosition + new Vector3(parentRT.sizeDelta.x, -parentRT.sizeDelta.y, 0);
	}

    // Update is called once per frame
    void Update()
    {
        if (isScrolling) {
            float delta = (transform.InverseTransformPoint(activeInteractor.transform.position + activeInteractor.transform.forward * .5f).y - endPos.y) / (startPos.y - endPos.y);
			if (delta >= 0f && delta <= 1f) transform.parent.GetComponent<Scrollbar>().value = delta;
        }
    }

    public void StartScrolling() {
        activeInteractor = GetComponent<StatefulInteractable>().firstInteractorSelecting;
		handStartPos = transform.InverseTransformPoint(activeInteractor.transform.position);
        handStartPos = transform.localPosition;
        isScrolling = true;
    }

    public void StopScrolling() {
        isScrolling = false;
    }
}
