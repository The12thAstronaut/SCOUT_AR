using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickAccess : MonoBehaviour
{
	public RectTransform reference;
	private RectTransform rt;

	// Start is called before the first frame update
	void Start()
    {
		rt = gameObject.GetComponent<RectTransform>();
		rt.localPosition = new Vector3(reference.localPosition.x, rt.localPosition.y, rt.localPosition.z);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
