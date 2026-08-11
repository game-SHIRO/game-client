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
    public float wallBoostSpeed = 10.0f;
    public float wallClimbSPeed = 5f;

    private float currentSpeed;
    private bool isCrouch;
    private bool isDash;

    //アクション
    public float jumpPower = 5f;
    public float rollDuration = 0.5f;
    public float rollSpeed = 15f;
    // public float wallDetachDistance = 0.5f;
    public float wallCheckDistance = 1.0f;
    public float wallBoostDistance = 2.0f;
    
    private bool isJump;
    private bool isRoll = false;
    private float rollTimer;
    private Vector3 rollDirection;

    private bool isWall;
    private bool isWallClimbing;
    private RaycastHit wallHit;
    private Vector3 wallBoostTarget;
    private bool isWallBoost;

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
        Vector3 rayOrigin = transform.position + Vector3.up * 1.0f;

        Debug.DrawRay(
            rayOrigin,
            transform.forward * wallCheckDistance,
            Color.red
        );

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
        if (
            isWallClimbing &&
            !isWallBoost &&
            Input.GetKeyDown(KeyCode.Space)
            ){
                StartWallBoost();
            } else if(Input.GetKeyDown(KeyCode.Space) && isGround){
            isJump = true;
        }

        if(Input.GetKeyDown(KeyCode.C) &&
            !isRoll &&
            !isWallClimbing){
                StartRoll();
            }

        //壁判定
        isWall = Physics.Raycast(
            rayOrigin,
            transform.forward,
            out wallHit,
            wallCheckDistance
        );

        if(isWall){
            isWall = wallHit.collider.CompareTag("Wall");
        }

        //壁上り判定
        //空中から壁上り開始
        if (
            isWall &&
            !isGround &&
            Input.GetKey(KeyCode.W) &&
            !isWallClimbing &&
            !isRoll
        ){
            StartWallClimb();
        }

        //地上から壁上り開始
        if (
            isWall &&
            isGround &&
            Input.GetKey(KeyCode.W) &&
            Input.GetKeyDown(KeyCode.Space) &&
            !isWallClimbing &&
            !isRoll
        ){
            StartWallClimb();
        }

        //Eキーで壁上り終了
        if(isWallClimbing && Input.GetKeyDown(KeyCode.E)){
            StopWallClimb();
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

 if(isWallClimbing)
{
    // 壁を再検出
    if(!Physics.Raycast(
        transform.position,
        transform.forward,
        out wallHit,
        wallCheckDistance + 0.5f
    ) || !wallHit.collider.CompareTag("Wall"))
    {
        StopWallClimb();
        return;
    }

    // 重力を無効化
    rb.useGravity = false;
    rb.linearVelocity = Vector3.zero;

    // 壁の正面を向く
    Quaternion targetRotation =
        Quaternion.LookRotation(-wallHit.normal);

    rb.MoveRotation(targetRotation);

    // ブースト中
    if(isWallBoost)
    {
        Vector3 nextPosition = Vector3.MoveTowards(
            rb.position,
            wallBoostTarget,
            wallBoostSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        if(Vector3.Distance(
            nextPosition,
            wallBoostTarget
        ) < 0.01f)
        {
            isWallBoost = false;
        }

        return;
    }

    // 壁に沿った上方向
    Vector3 wallUp =
        Vector3.ProjectOnPlane(
            Vector3.up,
            wallHit.normal
        ).normalized;

    // 壁に沿った右方向
    Vector3 wallRight =
        Vector3.Cross(
            wallHit.normal,
            wallUp
        ).normalized;

    // WASD
    Vector3 wallMove =
        wallUp * v +
        wallRight * h;

    if(wallMove.magnitude > 1f)
    {
        wallMove.Normalize();
    }

    // 移動先
    Vector3 movePosition =
        rb.position +
        wallMove *
        wallClimbSPeed *
        Time.fixedDeltaTime;

    // 移動先から壁を検出
    if(Physics.Raycast(
        movePosition,
        transform.forward,
        out RaycastHit nextWallHit,
        wallCheckDistance + 0.5f
    ) && nextWallHit.collider.CompareTag("Wall"))
    {
        // 壁との距離を一定にする
        Vector3 wallPosition =
            nextWallHit.point +
            nextWallHit.normal *
            wallCheckDistance;

        rb.MovePosition(wallPosition);

        // 移動先の壁の方向を向く
        Quaternion nextRotation =
            Quaternion.LookRotation(
                -nextWallHit.normal
            );

        rb.MoveRotation(nextRotation);
    }
    else
    {
        // 移動先に壁がなければ終了
        StopWallClimb();
    }
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
        if (!isWallClimbing && move != Vector3.zero){
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

    void StartWallClimb(){
        isWallClimbing = true;
        rb.useGravity = false;
    }

    void StopWallClimb(){
        isWallClimbing = false;
        isWallBoost = false;

        rb.useGravity = true;

        // transform.position += wallHit.normal * wallDetachDistance;

        rb.linearVelocity = Vector3.zero;
    }

    void StartWallBoost(){
        isWallBoost = true;

        wallBoostTarget = 
            transform.position + Vector3.up * wallBoostDistance;
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