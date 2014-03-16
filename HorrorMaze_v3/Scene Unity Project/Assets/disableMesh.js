#pragma strict
var door : GameObject;
var sunSky : GameObject;
var darkSky : GameObject;


function Start () {
	door = GameObject.Find("Door");
	sunSky = GameObject.Find("Sphere_background");
	darkSky = GameObject.Find("Sphere_background - Copy");
}

function Update () {
	var doorDist = Vector3.Distance(door.transform.position, transform.position);

	if(doorDist < 70){
		//sunSky.renderer.enabled = false;
		darkSky.renderer.enabled = false;
	}else{
		//sunSky.renderer.enabled = true;
		darkSky.renderer.enabled = true;
	}
}