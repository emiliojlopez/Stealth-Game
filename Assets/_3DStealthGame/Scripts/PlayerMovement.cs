using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    Animator m_Animator;
    public InputAction MoveAction;

    public float walkSpeed = 1.0f;
    public float turnSpeed = 20f;
    public float sprintSpeed = 1.75f;
    public float Stamina;
    public float MaxStamina;
    public float StaminaDrain;
    private bool HasStamina = true;
    private bool HoldingShift = false;
    public float ChargeRate;
    private float currentSpeed;
    public Image StaminaBar;
    private Coroutine recharge;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        MoveAction.Enable();
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Stamina > 0)
        {
            HasStamina = true;
        }
        HoldingShift = (Input.GetKey(KeyCode.LeftShift));
        // Check if sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            var pos = MoveAction.ReadValue<Vector2>();

            float horizontal = pos.x;
            float vertical = pos.y;

            m_Movement.Set(horizontal, 0f, vertical);
            m_Movement.Normalize();


            bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
            bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
            bool isWalking = hasHorizontalInput || hasVerticalInput;

            if (isWalking == true)
            {
                if (recharge != null) StopCoroutine(recharge);
                recharge = StartCoroutine(RechargeStamina());

                if (HasStamina == true)
                {
                    currentSpeed = sprintSpeed;
                    Stamina -= StaminaDrain * Time.deltaTime;
                    StaminaBar.fillAmount = Stamina / MaxStamina;
                    if (Stamina <= 0)
                    {
                        Stamina = 0;
                        currentSpeed = walkSpeed;
                        HasStamina = false;
                    }
                }
                else
                {
                    currentSpeed = walkSpeed;
                }
            }
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }
    private IEnumerator RechargeStamina ()
    {
        yield return new WaitForSeconds(1f);

        while (Stamina < MaxStamina)
        {
            Stamina += ChargeRate / 10f;
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            StaminaBar.fillAmount = Stamina / MaxStamina;
            yield return new WaitForSeconds(.1f);
        }
        }


        void FixedUpdate()
    {
        var pos = MoveAction.ReadValue<Vector2>();

        float horizontal = pos.x;
        float vertical = pos.y;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();


        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        bool TryingSprint = HasStamina && HoldingShift;
        bool isSprinting = TryingSprint && isWalking;
        m_Animator.SetBool("IsSprinting", isSprinting);
        m_Animator.SetBool("IsWalking", isWalking);


        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        m_Rigidbody.MoveRotation(m_Rotation);
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * currentSpeed * Time.deltaTime);
    }
}