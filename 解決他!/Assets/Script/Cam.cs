using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public bool Lock_Cursor;
    public float Mouse_sensitivity = 10;
    public Transform Target;
    public float ds_From_Target = 2;
    public Vector2 Pitch_MinMix = new Vector2(-40, 85);

    public float Rotation_Smooth_time = .12f;
    Vector3 Rotation_smooth_Velocity;
    Vector3 Curren_Rotation;

    float Yaw;
    float Pitch;


    void Start()
    {
        //是否看見滑鼠
        if (Lock_Cursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
    void LateUpdate()
    {
        Yaw += Input.GetAxis("Mouse X") * Mouse_sensitivity;
        Pitch -= Input.GetAxis("Mouse Y") * Mouse_sensitivity;
        Pitch = Mathf.Clamp(Pitch, Pitch_MinMix.x, Pitch_MinMix.y);

        Curren_Rotation = Vector3.SmoothDamp(Curren_Rotation, new Vector3(Pitch, Yaw), ref Rotation_smooth_Velocity, Rotation_Smooth_time);

        Vector3 Target_rotation = new Vector3(Pitch, Yaw);
        transform.eulerAngles = Target_rotation;
        transform.position = Target.position - transform.forward * ds_From_Target;
    }


}
