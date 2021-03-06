﻿ static var whichRemote: int;

//var theIRMain: GUITexture;
//var theIR1:    GUITexture;
//var theIR2:    GUITexture;
//var theIR3:    GUITexture;
//var theIR4:    GUITexture;

//private var searching = false;
var WiiObject: GameObject;
var Wii;
//var player: Transform;
var assaultRifle: Transform;
var input;

public var buttonBPressed: boolean;
public var buttonRightPressed: boolean;
public var buttonLeftPressed: boolean; 

Wii = WiiObject.GetComponent("Wii");
input = GetComponent("FPSInputController");

function setVisibility(t : Transform , v : boolean)
{
	if (t.renderer && t.renderer.enabled != v)
	{
		t.renderer.enabled = v;
	}
	renderers = t.GetComponentsInChildren (Renderer);
	for (var rendy : Renderer in renderers) {
    	rendy.enabled = v;
	}
}

function Start () {
	buttonBPressed = false;
}

var totalRemotes = 0;

function OnDiscoveryError(i : int) {
}

function OnWiimoteFound (thisRemote: int) {
	Debug.Log("found this one: "+thisRemote);
	if(!Wii.IsActive(whichRemote))
		whichRemote = thisRemote;
}

function OnWiimoteDisconnected (whichRemote: int) {
	Debug.Log("lost this one: "+ whichRemote);
}

function Update () {
	var whichRemote : int = 1;

if(Wii.IsActive(whichRemote))
	{
		var inputDisplay = "";
		inputDisplay = inputDisplay + "Remote #"+whichRemote.ToString();
		inputDisplay = inputDisplay + "\nbattery "+Wii.GetBattery(whichRemote).ToString();

		if(Wii.GetExpType(whichRemote)==3)//balance board is in is in
		{
		}
		else
		{
			///WIIREMOTE
			pointerArray = Wii.GetRawIRData(whichRemote);
			mainPointer = Wii.GetIRPosition(whichRemote);
			wiiAccel = Wii.GetWiimoteAcceleration(whichRemote);

//			theIRMain.pixelInset = Rect(mainPointer.x*Screen.width-25.0f,mainPointer.y*Screen.height-25.0f,50.0,50.0);
//			var sizeScale = 5.0f;
//
//			theIR1.pixelInset = Rect (pointerArray[0].x*Screen.width-(pointerArray[0].z*sizeScale/2.0f),
//			 							pointerArray[0].y*Screen.height-(pointerArray[0].z*sizeScale/2.0f),
//			 							pointerArray[0].z*sizeScale*10, pointerArray[0].z*sizeScale*10);
//
//			theIR2.pixelInset = Rect (pointerArray[1].x*Screen.width-(pointerArray[1].z*sizeScale/2.0f),
//										pointerArray[1].y*Screen.height-(pointerArray[1].z*sizeScale/2.0f),
//										pointerArray[1].z*sizeScale*10, pointerArray[1].z*sizeScale*10);
//
//			theIR3.pixelInset = Rect (pointerArray[2].x*Screen.width-(pointerArray[2].z*sizeScale/2.0f),
//										pointerArray[2].y*Screen.height-(pointerArray[2].z*sizeScale/2.0f),
//										pointerArray[2].z*sizeScale*10, pointerArray[2].z*sizeScale*10);
//
//			theIR4.pixelInset = Rect (pointerArray[3].x*Screen.width-(pointerArray[3].z*sizeScale/2.0f),
//										pointerArray[3].y*Screen.height-(pointerArray[3].z*sizeScale/2.0f),
//										pointerArray[3].z*sizeScale*10, pointerArray[3].z*sizeScale*10);

//
//			wiimote.localRotation = Quaternion.Slerp(transform.localRotation,
//				Quaternion.Euler(wiiAccel.y*90.0, 0.0,wiiAccel.x*-90.0),5.0);
			if(Wii.GetButton(whichRemote, "B")){
				Debug.Log("B pressed");
				buttonBPressed = true;
			} else {
				buttonBPressed = false;
			}
			
			if(Wii.GetButton(whichRemote, "RIGHT")){
				Debug.Log("Right pressed");
				buttonRightPressed = true;
			} else {
				buttonRightPressed = false;
			}
			
			if(Wii.GetButton(whichRemote, "LEFT")){
				Debug.Log("Left pressed");
				buttonLeftPressed = true;
			} else {
				buttonLeftPressed = false;
			}
			
//			assaultRifle.localRotation = Quaternion.Slerp(transform.localRotation,
//				Quaternion.Euler(/* tate*/wiiAccel.y*90.0,/*yoko*/0.0 ,0.0),5.0);   /*tate*/
			 if (Wii.GetIRPosition(whichRemote).x > -1.0){
//			 assaultRifle.localRotation = Quaternion.Slerp(transform.localRotation,
//				Quaternion.Euler(/* tate*/ wiiAccel.y*90.0,/*yoko*/(Wii.GetIRPosition(whichRemote).x - 0.5 )*180.0 ,0.0),5.0);   /*yoko*/
			 assaultRifle.localRotation = Quaternion.Slerp(transform.localRotation,
				Quaternion.Euler(/* tate*/ (Wii.GetIRPosition(whichRemote).y - 0.5) *  -180.0,/*yoko*/(Wii.GetIRPosition(whichRemote).x - 0.5 )*180.0 ,0.0),5.0);   /*yoko*/

			//Debug.Log(wiiAccel.x);
			 }
			

			
			//Debug.Log(Wii.GetIRPosition(whichRemote).ToString("#.0000"));
			//Debug.Log(Wii.GetWiimoteAcceleration(whichRemote).ToString("#.0000"));

			inputDisplay = inputDisplay + "\nIR      "+Wii.GetIRPosition(whichRemote).ToString("#.0000");
			inputDisplay = inputDisplay + "\nIR rot  "+Wii.GetIRRotation(whichRemote).ToString();
			inputDisplay = inputDisplay + "\nA       "+Wii.GetButton(whichRemote,"A").ToString();
			inputDisplay = inputDisplay + "\nB       "+Wii.GetButton(whichRemote,"B").ToString();
			inputDisplay = inputDisplay + "\n1       "+Wii.GetButton(whichRemote,"1").ToString();
			inputDisplay = inputDisplay + "\n2       "+Wii.GetButton(whichRemote,"2").ToString();
			inputDisplay = inputDisplay + "\nUp      "+Wii.GetButton(whichRemote,"UP").ToString();
			inputDisplay = inputDisplay + "\nDown    "+Wii.GetButton(whichRemote,"DOWN").ToString();
			inputDisplay = inputDisplay + "\nLeft    "+Wii.GetButton(whichRemote,"LEFT").ToString();
			inputDisplay = inputDisplay + "\nRight   "+Wii.GetButton(whichRemote,"RIGHT").ToString();
			inputDisplay = inputDisplay + "\n-       "+Wii.GetButton(whichRemote,"-").ToString();
			inputDisplay = inputDisplay + "\n+       "+Wii.GetButton(whichRemote,"+").ToString();
			inputDisplay = inputDisplay + "\nHome    "+Wii.GetButton(whichRemote,"HOME").ToString();
			inputDisplay = inputDisplay + "\nAccel   "+Wii.GetWiimoteAcceleration(whichRemote).ToString("#.0000");

			if(Wii.HasMotionPlus(whichRemote))
			{
			}
			else
			{
			}

			if(Wii.GetExpType(whichRemote)==1)//nunchuck is in
			{
			}
			else if(Wii.GetExpType(whichRemote)==2)//classic controller is in
			{
			}
			else if(Wii.GetExpType(whichRemote)==4)//guitar is in
			{
			}
			else if(Wii.GetExpType(whichRemote)==5)//drums are in
			{
			}
			else if(Wii.GetExpType(whichRemote)==6)//turntable is in
			{
			}
			else if(Wii.GetExpType(whichRemote)==0)
			{
			}
		}
	}
	else
	{
	}
}
