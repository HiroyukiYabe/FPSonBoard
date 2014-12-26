static var whichRemote: int;

//private var searching = false;
var WiiObject: GameObject;
var Wii;
var Player: Transform;
var Input;

Wii = WiiObject.GetComponent("Wii");
Input = GetComponent("FPSInputController");

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
	
}

var totalRemotes = 0;


function OnDiscoveryError(i : int) {
	//searching = false;
} 

function OnWiimoteFound (thisRemote: int) {
	Debug.Log("found this one: "+thisRemote);
	if(!Wii.IsActive(whichRemote))
		whichRemote = thisRemote;
}

function OnWiimoteDisconnected (whichRemote: int) {
}

function Update () {


	if(Wii.IsActive(whichRemote))
	{		
		var inputDisplay = ""; 
		inputDisplay = inputDisplay + "Remote #"+whichRemote.ToString();
		inputDisplay = inputDisplay + "\nbattery "+Wii.GetBattery(whichRemote).ToString();
		
		if(Wii.GetExpType(whichRemote)==3)//balance board is in is in
		{
			
			var theBalanceBoard = Wii.GetBalanceBoard(whichRemote); 
			//var theCenter = Wii.GetCenterOfBalance(whichRemote); // bug
			
		    var vecTopRight : Vector2 = Vector2(Mathf.Sin(Mathf.PI/4.0),Mathf.Cos(Mathf.PI/4.0)) * theBalanceBoard.x;
			var vecTopLeft : Vector2 = Vector2(Mathf.Sin(Mathf.PI/4.0),-1.0 * Mathf.Cos(Mathf.PI/4.0)) * theBalanceBoard.z;
			var vecBottomLeft :Vector2 = Vector2(-1.0 * Mathf.Sin(Mathf.PI/4.0),-1.0 * Mathf.Cos(Mathf.PI/4.0)) * theBalanceBoard.w;
			var vecBottomRight : Vector2 = Vector2(-1.0 * Mathf.Sin(Mathf.PI/4.0),Mathf.Cos(Mathf.PI/4.0)) * theBalanceBoard.y;
			var center : Vector2 = vecTopRight + vecTopLeft + vecBottomRight + vecBottomLeft;
			//Player.Translate(Vector3(center.x/100.0,0,center.y/100.0));
			Input.directionVector = Vector3(center.x*5,0,center.y*5);
		
		}
	}
	
}
	