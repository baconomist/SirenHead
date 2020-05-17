using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SirenHeadAnimEventReceiver : MonoBehaviour
{
    public static event Action OnFootstep;
    
    // Defined in animation clip
    void Footstep()
    {
        OnFootstep?.Invoke();
    }
}
