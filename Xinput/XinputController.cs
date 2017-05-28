using UnityEngine;
using System.Collections;
using XInputDotNetPure;

/*
 * http://speps.fr/xinputdotnet
 * */

public class XinputController : Controller360 {
	
	GamePadState gamepadState;
  PlayerIndex xinputIndex;
	
	override protected void Start () {


		base.Start ();

		string[] split = name.Split('-');
		controllerIndex = int.Parse(split[1]);
		controllerId = controllerIndex;
		xinputIndex = (PlayerIndex)controllerIndex;

    ControllerManager i_manager = XinputControllerManager.getXinputManager();
    if(i_manager != null){
      i_manager.event__controllerPlugged(getControllerId());
    }

    //Debug.Log("<XinputC> Start() idx "+controllerIndex+", id "+controllerId+", xinput "+xinputIndex);
	}
	
  override public bool isConnected(){
    gamepadState = GamePad.GetState(xinputIndex, XInputDotNetPure.GamePadDeadZone.Circular);
    return gamepadState.IsConnected;
  }

	override protected void Update () {
    gamepadState = GamePad.GetState(xinputIndex, XInputDotNetPure.GamePadDeadZone.Circular);

    if(!gamepadState.IsConnected){
      event__controllerUnplugged();
      return;
    }

    updateControllerInfo(); // update everything
	}
	
  override public int getControllerId(){ return getXinputIndex(); }
  public int getXinputIndex(){ return (int)xinputIndex; }

	override protected void updateTriggers(){
		
		leftTrigger = gamepadState.Triggers.Left;
		rightTrigger = gamepadState.Triggers.Right;

    updateAnyTriggers();
	}
	
	override protected void updateButtons(){
		
		updateKeyState(Controller360.LB, gamepadState.Buttons.LeftShoulder == ButtonState.Pressed);
		updateKeyState(Controller360.RB, gamepadState.Buttons.RightShoulder == ButtonState.Pressed);
		updateKeyState(Controller360.A, gamepadState.Buttons.A == ButtonState.Pressed);
		updateKeyState(Controller360.X, gamepadState.Buttons.X == ButtonState.Pressed);
		updateKeyState(Controller360.Y, gamepadState.Buttons.Y == ButtonState.Pressed);
		updateKeyState(Controller360.B, gamepadState.Buttons.B == ButtonState.Pressed);
		
		updateKeyState(Controller360.START, gamepadState.Buttons.Start == ButtonState.Pressed);
		updateKeyState(Controller360.BACK, gamepadState.Buttons.Back == ButtonState.Pressed);
	}
	
  override protected void updateDpad(){
    dPad[0] = 0;
    dPad[1] = 0;

    //all else if because dpad precision is baaaad
    if(gamepadState.DPad.Left == ButtonState.Pressed) dPad[0] = -1;
    else if(gamepadState.DPad.Right == ButtonState.Pressed) dPad[0] = 1;
    if(gamepadState.DPad.Up == ButtonState.Pressed) dPad[1] = 1;
    else if(gamepadState.DPad.Down == ButtonState.Pressed) dPad[1] = -1;

    dPadVector.x = dPad[0];
    dPadVector.z = dPad[1];
  }

	override protected void updateSticks(){
		previousRightStickVector = rightStickVector;
		leftStick[0] = gamepadState.ThumbSticks.Left.X;
		leftStickVector.x = leftStick[0];
		leftStick[1] = gamepadState.ThumbSticks.Left.Y;
		leftStickVector.z = -leftStick[1];
		
		rightStick[0] = gamepadState.ThumbSticks.Right.X;
		rightStickVector.x = rightStick[0];
		rightStick[1] = gamepadState.ThumbSticks.Right.Y;
		rightStickVector.z = -rightStick[1];
		
		//Debug.Log(Input.GetAxis("L_XAxis_"+controllerId));
	}
	
	void updateKeyState(int keyIndex, bool keyState){
		bool current = keyState;
		
		if(pressed[keyIndex])	pressed[keyIndex] = false;
		if(released[keyIndex])	released[keyIndex] = false;
		
		if(current && !state[keyIndex]){
			//Debug.Log(name+", "+map[i]+" PRESSED");
			pressed[keyIndex] = true;
		}else if(state[keyIndex] && !current){
			//Debug.Log(name+", "+map[i]+" RELEASED");
			released[keyIndex] = true;
		}
		state[keyIndex] = current;
	}
	
	override protected void OnGUI(){
    bool draw = false;

#if UNITY_EDITOR
    if(UnityEditor.Selection.activeGameObject == gameObject) draw = true;
    if(drawDebug)  draw = true;
#endif

    if(!draw) return;

		string content = name+" -- ControllerID="+controllerIndex;
		if(gamepadState.IsConnected){
			content += "\nLS="+leftStickVector+" ("+leftStickVector.magnitude+")";
			content += "\nRS="+rightStickVector+" ("+rightStickVector.magnitude+")";
			//content += "\nDeadZone="+gamepadState;

			content += "\nA="+state[A]+","+pressed[A]+","+released[A];
			content += "\nB="+state[B]+","+pressed[B]+","+released[B];
			content += "\nX="+state[X]+","+pressed[X]+","+released[X];
			content += "\nY="+state[Y]+","+pressed[Y]+","+released[Y];
			content += "\nLB="+state[LB]+","+pressed[LB]+","+released[LB];
			content += "\nRB="+state[RB]+","+pressed[RB]+","+released[RB];

			content += "\nLT="+leftTrigger;
			content += "\nRT="+rightTrigger;

			content += "\nSTART="+state[START]+","+pressed[START]+","+released[START];
			content += "\nBACK="+state[BACK]+","+pressed[BACK]+","+released[BACK];
		}else{
			content += "\nController is not connected";
		}
		
		GUI.TextArea(new Rect((int)controllerIndex * 250f, 5f, 250f, 200f), content);
	}
	
	static public XinputController add(int index){
		GameObject obj = GameObject.Find("controller-"+index);
		if(obj == null)	obj = new GameObject("controller-"+index);
		else return obj.GetComponent<XinputController>();

    Debug.Log("<color=gray>XinputController | add xinput controller # "+index+"</color>");
		return obj.AddComponent<XinputController>();
	}
}
