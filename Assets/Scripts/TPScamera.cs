using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 200f;
    public float xRotation = 0f;
    public float yRotation = 0f;


    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        transform.localPosition = new Vector3(0, 1.6f, 0);
    }

    void Update(){
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation,-80f,80f);

        transform.rotation = Quaternion.Euler(xRotation,yRotation,0);

    }
}
