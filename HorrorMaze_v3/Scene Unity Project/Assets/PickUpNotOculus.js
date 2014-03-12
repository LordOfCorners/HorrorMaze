public var hasKey : boolean;
var camRight : GameObject; 
var key : GameObject;
var carryObjectScript : objectPickUp;

function Start(){
	key = GameObject.FindWithTag("Key");
	carryObjectScript = key.GetComponent("objectPickUp");
	camRight = GameObject.Find("Main Camera");
}
 
function Update()
{
	if(carryObjectScript.pickUpBool ==true){ 
          key.transform.parent = camRight.transform;//make the letter a child of the player so it stays with him. It's not really needed though.
          key.transform.position = camRight.transform.position + camRight.transform.forward * 10;//put the letter at our player's location.
          key.collider.enabled = false;//disable the Letter object's collider, so it doesn't interfere with any raycasting we do.
          hasKey = true;
    }
}
