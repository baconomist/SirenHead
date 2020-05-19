using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SirenHeadAnimEventReceiver : MonoBehaviour
{
    public static event Action OnFootstep;
    public static event Action OnPlayerEaten;
    
    // Defined in animation clip
    void Footstep()
    {
        OnFootstep?.Invoke();
    }

    void PlayerEaten()
    {
        OnPlayerEaten?.Invoke();
    }
}
