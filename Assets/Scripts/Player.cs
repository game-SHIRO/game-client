using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //Unity内オブジェクト
    private Rigidbody rb;

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

    //アクション
    public float jumpPower = 5f;

    private bool isJump;

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

        //ジャンプ判定
        if (Input.GetKeyDown(KeyCode.Space) && isGround){
            isJump = true;
        }
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

        //ジャンプ動作
        if (isJump && isGround){
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGround = false;
        }

        isJump = false;
    }

    //地面に設置してるか判定
    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Ground")){
            isGround = true;
        }
    }

    void OnCollisionExit(Collision collision){
        if (collision.gameObject.CompareTag("Ground")){
            isGround = false;
        }
    }
}