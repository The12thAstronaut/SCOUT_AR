using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Scrollbar))]
public class VirtualScroll : MonoBehaviour
{
    public bool stickyEnds = true;
    public bool reverseScroll = false;
    [Range(0.2f, 1f)] public float scrollSensitivity = 1f;

    private Vector3 startPos;
    private Vector3 endPos;
    private bool isScrolling = false;
    private IXRSelectInteractor activeInteractor;
    private Scrollbar scrollbar;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform scrollRT = GetComponent<RectTransform>();
        scrollbar = GetComponent<Scrollbar>();

		startPos = transform.localPosition + new Vector3(-scrollRT.sizeDelta.x, scrollRT.sizeDelta.y, 0) / scrollSensitivity;
		endPos = transform.localPosition + new Vector3(scrollRT.sizeDelta.x, -scrollRT.sizeDelta.y, 0) / scrollSensitivity;
	}

    // Update is called once per frame
    void Update()
    {
        if (isScrolling) {

            Plane scrollPlane = new Plane(-transform.forward, transform.position);
            Ray ray = new Ray(activeInteractor.transform.position, activeInteractor.transform.forward);

            float intersectDist = 0.5f;
            scrollPlane.Raycast(ray, out intersectDist);

            float delta = 0f;
			if (scrollbar.direction == Scrollbar.Direction.TopToBottom || scrollbar.direction == Scrollbar.Direction.BottomToTop) {
				delta = (transform.InverseTransformPoint(activeInteractor.transform.position + activeInteractor.transform.forward * intersectDist).y - endPos.y) / (startPos.y - endPos.y);
			} else {
				delta = (transform.InverseTransformPoint(activeInteractor.transform.position + activeInteractor.transform.forward * intersectDist).x - endPos.x) / (startPos.x - endPos.x);
			}

            if (intersectDist < 0f || delta < 0f) {
				if (scrollbar.direction == Scrollbar.Direction.TopToBottom || scrollbar.direction == Scrollbar.Direction.BottomToTop) {
					delta = (transform.InverseTransformPoint(activeInteractor.transform.position + activeInteractor.transform.forward * 0.5f).y - endPos.y) / (startPos.y - endPos.y);
				} else {
					delta = (transform.InverseTransformPoint(activeInteractor.transform.position + activeInteractor.transform.forward * 0.5f).x - endPos.x) / (startPos.x - endPos.x);
				}
			}

            if (delta >= 0f && delta <= 1f) {
                if (stickyEnds) {
                    if (delta <= 0.04f || delta >= 0.96f) {
                        delta = Mathf.Round(delta);
                    }
                }

                if (reverseScroll) {
                    delta = 1 - delta;
                }

                scrollbar.value = delta;
            }
        }
    }

    public void StartScrolling() {
        activeInteractor = scrollbar.handleRect.GetComponent<StatefulInteractable>().firstInteractorSelecting;
        isScrolling = true;
    }

    public void StopScrolling() {
        isScrolling = false;
    }
}
