using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * http://answers.unity3d.com/questions/8019/how-do-i-use-an-xbox-360-controller-in-unity.html
 * http://wiki.unity3d.com/index.php?title=Xbox360Controller
 * */

public class ControllerTest : MonoBehaviour {
	
  public bool visible = false;
  protected GUIStyle style;

  protected ControllerManager manager;
  protected Controller360[] controls;

	void Start(){

    manager = ControllerSelector.getInputManager();

    controls = manager.getControllers();

    style = new GUIStyle();
    style.normal.textColor = Color.red;
    style.fontStyle = FontStyle.Bold;

#if !UNITY_EDITOR
    visible = false;
#endif

    //KeyManager.subscribeKey(KeyCode.I, "controller_test", onToggle).setupDescription("afficher les infos manette");
	}

  //protected void onToggle(Key key){ visible = !visible; }
	
	void OnGUI(){
		if(!visible)	return;

    string content = "[CONTROLLER TEST]";
    content += "\n  RAW UNITY JOYSTICK CONNECTED COUNT = "+Input.GetJoystickNames().Length;
    content += "\n  LIB CONTROLLER CONNECTED COUNT = " + manager.connectedCount;
    content += "\n  LIB CONTROLLER REF IN ARRAY COUNT = "+manager.countRefInArray();

		GUI.Label(new Rect(0,0,Screen.width,200), content, style);

		string output = "";

    Rect viewSize = new Rect();

    float offsetTop = 60f;
    viewSize.x = 0f;
    viewSize.y = 0f;
    viewSize.width = 250f;
    viewSize.height = 300f;

		for(int i = 0; i < controls.Length; i++){
			
			if(i == 0){
        viewSize.x = Screen.width * 0.25f - viewSize.width * 0.5f;
        viewSize.y = offsetTop;
			}else if(i == 1){
        viewSize.x = Screen.width * 0.75f - viewSize.width * 0.5f;
        viewSize.y = offsetTop;
			}else if(i == 2){
        viewSize.x = Screen.width * 0.25f - viewSize.width * 0.5f;
        viewSize.y = Screen.height * 0.25f + viewSize.height * 0.5f;
			}else if(i == 3){
        viewSize.x = Screen.width * 0.75f - viewSize.width * 0.5f;
        viewSize.y = Screen.height * 0.25f + viewSize.height * 0.5f;
			}
			
			output = "\nCONTROLLER #"+i;

			if(controls[i] == null)	continue;

			if(!controls[i].isConnected()){
				output += " IS NOT READY (controller id : "+controls[i].getControllerId()+")";
			}else{
        //output += " (index="+i+", InputAxisId:"+controls[i].getInputId()+") ";
        output += "\n"+((controls[i].isPrimary()) ? "PRIMARY" : "SECONDARY");
				output += "\nLS = "+controls[i].leftStick[0]+","+controls[i].leftStick[1];
				output += "\nRS = "+controls[i].rightStick[0]+","+controls[i].rightStick[1];
        output += "\nDPAD = "+controls[i].dPad[0]+","+controls[i].dPad[1];
        output += "\nLT = "+controls[i].leftTrigger;
				output += "\nRT = "+controls[i].rightTrigger;
				output += "\nLB = state:"+controls[i].isPressing(Controller360.ControllerButtons.LB);
				output += "\nRB = state:"+controls[i].isPressing(Controller360.ControllerButtons.RB);
        output += "\nA = state:" + controls[i].isPressing(Controller360.ControllerButtons.A);
        output += "\nB = state:" + controls[i].isPressing(Controller360.ControllerButtons.B);
        output += "\nSTART = state:" + controls[i].isPressing(Controller360.ControllerButtons.START);
        output += "\nBACK = state:" + controls[i].isPressing(Controller360.ControllerButtons.BACK);
			}
			
			GUI.Label(viewSize, output, style);
		}
		
	}
}
