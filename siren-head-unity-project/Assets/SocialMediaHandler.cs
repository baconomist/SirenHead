using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialMediaHandler : MonoBehaviour
{
    public void OnTwitterClicked()
	{
		Application.OpenURL("https://twitter.com/BadBaboonGames");	
	}
	
	public void OnInstaClicked()
	{
		Application.OpenURL("https://www.instagram.com/badbaboongames/");
	}
}
