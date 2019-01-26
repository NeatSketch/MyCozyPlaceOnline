using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Character character;

    void Start()
    {
        CharacterMovement.CanControl = true;
        CharacterMovement.SetCharacter(character);
        CharacterCamera.SetTarget(character);
    }

    void Update()
    {
        
    }
}
