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

    //移動
    private float h;
    private float v;
    private Vector3 moveDirection;

    // 移動速度
    public float crouchSpeed = 2.5f;
    public float walkSpeed = 5f;
    public float dashSpeed = 10f;

    private float currentSpeed;
    private bool isCrouch;
    private bool isDash;

    //アクション
    public float jumpPower = 5f;
    public float rollDuration = 0.5f;
    public float rollSpeed = 15f;
    
    private bool isJump;
    private bool isRoll = false;
    private float rollTimer;
    private Vector3 rollDirection;

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

        //移動速度判定
        isCrouch = Input.GetKey(KeyCode.LeftControl);
        isDash = Input.GetKey(KeyCode.LeftShift);

        if(isDash){//ダッシュ時
            currentSpeed = dashSpeed;
        } else if(isCrouch){//しゃがんでる時
            currentSpeed = crouchSpeed;
        } else {//歩いてる時
            currentSpeed = walkSpeed;
        }

        //ジャンプ判定
        if (Input.GetKeyDown(KeyCode.Space) && isGround){
            isJump = true;
        }

        if(Input.GetKeyDown(KeyCode.C) && !isRoll){
            StartRoll();
        }
    }

    void FixedUpdate(){
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // カメラ基準の移動方向
        Vector3 move = forward * v + right * h;
        moveDirection = move;

        if (move.magnitude > 1f){
            move.Normalize();
        }

        //移動
        if(isRoll){
            rb.linearVelocity = new Vector3(
                rollDirection.x * rollSpeed,
                rb.linearVelocity.y,
                rollDirection.z * rollSpeed
            );

            rollTimer -= Time.fixedDeltaTime;

            if(rollTimer <= 0){
                isRoll = false;
            }
        } else {
            rb.linearVelocity = new Vector3(
                move.x * currentSpeed,
                rb.linearVelocity.y,
                move.z * currentSpeed
            );
        }

        //プレイヤーを移動方向へ回転
        if (move != Vector3.zero){
            Quaternion targetRotation = Quaternion.LookRotation(move);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );
        }

        //ジャンプ動作
        if (isJump && isGround){
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGround = false;
        }

        isJump = false;
    }

    void StartRoll(){

        if(isRoll){
            return;
        }
        
        isRoll = true;
        rollTimer = rollDuration;

        if(moveDirection != Vector3.zero){
            rollDirection = moveDirection.normalized;
        } else {
            rollDirection = transform.forward;
        }
    }

    //地面に設置してるか判定
    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.CompareTag("Ground")){
            isGround = true;
        }
    }

    void OnCollisionStay(Collision collision){
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