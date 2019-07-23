using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public  Animator anim;
    public float Walk_Speed = 2;
    public float Run_Speed = 4;

    public float Turn_Smooth_Time = 0.2f;
    float Turn_Smooth_Velocity;

    public float Speed_Smooth_Time = 0.1f;
    float Speed_Smooth_Vlocity;
    float Current_Speed;

    Transform Camera_T;
    // Start is called before the first frame update
    void Start()
    {
        Camera_T = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 inputDir = input.normalized;

        if (inputDir != Vector2.zero)
        {
            float Target_Rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera_T.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, Target_Rotation, ref Turn_Smooth_Velocity, Turn_Smooth_Time);

        }

        bool Runnimg = Input.GetKey(KeyCode.LeftShift);
        float Target_Speed = ((Runnimg) ? Run_Speed : Walk_Speed) * inputDir.magnitude;
        Current_Speed = Mathf.SmoothDamp(Current_Speed, Target_Speed, ref Speed_Smooth_Vlocity, Speed_Smooth_Time);

        transform.Translate(transform.forward * Current_Speed * Time.deltaTime, Space.World);




        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            anim.SetBool("Walk", true);
        }



    }


}

