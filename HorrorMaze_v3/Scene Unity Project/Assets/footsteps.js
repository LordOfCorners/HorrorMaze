var AudioFile : AudioClip;

function Update () {
	if ( Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.D) )
	{
	    audio.clip = AudioFile;
	    audio.Play();
	 
	}
	
	if ( Input.GetKeyUp (KeyCode.W) || Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp (KeyCode.S) || Input.GetKeyUp (KeyCode.D) )
	{
    audio.clip = AudioFile;
    audio.Stop();
	}
}