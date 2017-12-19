using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Manager will only update events on joystickNames[] size change
 * */

public class ControllerManager : MonoBehaviour {

  static public bool XINPUT; // defined at runtime

  public const int MAX_CONTROLLER = 4;
	public int connectedCount = 0; // valeur qui varie en fonction de l'event de connection/deco

  protected Controller360[] controllers;
  protected Controller360[] tempControllers;

  protected Action<int> onControllerPlugged;
  protected Action<int> onControllerUnplugged;
  
  virtual protected void Awake()
  {
    ControllerManager[] cms = GameObject.FindObjectsOfType<ControllerManager>();
    if (cms.Length > 1)
    {
      Debug.LogError("multiple controller manager in scene");
      return;
    }

    manager = this;

    tempControllers = new Controller360[MAX_CONTROLLER];
    controllers = new Controller360[MAX_CONTROLLER];

    updateControllers();
  }
  
  public void subscribeToEvents(Action<int> plug, Action<int> unplug) {
    onControllerPlugged += plug;
    onControllerUnplugged += unplug;

    Debug.Log(GetType()+" "+name+" | someone subscribed to controller plugging events", gameObject);
    Debug.Log(onControllerPlugged);
    Debug.Log(onControllerUnplugged);
  }

  void Update()
  {
    int count = getSystemConnectedCount();
    if (count != connectedCount)
    {
      //Debug.LogError("<ControllerManager> joystick count changed from "+connectedCount+" to "+Input.GetJoystickNames().Length);
      connectedCount = count;

      updateControllers();
    }
  }

  virtual protected int getSystemConnectedCount(){
    return Input.GetJoystickNames().Length;
  }
  
	virtual public void updateControllers(){
		
    //POUR MAC
    //le nombre de joystick sous Unity != du nombre de joystick concidéré comme "connected" dans le framework
    //parce que le framework part du principe qu'un joystick doit avoir reçu une commande de l'user pour etre "connected"

    //update number
    connectedCount = Input.GetJoystickNames().Length;

    Controller360 c = null;
		for(int i = 0; i < MAX_CONTROLLER; i++){
			GameObject obj = GameObject.Find("controller-"+i);
      tempControllers[i] = null;

			if(i < connectedCount){

        if(obj == null){
					c = create(i);
					event__controllerPlugged(i);
          Debug.Log("<ControllerManager> adding missing controller bridge for index "+i);
				}else{
          c = obj.GetComponent<Controller360>();
        }

        tempControllers[i] = c;
			}else{
				if(obj != null){
					GameObject.DestroyImmediate(controllers[i].gameObject);
					event__controllerUnplugged(i);
				}
			}
		}
		
    //Debug.Log("controller update, list is "+list.Count+" long");
    updateControllerArray();
	}

  void debug__displayControllerContent(){
    for(int i = 0; i < controllers.Length; i++){
      if(controllers[i] == null)  Debug.Log("controller of index "+i+" is null");
      else Debug.Log("controller of index "+i+" has controllerId of "+controllers[i].getControllerId());
    }
  }

  /* assign les manettes dans les cases du manager */
  virtual protected void updateControllerArray(){
    // sur le manager de base les manettes sont rangées dans l'ordre
    // sous xinput les manettes sont dans les cases correspondantes aux ids

    for(int i = 0; i < tempControllers.Length; i++){
      if(i >= tempControllers.Length) controllers[i] = null;
      else controllers[i] = tempControllers[i];
    }

  }

	public void event__controllerPlugged(int controllerId){
    Debug.Log("<color=orange>" + GetType() + "</color> | event__controller<b>Plugged</b> #" + controllerId);
    //updateControllers();
    
    if (onControllerPlugged != null) onControllerPlugged(controllerId);
	}

  public void event__controllerUnplugged(int controllerId){
    Debug.Log("<color=orange>"+GetType()+"</color> | event__controller<b>Unplugged</b> #" + controllerId);
    updateControllers();
    
    if(onControllerUnplugged != null) onControllerUnplugged(controllerId);
	}

  virtual public Controller360 getPrimaryController(){
    for (int i = 0; i < controllers.Length; i++) {
      if(controllers[i] != null) return controllers[i];
    }
    Debug.LogWarning("no primary controller found");
    return null;
  }

  public Controller360 getControllerById(int id){
    return controllers[id];
  }

	virtual public Controller360[] getControllers(){
		updateControllers();
		return controllers;
	}

	virtual public bool anyPressedSkip(){
		if(connectedCount < 1)	return false;

		for(int i = 0; i < controllers.Length; i++){
			Controller360 c = controllers[i];
			if(c == null) continue;
			//ConsoleSurround.add("checking "+c.name);
      if(c.isReleased(Controller360.ControllerButtons.START)) return true;
			if(c.isReleased(Controller360.ControllerButtons.A)) return true;
		}
		return false;
	}

	virtual public bool anyPressedBack(){
		if(Input.GetKeyUp(KeyCode.Escape))	return true;
		if(controllers.Length < 1)	return false;
		for(int i = 0; i < controllers.Length; i++){ if(controllers[i] == null) continue; if(controllers[i].isReleased(Controller360.ControllerButtons.BACK)) return true; }
		return false;
	}

	virtual public bool anyPressedStart(){
		if(controllers.Length < 1)	return false;
		for(int i = 0; i < controllers.Length; i++){ if(controllers[i] != null){ if (controllers[i].isReleased(Controller360.ControllerButtons.START)) return true; } }
		return false;
	}
	
	virtual public bool anyPressedA(){
		if(controllers.Length < 1)	return false;
		
		foreach(Controller360 c in controllers){
			if(c != null){
        //Debug.Log("checking "+c.name);
        if (c.isReleased(Controller360.ControllerButtons.A)) return true;
			}
		}
		return false;
	}

	// create new controller
	virtual public Controller360 create(int id){
		Controller360 control = null;
		if(GameObject.Find("controller-"+id) == null){
			GameObject obj = new GameObject("controller-"+id);
			control = obj.AddComponent<Controller360>();
		}
		//Debug.Log("Created controller "+id);
		return control;
	}

  /*
	virtual public int countConnected(){
		int count = 0;
		for(int i = 0; i < controllers.Length; i++){
			Controller360 c = controllers[i];
			if(c == null)	continue;
			if(c.isConnected())	count++;
		}
		return count;
	}
  */

	virtual public int countRefInArray(){
		int count = 0;
    //Debug.Log("checking ref in array, controllers array length is : "+controllers.Length);
		for(int i = 0; i < controllers.Length; i++){
			Controller360 c = controllers[i];
			if(c == null){
        //Debug.Log(i+" is null");
        continue;
      }
			count++;
		}
		return count;
	}

  virtual public int getMaxControllerCount() { return MAX_CONTROLLER; }

  static protected ControllerManager manager;

  static public bool isMac(){
    if(Application.platform == RuntimePlatform.OSXEditor) return true;
    if(Application.platform == RuntimePlatform.OSXPlayer) return true;
    return false;
  }
  static public bool isWindows(){
    if(Application.platform == RuntimePlatform.WindowsEditor) return true;
    if(Application.platform == RuntimePlatform.WindowsPlayer) return true;
    return false;
  }


}
