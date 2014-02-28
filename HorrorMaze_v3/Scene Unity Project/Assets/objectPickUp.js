#pragma strict
public var pickUpBool : boolean;


function Start () {
	pickUpBool = false;
}

function Update () {

}

function OnTriggerEnter (other : Collider){
	if (other.gameObject.tag == "Player") {
		pickUpBool = true;
	}
}