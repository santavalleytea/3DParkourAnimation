using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {
    Animator animator;

    int isJoggingHash;
    int isRunningHash;
    int jumpHash;

    public float joggingSpeed = 5.0f;
    public float runningSpeed = 7.0f;
    public float turnSpeed = 200f;

    void Start() {
        animator = GetComponent<Animator>();
        Debug.Log(animator);

        isJoggingHash = Animator.StringToHash("isJogging");
        isRunningHash = Animator.StringToHash("isRunning");
        jumpHash = Animator.StringToHash("Jump");
    }

    void Update() {
        bool isJogging = animator.GetBool("isJogging");
        bool isRunning = animator.GetBool("isRunning");
        bool Jump = animator.GetBool("Jump");

        bool forwardKey = Input.GetKey(KeyCode.W);
        bool rightKey = Input.GetKey(KeyCode.D);
        bool leftKey = Input.GetKey(KeyCode.A);
        bool downKey = Input.GetKey(KeyCode.S);
        bool runningKey = Input.GetKey(KeyCode.LeftShift);
        bool jumpKey = Input.GetKey(KeyCode.Space);

        if (!isJogging && (forwardKey || rightKey || leftKey || downKey)) {
            animator.SetBool(isJoggingHash, true);
        }

        // Stop moving
        if (isJogging && !(forwardKey || rightKey || leftKey || downKey)) {
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isJoggingHash, false);
        }

        // Running
        if (!isRunning && ((forwardKey || rightKey || leftKey || downKey) && runningKey)) {
            animator.SetBool(isRunningHash, true);
            transform.Translate(Vector3.forward * runningSpeed * Time.deltaTime);
        }

        // Stop running
        if (isRunning && (!(forwardKey || rightKey || leftKey || downKey) || !runningKey)) {
            animator.SetBool(isRunningHash, false);
        }

        if (forwardKey || rightKey || leftKey || downKey) {
            float speed = isRunning ? runningSpeed : joggingSpeed;

            Vector3 moveDirection = Vector3.zero;

            if (forwardKey) {
                moveDirection += Vector3.forward;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), speed * Time.deltaTime);
                //transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
            }

            if (rightKey) {
                moveDirection += Vector3.right;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90, 0), speed * Time.deltaTime);
                //transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
            }

            if (leftKey) {
                moveDirection += Vector3.left;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -90, 0), speed * Time.deltaTime);
                //transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
            }

            if (downKey) {
                moveDirection += Vector3.back;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180, 0), speed * Time.deltaTime);
                //transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
            }

            if (moveDirection.magnitude > 1f) {
                moveDirection.Normalize();
            }

            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        }

        if (jumpKey) {
            animator.SetBool(jumpHash, true);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            // Reset jump once animation ends
            animator.SetBool(jumpHash, false);
        }
    }
}