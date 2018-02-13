using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movespeed;
    public float fastMoveSpeed;
    public float mouseSensativityY;
    public float mouseSensativityX;

    public int invertX;
    public int invertY;

    public float maxAngleUp;
    public float maxAngleDown;
    // Update is called once per frame

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float inputVertical = 0;

        if (Input.GetKey(KeyCode.Space))
        {
            inputVertical = 1;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            inputVertical = -1;
        }

        transform.Translate(new Vector3(input.x, inputVertical, input.y) * (Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : movespeed) * Time.deltaTime);
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));


        Vector3 newRotationCamera = transform.eulerAngles;
        newRotationCamera.x += Input.GetAxisRaw("Mouse Y") * mouseSensativityY * invertY * Time.deltaTime;
        newRotationCamera.y += Input.GetAxisRaw("Mouse X") * mouseSensativityX * invertX * Time.deltaTime;

        if (newRotationCamera.x > 180)
        {
            if (newRotationCamera.x < 360 - maxAngleUp)
            {
                //print("Too small");
                newRotationCamera.x = 360 - maxAngleUp;
            }
        }
        else
        {
            if (newRotationCamera.x > maxAngleDown)
            {
                //print("Too large");
                newRotationCamera.x = maxAngleDown;
            }
        }
        transform.eulerAngles = newRotationCamera;

        //transform.eulerAngles += new Vector3(mouseInput.y * mouseSensativity * invertY, mouseInput.x * mouseSensativity * invertX, 0) * Time.deltaTime;
    }
}
