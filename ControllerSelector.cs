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
    
    if (ControllerManager.isMac()){
			Debug.Log("<ControllerSelector> Is under MACOS, using unity inputManager");
      _manager = getManager<ControllerManager>("[input]");
			
      ControllerManager.XINPUT = false;

    }else if(ControllerManager.isWindows()){
      Debug.Log("<color=orange>ControllerSelector</color> | Is under WINDOWS, <b>using xinput dll</b>");
      _manager = getManager<XinputControllerManager>("[input]");
      
      ControllerManager.XINPUT = true;
		}

	}

  static protected ControllerManager _manager;
  static public ControllerManager getInputManager(){ return _manager; }
	
  static public T getManager<T>(string nm) where T : ControllerManager{
    GameObject obj = GameObject.Find(nm);
    T tmp = null;
    if (obj != null){
      tmp = obj.GetComponent<T>();
    }

    if (tmp != null) return tmp;

    if (obj == null)
    {
      obj = new GameObject(nm, typeof(T));
      tmp = obj.GetComponent<T>();
    }
    else tmp = obj.AddComponent<T>();

    return tmp;
  }

}
