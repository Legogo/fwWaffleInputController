using UnityEngine;
using XInputDotNetPure;

/*
 * * Les controllers sont créer dans des GameObjects séparés "controller-[ID]"
 * * La création des index se fait dans l'ordre. Si il manque 2 entre 1 et 3, 2 sera recréé avant 4
 * 
 * 2012-12-23
 * Les ids des manettes correspondent à l'ordre de branchement
 * Il faut attendre un input de la manette pour être certains de l'id
 * 
 * http://forum.unity3d.com/threads/114993-Joystick-detection-and-direct-axis-detection
 * */

public class XinputControllerManager : ControllerManager {

  static public XinputControllerManager _input_xinput;

  protected override void Awake()
  {
    base.Awake();
    _input_xinput = this;
  }
  
  /* au changement de scène les controller-X dégagent et sont recréé */
  override public void updateControllers(){

    Controller360 c = null;
    GameObject obj = null;
    for(int i = 0; i < MAX_CONTROLLER; i++){
      tempControllers[i] = null;
      GamePadState gamepadState;

      try
      {
        gamepadState = GamePad.GetState((PlayerIndex)i);

        if (gamepadState.IsConnected)
        {
          //Debug.Log("xinput pad "+i+" is connected");
          obj = GameObject.Find("controller-" + i);
          if (obj == null)
          {
            //Debug.Log("<XinputManager> pad of index "+i+" is connected but has no bridge, creating one");
            c = createXinput(i);
            
            //event__controllerPlugged(i);
          }
          else
          {
            c = obj.GetComponent<Controller360>();
          }

          tempControllers[i] = c;
        }
      }
      catch
      {
        Debug.LogError("CATCH :: xinput :: cant get state for index " + i);
      }
      
    }
    
    //Debug.Log("controller update, list is "+list.Count+" long");
    updateControllerArray();
  }

	override public bool anyPressedStart(){
		if(controllers.Length < 1)	return false;
		
		foreach(XinputController c in controllers){
			if(c != null){
				if(c.isReleased(Controller360.ControllerButtons.START))	return true;
			}
		}
		return false;
	}
	
	override public bool anyPressedA(){
		if(controllers.Length < 1)	return false;
		
		foreach(XinputController c in controllers){
			if(c != null){
        //Debug.Log("checking "+c.name);
        if (c.isReleased(Controller360.ControllerButtons.A)) return true;
			}
		}
		return false;
	}

  public XinputController getControllerByXinputId(int id){
    //Debug.Log("getting xinput controller of id "+id);

    for(int i = 0; i < tempControllers.Length; i++){
      if(tempControllers[i] == null){
        //Debug.Log("listTempController (len:"+list.Count+") index:"+i+" is null"); 
        continue;
      }

      XinputController c = (XinputController)tempControllers[i];
      if(c == null){
        //Debug.Log("controller "+i+" is null");
        continue;
      }

      //Debug.Log("checking for xinput controller with id "+id+", this one is "+c.getXinputIndex());
      if(c.getXinputIndex() == id){
        //Debug.Log("found xinput of id "+c.getXinputIndex());
        return c;
      }
    }
    return null;
  }

  override protected void updateControllerArray(){
    for(int i = 0; i < tempControllers.Length; i++){
      controllers[i] = getControllerByXinputId(i);
    }
  }

  /* le nombre de la lib xinput */
  override protected int getSystemConnectedCount(){
    GamePadState state;
    int count = 0;
    for (int i = 0; i < MAX_CONTROLLER; i++)
    {
      state = (GamePadState)GamePad.GetState((PlayerIndex)i);
      if (state.IsConnected) count++;
    }
    return count;
  }

  /* le nombre de la lib ControllerManager */
  /*
	override public int countConnected(){
    Controller360[] controls = getControllers();
		int count = 0;
    for(int i = 0; i < controls.Length; i++){
      XinputController c = (XinputController)controls[i];
			if(c == null)	continue;
			if(c.isConnected())	count++;
		}
		return count;
	}
  */

  public XinputController createXinput(int index){
		XinputController controller = XinputController.add(index);
		//GamePadState state = GamePad.GetState((PlayerIndex)index);
		return controller;
	}

  static public XinputControllerManager getXinputManager(){
    if(manager == null) return null; // devrait instance le manager ...
    return _input_xinput;
  }
}
