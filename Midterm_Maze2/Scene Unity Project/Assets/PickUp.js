var Letter : GameObject;//leave empty, the script will assign it
var Image : Texture2D;//leave empty, the script will assign it
var ShowLetter : boolean;//don't touch :)
var LetterMask : LayerMask;//select the layer of your letter object in this drop-down list
var cam : Transform;

cam = Camera.main.transform;
 
function Update()
{
    var ray = Ray(cam.position, cam.forward);
    var hit : RaycastHit;//stores information about things we hit with our raycast
 
    if(Input.GetButtonDown("grab"))
    {
       if(ShowLetter == false)
       {
         if (Physics.Raycast (ray, hit, Mathf.Infinity, LetterMask))
         {
          Letter = hit.collider.gameObject;//assigns the letter object that was hit by the raycast to this script's Letter variable so we can access it, should we want to pick it up.
          Image = hit.collider.gameObject.GetComponent(LetterScript).LetterImage;//assign the LetterImage from the letter object that was hit to this script's Image variable so it can be accessed in OnGUI()
          ShowLetter = true;
         }
       }
       else
       {
         ShowLetter = false;//if the letter image is already being shown, hide it again.
         //here, you could also make the player pick up the letter object by making it a child of himself:
         if(Letter != null)//just to be sure, check if the Letter variable isn't empty
         {
         
          Letter.transform.parent = transform;//make the letter a child of the player so it stays with him. It's not really needed though.
          Letter.transform.localScale -= Vector3(0.9,0.9,0.9);
          Letter.transform.position = transform.position + transform.forward * 0.4;//put the letter at our player's location.
          Letter.renderer.enabled = true;//make to Letter object invisible, we don'T want to see it while carrying it, right?
          Letter.collider.enabled = false;//disable the Letter object's collider, so it doesn't interfere with any raycasting we do.
         }
       }
    }
}
 
function OnGUI()
{
    if(ShowLetter == true)
    {
       GUI.DrawTexture (Rect (Screen.width / 2 - Image.width / 2, Screen.height / 2 - Image.height / 2, Image.width, Image.height), Image);
    }
 
}