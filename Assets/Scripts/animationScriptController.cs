using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    Animator animator;

    public Transform cameraTransform;

    int isJoggingHash;
    int isRunningHash;
    int jumpHash;
    int runSlideHash;

    public float joggingSpeed = 5.0f;
    public float runningSpeed = 7.0f;
    public float turnSpeed = 200f;

    private bool isSliding = false;
    public float slideTimer = 0.0f;

    void Start() {
        animator = GetComponent<Animator>();
        Debug.Log(animator);

        isJoggingHash = Animator.StringToHash("isJogging");
        isRunningHash = Animator.StringToHash("isRunning");
        jumpHash = Animator.StringToHash("Jump");
        runSlideHash = Animator.StringToHash("runSlide");
    }

    void Update() {
        bool isJogging = animator.GetBool("isJogging");
        bool isRunning = animator.GetBool("isRunning");
        bool Jump = animator.GetBool("Jump");
        bool runSlide = animator.GetBool("runSlide");

        bool forwardKey = Input.GetKey(KeyCode.W);
        bool rightKey = Input.GetKey(KeyCode.D);
        bool leftKey = Input.GetKey(KeyCode.A);
        bool downKey = Input.GetKey(KeyCode.S);
        bool runningKey = Input.GetKey(KeyCode.LeftShift);
        bool jumpKey = Input.GetKey(KeyCode.Space);
        bool slideKey = Input.GetKey(KeyCode.E);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!isJogging && (forwardKey || rightKey || leftKey || downKey)) {
            animator.SetBool(isJoggingHash, true);
        }

        //Stop moving
        if (isJogging && !(forwardKey || rightKey || leftKey || downKey)) {
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isJoggingHash, false);
        }

        //Running
        if (!isRunning && ((forwardKey || rightKey || leftKey || downKey) && runningKey)) {
            animator.SetBool(isRunningHash, true);
            transform.Translate(Vector3.forward * runningSpeed * Time.deltaTime);
        }

        // Stop running
        if (isRunning && (!(forwardKey || rightKey || leftKey || downKey) || !runningKey)) {
            animator.SetBool(isRunningHash, false);
        }

        if (isSliding) {
            // Translation of player when sliding
            transform.Translate(Vector3.forward * runningSpeed * Time.deltaTime);

            slideTimer -= Time.deltaTime;

            if (slideTimer <= 0f) {
                isSliding = false;
                animator.SetBool(runSlideHash, false);
            }
        } else {
            // Normalized input
            Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

            if (inputDirection.magnitude >= 0.1f) {
                // Calculate target angle based on camera direction
                float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

                // Smooth player rotation
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSpeed, 0.1f);
                transform.rotation = Quaternion.Euler(0, angle, 0);

                // Move in direction relative to camera
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                float speed = runningKey ? runningSpeed : joggingSpeed;
                transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
            }
        }
       
        if (jumpKey) {
            animator.SetBool(jumpHash, true);
        }

        if (stateInfo.IsName("Jump") && stateInfo.normalizedTime >= 1.0f) {
            // Reset jump once animation ends
            animator.SetBool(jumpHash, false);
        }

        if (isRunning && slideKey && !isSliding) {
            isSliding = true;
            animator.SetBool(runSlideHash, true);
            slideTimer = 1.0f;
        }        
    }
}