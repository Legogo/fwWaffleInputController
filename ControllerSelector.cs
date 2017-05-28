using UnityEngine;
using System.Collections;

/*
 * Class qui permet de déterminer si on utilise XINPUT ou l'input de unity (win/mac)
 * */

public class ControllerSelector : MonoBehaviour {
  
  [RuntimeInitializeOnLoadMethod]
	static protected void activePlatformManager(){

    //déjà dans la scène ?
    ControllerManager cm = GameObject.FindObjectOfType<ControllerManager>();
    if (cm != null) return;
    
    //ControllerManager cm = UnityTools.getManager<ControllerManager>("[input]");
    //if (cm != null) return;

    if (ControllerManager.isMac()){
			Debug.Log("<ControllerSelector> Is under MACOS, using unity inputManager");
      _manager = UnityTools.getManager<ControllerManager>("[input]");
			
      ControllerManager.XINPUT = false;

    }else if(ControllerManager.isWindows()){
      Debug.Log("<ControllerSelector> Is under WINDOWS, <b>using xinput dll</b>");
      _manager = UnityTools.getManager<XinputControllerManager>("[input]");
      
      ControllerManager.XINPUT = true;
		}

	}

  static protected ControllerManager _manager;
  static public ControllerManager getInputManager(){ return _manager; }
	
}
