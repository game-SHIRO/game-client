using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //地面
    bool isGround;

    // カメラ
    public Transform cameraTransform;

    //カメラ感度
    public float rotationSpeed = 10f;

    // 移動速度
    public float walkSpeed = 5f;

    private float h;
    private float v;
    private float currentSpeed;
    private Vector3 moveDirection;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = walkSpeed;

        Debug.Log(rb);

        if (rb == null)
        {
            Debug.LogError("Rigidbodyが見つからない（Playerに付いていない or 別オブジェクト）");
        }
    }

    void Update()
    {
        //　WASD入力判定
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * v + right * h;
        moveDirection = move;

        if (move.magnitude > 1f){
            move.Normalize();
        }

        //移動
        rb.linearVelocity = new Vector3(
            move.x * currentSpeed,
            rb.linearVelocity.y,
            move.z * currentSpeed
        );
    }
}