var Letter : GameObject;//leave empty, the script will assign it
var Image : Texture2D;//leave empty, the script will assign it
var ShowLetter : boolean;//don't touch :)
var LetterMask : LayerMask;//select the layer of your letter object in this drop-down list
var cam : Transform;
public var hasKey : boolean;
cam = Camera.main.transform;
 
function Update()
{
    var ray = Ray(cam.position, cam.forward);
    var hit : RaycastHit;//stores information about things we hit with our raycast
 if( Input.GetButtonDown("grab") || Input.GetKeyDown(KeyCode.E))

    {
         if (Physics.Raycast (ray, hit, Mathf.Infinity, LetterMask))
         {

          Letter = hit.collider.gameObject;//assigns the letter object that was hit by the raycast to this script's Letter variable so we can access it, should we want to pick it up.
       //   Image = hit.collider.gameObject.GetComponent(LetterScript).LetterImage;//assign the LetterImage from the letter object that was hit to this script's Image variable so it can be accessed in OnGUI()
         
           if(Letter.gameObject.tag == "Key"){


          Letter.transform.parent = transform;//make the letter a child of the player so it stays with him. It's not really needed though.
          Letter.transform.localScale -= Vector3(0.9,0.9,0.9);
          Letter.transform.position = transform.position + transform.forward * 10;//put the letter at our player's location.
          Letter.collider.enabled = false;//disable the Letter object's collider, so it doesn't interfere with any raycasting we do.
          
          	hasKey = true;
          }
          }
         
     //  }
    }
}
 
function OnGUI()
{
    if(ShowLetter == true)
    {
   //    GUI.DrawTexture (Rect (Screen.width / 2 - Image.width / 2, Screen.height / 2 - Image.height / 2, Image.width, Image.height), Image);
    }
 
}