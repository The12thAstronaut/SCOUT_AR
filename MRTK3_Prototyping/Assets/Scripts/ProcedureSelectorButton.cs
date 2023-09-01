using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProcedureSelectorButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI stepText;
    public TextMeshProUGUI durationText;
    public int procedureIndex { get; set; }

    public ProcedureManager procedureManager { get; set; }

    // Start is called before the first frame update
    void Start()
    {
		transform.localRotation = Quaternion.identity;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectProcedure() {
        procedureManager.ActivateProcedure(procedureIndex);
    }
}
