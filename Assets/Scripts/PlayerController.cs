using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    enum PlayerState {
        Moving,
        Jumping,
        Falling,
    }

    private bool isGrounded;
    private bool jumpEnabled;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    // OPTIONAL: include if you want to limit x velocity
    private const float MAX_VEL_X = 15;

    private PlayerState currentState = PlayerState.Falling;

    public float xSpeed;
    public float jumpStrength;

    void Start()
    {
        isGrounded = false;
        jumpEnabled = false;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Method <c>MoveX</c> enables x inputs and movement.
    /// <param>percent</param>: A range of 0.0f to 1.0f for movement strength.
    /// </summary>
    void MoveX(float percent) {
        if (percent < 0.0f || percent > 1.0f) {
            throw new ArgumentException(String.Format("{0} is not in the range of [0.0f, 1.0f].", percent), "percent");
        }

        float xHat = new Vector2(Input.GetAxis("Horizontal"), 0).normalized.x;
        float vx = xHat * xSpeed * percent;
        rb.AddForce(transform.right * vx);
        // OPTIONAL: include if you want to limit x velocity
        rb.velocity = new Vector2(Vector2.ClampMagnitude(rb.velocity, MAX_VEL_X).x, rb.velocity.y);
    }

    void MoveState() {
        sr.color = Color.cyan;
        MoveX(1.0f);
        // jump input
        float yHat = new Vector2(0, Input.GetAxis("Vertical")).normalized.y;
        if (isGrounded && yHat == 1) {
            currentState = PlayerState.Jumping;
        }
    }

    void JumpState() {
        sr.color = Color.magenta;
        float vy = jumpStrength;
        isGrounded = false;
        rb.AddForce(transform.up * vy);
        currentState = PlayerState.Falling;
    }

    void FallingState() {
        sr.color = Color.white;
        MoveX(0.5f);

        jumpEnabled = isGrounded && rb.velocity.y <= 0;
        if (jumpEnabled) {
            currentState = PlayerState.Moving;
        }
    }


    void FixedUpdate()
    {
        if (currentState == PlayerState.Moving) {
            MoveState();
        } else if (currentState == PlayerState.Jumping) {
            JumpState();
        } else if (currentState == PlayerState.Falling) {
            FallingState();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") {
            isGrounded = true;
        }

        if (collision.gameObject.tag == "Enemy") {
            // Respawn
            SceneManager.LoadScene(0);
        }
    }
}