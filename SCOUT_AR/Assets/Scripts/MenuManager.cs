using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu(Transform menu) {
        menu.gameObject.SetActive(true);
        PositionMenu menuPositioner = menu.GetComponent<PositionMenu>();
        menuPositioner.pinButton.ForceSetToggled(false);
        menuPositioner.OpenMenu();
    }

	public void CloseMenu(Transform menu) {
		PositionMenu menuPositioner = menu.GetComponent<PositionMenu>();
		menuPositioner.pinButton.ForceSetToggled(true);
		menu.gameObject.SetActive(false);
	}
}
