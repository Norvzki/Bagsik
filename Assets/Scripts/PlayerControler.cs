using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator _animator;
    float Velocity = 0.0f;
    public float acceleration = 0.1f;
    public float deceleration = 0.5f;
    int VelocityHash;

    void Start()
    {
        // Ensure character starts above ground
        _animator = GetComponent<Animator>();
        Debug.Log(_animator);

        VelocityHash = Animator.StringToHash("Velocity");
    }

    void Update()
    {
        bool isrunning =  _animator.GetBool("RunRun");
        bool walking = Input.GetKey("w");
        bool Runingpress = Input.GetKey("left shift");
       
        if (walking && Velocity < 0.5f)
        {
          Velocity += Time.deltaTime * acceleration;    
        }

        _animator.SetFloat(VelocityHash, Velocity);

        if (!walking && Velocity > 0.0f)
        {
          Velocity -= Time.deltaTime * deceleration;
        }
        if (!walking && Velocity < 0.0f)
        {
          Velocity = 0.0f;
        }
        if (walking && Runingpress)
        {
            _animator.SetBool("RunRun", true);
        }
        if(!walking || !Runingpress)
        {
            _animator.SetBool("RunRun", false);
        }

    }

    

}