using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    public bool canReuseCabinets = false;

    public float playerSpeed = 3;
    public float playerReverseSpeed = 4;
    public float DeadZone = 0f;

    Vector3 playerSpawnPos;

    GameObject attachedCable;

    public bool isSpaceShip;

    public LineRenderer CableLineRendered;
    public float CableMinDistance = 0.25f;
    float CableNextDistance = 0.5f;
    public float CableMaxLength = 15f;
    // Do not change
    public float CableCurrentLenght = 0f;
    public float CableCurrentLenghtFromCable0 = 0f;

    public float CableVibrationStartFromEnd = 10;
    public float CableSlowDownStartFromEnd = 2;
    public float CablePullingSlowDownStartFromLast0 = 4f;
    public float VibrationIntensity = 0.4f;

    public bool hasCable = false;
    public bool isActive = false;

    public DayNightCycle DayNightCycle;

    public Animator anim;
    public SpriteRenderer spriteRendered;
    PlayerDeathController deathController;

    public float BashingPower = 14f;
    public float BashingAmount = 0f;
    public float BashingPowerMin = 0.5f;

    float DisableMovementTimer = 0f;

    public float LastVibrationAlpha = 0f;

    public bool CanEatLastCablePosition = false;

    public float ClosedEyesTime = 0.3f;
    public float OpenedEyesTime = 0.8f;
    bool eyesOpened = true;
    // Do not change
    public float ClosedEyesTimer = 0f;

    public float CablePickupDistance = 0.301f;

    int Cable0 = 0;
    Vector2 CablePickupPosition = Vector2.zero;
    LevelStart levelStart;

    public GameObject CablePickup;
    public GameObject CablePickupInstance;

    public enum Gamepads
    {
        GAMEPAD_1,
        GAMEPAD_2,
        GAMEPAD_3,
        GAMEPAD_4
    }
    public Gamepads gamepadAssigned;

    public enum AnimationDirections
    {
        FACING_UP,
        FACING_DOWN,
        FACING_LEFT,
        FACING_RIGHT
    }
    AnimationDirections animDir;

    public bool GameIsThrowning = true;
    public float ThrowMultiplier = 10f;
    public bool ChargingThrow = false;
    public float ThrowPower = 0f;
    public float MaxThrowPower = 3f;
    public Vector2 ThrowDirection;

    Transform CableTarget;

    void Start()
    {
        CableTarget = transform;

        levelStart = FindObjectOfType<LevelStart>();
        DayNightCycle = FindObjectOfType<DayNightCycle>();
        rb = GetComponent<Rigidbody2D>();
        playerSpawnPos = transform.position;
        deathController = GetComponent<PlayerDeathController>();

        spriteRendered = anim.gameObject.GetComponent<SpriteRenderer>();

        if (spriteRendered.gameObject.transform.GetChild(0))
        {
            CableTarget = spriteRendered.gameObject.transform.GetChild(0);
        }

        CableNextDistance = CableMinDistance;

        CableLineRendered.SetPosition(0, playerSpawnPos);
        if (hasCable)
        {
            CableLineRendered.SetPosition(1, CableTarget.position);
        }
        
        CanEatLastCablePosition = false;
    }

    public float CabledSlowDownRate;

    void Update()
    {
        UpdateEyesColor();

        if (!levelStart.gamePlaying)
        {
            return;
        }

        UpdateCableLenght();
        CableCurrentLenghtFromCable0 = CableLenghtUpToCable0();

        if (GetBackAxisButtonDown() > 0 && HasCableMoreThan3MovablePoints() && hasCable)
        {
            DisableMovementTimer = 0.4f;
            BashingAmount += Time.deltaTime * BashingPower;
            BashingAmount = Mathf.Clamp(BashingAmount, 0, 1.5f);
        }
        else if (BashingAmount > 0)
        {
            BashingAmount -= Time.deltaTime;
            if (BashingAmount < 0)
            {
                BashingAmount = 0;
            }
        }

        // Go back on the cable
        if (hasCable && (DisableMovementTimer > 0))
        {
            DisableMovementTimer -= Time.deltaTime;

            //if (CableCurrentLenght >= CableMaxLength || BashingAmount >= BashingPowerMin)
            {
                if (HasCableMoreThan2MovablePoints())
                {
                    int i = 2;
                    float Dist = Vector2.Distance(CableTarget.position, CableLineRendered.GetPosition(CableLineRendered.positionCount - i));
                    if (Dist <= CablePickupDistance)
                    {
                        i = 3;
                    }

                    Vector2 newVelocity = new Vector2(CableLineRendered.GetPosition(CableLineRendered.positionCount - i).x - CableTarget.position.x, CableLineRendered.GetPosition(CableLineRendered.positionCount - i).y - CableTarget.position.y);

                    // Method 1
                    //rb.AddForce(playerReverseSpeed * newVelocity);
                    //rb.velocity = Vector2.ClampMagnitude(rb.velocity, playerReverseSpeed);

                    CabledSlowDownRate = 1f;
                    if (CableCurrentLenghtFromCable0 < CablePullingSlowDownStartFromLast0)
                    {
                        CabledSlowDownRate = CableCurrentLenghtFromCable0 / CablePullingSlowDownStartFromLast0;
                    }
                    CabledSlowDownRate = Mathf.Pow(CabledSlowDownRate, 1.67f);
                    // Method 2
                    rb.velocity = playerReverseSpeed * (1f + BashingAmount) * newVelocity.normalized * CabledSlowDownRate;

                    // Correction: Useless
                    //Vector2 newPosition = rb.velocity * Time.deltaTime;
                    //Vector2 backVelocity = new Vector2(CableLineRendered.GetPosition(CableLineRendered.positionCount - i).x - newPosition.x, CableLineRendered.GetPosition(CableLineRendered.positionCount - i).y - newPosition.y);
                    //if (Vector2.Dot(newVelocity, backVelocity) < 0)
                    //{
                    //    rb.velocity
                    //}
                }
                else
                {
                    rb.velocity *= (1f - (Time.deltaTime * 3.73f));
                }
            }

            ThrowPower = 0;
        }
        else //if (DisableMovementTimer <= 0)
        {
            if (GameIsThrowning)
            {
                float StickThrowPrecision = 0.994f;
                Vector2 Stick = DetectPlayerInput();
                if (ThrowPower > 0 && Stick.magnitude <= StickThrowPrecision)
                {
                    ChargingThrow = false;
                    ThrowPower = ThrowPower / MaxThrowPower;
                    ThrowPower = Mathf.Pow(ThrowPower, 0.3f);

                    Vector2 VelocityDir = ThrowDirection.normalized;
                    //rb.velocity = playerSpeed * VelocityDir * ((VelocityMagnitude - DeadZone) / (1f - DeadZone));
                    rb.AddForce(playerSpeed * ThrowMultiplier * ThrowPower * VelocityDir);
                    //rb.velocity = Vector2.ClampMagnitude(rb.velocity, playerSpeed * 3);

                    ThrowPower = 0;
                }
                else if (Stick.magnitude > StickThrowPrecision)
                {
                    ThrowDirection = -Stick;
                    ThrowPower += Time.deltaTime;
                    ThrowPower = Mathf.Clamp(ThrowPower, 0f, MaxThrowPower);
                    ChargingThrow = true;                   
                }
                else
                {
                    ThrowPower = 0;
                }                
            }
            else
            {
                UpdateMovement(DetectPlayerInput());
            }
        }

        if (HasCableMoreThan2MovablePoints() && hasCable)
        {
            Vector2 lastPointDirection2 = CableLineRendered.GetPosition(CableLineRendered.positionCount - 2) - CableTarget.position;
            CanEatLastCablePosition = Vector2.Dot(rb.velocity.normalized, lastPointDirection2.normalized) > 0;
            //if (GameIsThrowning)
            //{
            //    CanEatLastCablePosition = false;
            //}

            // Collecting last cable position
            if (CanEatLastCablePosition || DisableMovementTimer > 0 || CableCurrentLenght >= CableMaxLength)
            {
                float Dist2 = Vector2.Distance(CableTarget.position, CableLineRendered.GetPosition(CableLineRendered.positionCount - 2));
                while (Dist2 <= CablePickupDistance && HasCableMoreThan2MovablePoints())
                {
                    //CableCurrentLenght -= Vector2.Distance(CableLineRendered.GetPosition(CableLineRendered.positionCount - 2), CableLineRendered.GetPosition(CableLineRendered.positionCount - 3));

                    CableLineRendered.positionCount--;
                    CableLineRendered.SetPosition(CableLineRendered.positionCount - 1, CableTarget.position);

                    Dist2 = Vector2.Distance(CableTarget.position, CableLineRendered.GetPosition(CableLineRendered.positionCount - 2));

                    // Update CanEatLastCablePosition
                    lastPointDirection2 = CableLineRendered.GetPosition(CableLineRendered.positionCount - 2) - CableTarget.position;
                    CanEatLastCablePosition = Vector2.Dot(rb.velocity.normalized, lastPointDirection2.normalized) > 0;
                }
            }
        }
        else
        {
            CanEatLastCablePosition = false;
        }

        // Laying cable down
        //if (BashingAmount < BashingPowerMin || !HasCableMoreThan2MovablePoints())
        if (DisableMovementTimer <= 0 && (!CanEatLastCablePosition || !HasCableMoreThan2MovablePoints()) && hasCable)
        {
            float Dist = Vector2.Distance(CableTarget.position, CableLineRendered.GetPosition(CableLineRendered.positionCount - 2));
            if (Dist >= CableNextDistance)
            {
                CableNextDistance = CableMinDistance; // - (Dist - CableNextDistance);
                //CanEatLastCablePosition = false;
                CableLineRendered.positionCount++;
                CableLineRendered.SetPosition(CableLineRendered.positionCount - 2, CableTarget.position);
                CableLineRendered.SetPosition(CableLineRendered.positionCount - 1, CableTarget.position);
            }
        }

        // Update vibration
        if (hasCable && (CableMaxLength - CableCurrentLenght <= CableVibrationStartFromEnd))
        {
            LastVibrationAlpha = (CableCurrentLenght - (CableMaxLength - CableVibrationStartFromEnd)) / CableVibrationStartFromEnd;
            XInputDotNetPure.GamePad.SetVibration((PlayerIndex)GetVibrationPlayerNumber(), LastVibrationAlpha * VibrationIntensity, LastVibrationAlpha * VibrationIntensity);
        }
        else
        {
            XInputDotNetPure.GamePad.SetVibration((PlayerIndex)GetVibrationPlayerNumber(), 0, 0);
        }

        DetectPlayerAnimationDirection();

        if (!hasCable && Vector2.Distance(CablePickupPosition, CableTarget.position) < CablePickupDistance)
        {
            hasCable = true;
            if (CablePickupInstance)
            {
                Destroy(CablePickupInstance);
            }
        }

        // Update cable position (last) in player's hand
        if (hasCable)
        {
            CableLineRendered.SetPosition(CableLineRendered.positionCount - 1, CableTarget.position);
        }
       
    }

    public float SlowDownRate;
    public void UpdateMovement(Vector2 newVelocity)
    {
        //newVelocity = Vector2.ClampMagnitude(newVelocity.x, newVelocity.y), 1);
        float VelocityMagnitude = newVelocity.magnitude;
        SlowDownRate = (CableCurrentLenght - (CableMaxLength - CableSlowDownStartFromEnd)) / CableSlowDownStartFromEnd;
        if (!hasCable || CableMaxLength - CableCurrentLenght > CableSlowDownStartFromEnd)
        {
            SlowDownRate = 0f;
        }

        SlowDownRate = Mathf.Pow(1 - SlowDownRate, 4);

        if (VelocityMagnitude < DeadZone)
        {
            rb.velocity *= (1f - (Time.deltaTime * 0.5f)) * SlowDownRate;
            //rb.velocity = Vector2.zero;
        }
        else if (!hasCable || CableCurrentLenght <= CableMaxLength - CableSlowDownStartFromEnd) // || BashingAmount >= BashingPowerMin)
        {
            //rb.inertia = 55;
            Vector2 VelocityDir = newVelocity.normalized;
            //rb.velocity = playerSpeed * VelocityDir * ((VelocityMagnitude - DeadZone) / (1f - DeadZone));
            rb.AddForce((playerSpeed / 4) * VelocityDir * SlowDownRate * ((VelocityMagnitude - DeadZone) / (1f - DeadZone)));
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, playerSpeed);
        }
        else //if (hasCable && CableCurrentLenght > CableMaxLength - CableSlowDownStartFromEnd)
        {
            rb.velocity *= (1f - (Time.deltaTime * 0.5f)) * SlowDownRate;
        }        
    }
    
    void UpdateCableLenght()
    {
        CableCurrentLenght = 0f;
        //for (int i = Cable0; i < CableLineRendered.positionCount - 1; ++i)
        for (int i = 0; i < CableLineRendered.positionCount - 1; ++i)
        {
            CableCurrentLenght += Vector2.Distance(CableLineRendered.GetPosition(i), CableLineRendered.GetPosition(i + 1));
        }
    }

    float CableLenghtUpToCable0()
    {
        float Lenght = 0f;
        //for (int i = 0; i < Cable0 + 1; ++i)
        //{
        //    Lenght += Vector2.Distance(CableLineRendered.GetPosition(i), CableLineRendered.GetPosition(i + 1));
        //}
        for (int i = Cable0; i < CableLineRendered.positionCount - 1; ++i)
        {
            Lenght += Vector2.Distance(CableLineRendered.GetPosition(i), CableLineRendered.GetPosition(i + 1));
        }
        return Lenght;
    }

    bool HasCableMoreThan2MovablePoints()
    {
        return CableLineRendered.positionCount > Cable0 + 2;
    }
    bool HasCableMoreThan3MovablePoints()
    {
        return CableLineRendered.positionCount > Cable0 + 3;
    }

    //
    // Events
    //

    public void Finished()
    {
        isActive = false;
        playerReverseSpeed = 0f;
        playerSpeed = 0f;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        CableMaxLength = 99999f; //Fast hack to stop vibration
        XInputDotNetPure.GamePad.SetVibration((PlayerIndex)GetVibrationPlayerNumber(), 0, 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            PlayerDies();
        }
    }

    // Attach cable to Cabinet (Called by cabinet)
    public void AttachCable(Transform cabinet, float NewCableLenght, bool IsStartingCabinet)
    {
        if (CableTarget)
        {
            CableLineRendered.SetPosition(CableLineRendered.positionCount - 1, CableTarget.position);
        }
        else
        {
            CableLineRendered.SetPosition(CableLineRendered.positionCount - 1, transform.position);
        }

        if (IsStartingCabinet)
        {
            hasCable = true;
            CableLineRendered.SetPosition(CableLineRendered.positionCount - 2, cabinet.position);
            CableMaxLength = NewCableLenght;
            return;
        }

        UpdateCableLenght();
        CableMaxLength = CableCurrentLenght + NewCableLenght;

        Cable0 = CableLineRendered.positionCount - 1;

        CableLineRendered.positionCount++;
        if (CableTarget)
        {
            CableLineRendered.SetPosition(CableLineRendered.positionCount - 1, CableTarget.position);
        }
        else
        {
            CableLineRendered.SetPosition(CableLineRendered.positionCount - 1, transform.position);
        }
        CableLineRendered.SetPosition(CableLineRendered.positionCount - 2, cabinet.position);
    }
    
    public void PlayerDies()
    {
        if (hasCable)
        {
            DisableMovementTimer = 0f;
            CanEatLastCablePosition = false;
            hasCable = false;
            CablePickupPosition = CableTarget.position;
            if (CablePickupInstance)
            {
                Destroy(CablePickupInstance);
            }
            CablePickupInstance = GameManager.Instantiate(CablePickup);
            CablePickupInstance.transform.position = CablePickupPosition;
        }

        //CableLineRendered.positionCount = 2;
        deathController.ShowPlayerDies();

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        transform.position = playerSpawnPos;
    }

    private void OnDestroy()
    {
        XInputDotNetPure.GamePad.SetVibration((PlayerIndex)GetVibrationPlayerNumber(), 0, 0);
    }

    //
    // Input
    //

    public void SetPlayerToGamepad(int gamepad)
    {
        gamepadAssigned = (Gamepads)gamepad;
        if (gamepad == 0)
        {
            SetAnimDirection(false, false, false, true);
        }
    }
    float GetBackAxis()
    {
        int PlayerN = (int)gamepadAssigned + 1;
        string BackAxis = "Player" + PlayerN + "Back";
        if (Input.GetAxisRaw(BackAxis + "Axis") > 0f)
        {
            return Input.GetAxisRaw(BackAxis + "Axis");
        }
        return Input.GetButton(BackAxis) == true ? 1 : 0;
    }
    float GetBackAxisButtonDown()
    {
        int PlayerN = (int)gamepadAssigned + 1;
        string BackAxis = "Player" + PlayerN + "Back";
        return Input.GetButtonDown(BackAxis) == true ? 1 : 0;
    }
    float GetXMovement()
    {
        int PlayerN = (int)gamepadAssigned + 1;
        string Axis = "Player" + PlayerN + "Horizontal";
        return Input.GetAxisRaw(Axis);
    }
    float GetYMovement()
    {
        int PlayerN = (int)gamepadAssigned + 1;
        string Axis = "Player" + PlayerN + "Vertical";
        return Input.GetAxisRaw(Axis);
    }
    int GetVibrationPlayerNumber()
    {
        //Vector2 asd1 = new Vector2(Input.GetAxisRaw("Player1Horizontal"), Input.GetAxisRaw("Player1Vertical"));
        //Vector2 asd2 = new Vector2(XInputDotNetPure.GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X, XInputDotNetPure.GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y);
        //Vector2 asd21 = new Vector2(Input.GetAxisRaw("Player2Horizontal"), Input.GetAxisRaw("Player2Vertical"));
        //Vector2 asd22 = new Vector2(XInputDotNetPure.GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.X, XInputDotNetPure.GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.Y);

        //if (Vector2.Distance(asd1, asd2) <= Vector2.Distance(asd1, asd22))
        //{
        //    return 1;
        //}
        //return 0;

        //return (int)gamepadAssigned;

        if ((int)gamepadAssigned == 0)
        {
            return 1;
        }
        else if ((int)gamepadAssigned == 1)
        {
            return 0;
        }
        else return (int)gamepadAssigned;
    }
    PlayerIndex GetPlayerIndex()
    {
        if ((int)gamepadAssigned == 0)
        {
            return PlayerIndex.One;
        }
        else if ((int)gamepadAssigned == 1)
        {
            return PlayerIndex.Two;
        }
        else if ((int)gamepadAssigned == 2)
        {
            return PlayerIndex.Three;
        }
        //else if ((int)gamepadAssigned == 3)
        //{
        //}
        return PlayerIndex.Four;
    }
    
    Vector2 DetectPlayerInput()
    {
        float x = 0;
        float y = 0;

        switch (gamepadAssigned)
        {
            case Gamepads.GAMEPAD_1:
                if (Input.GetAxisRaw("Player1Horizontal") > 0) { x = Input.GetAxisRaw("Player1Horizontal"); }
                if (Input.GetAxisRaw("Player1Horizontal") < 0) { x = Input.GetAxisRaw("Player1Horizontal"); }
                if (Input.GetAxisRaw("Player1Vertical") > 0) { y = Input.GetAxisRaw("Player1Vertical"); }
                if (Input.GetAxisRaw("Player1Vertical") < 0) { y = Input.GetAxisRaw("Player1Vertical"); }
                break;
            case Gamepads.GAMEPAD_2:
                if (Input.GetAxisRaw("Player2Horizontal") > 0) { x = Input.GetAxisRaw("Player2Horizontal"); }
                if (Input.GetAxisRaw("Player2Horizontal") < 0) { x = Input.GetAxisRaw("Player2Horizontal"); }
                if (Input.GetAxisRaw("Player2Vertical") > 0) { y = Input.GetAxisRaw("Player2Vertical"); }
                if (Input.GetAxisRaw("Player2Vertical") < 0) { y = Input.GetAxisRaw("Player2Vertical"); }
                break;
            case Gamepads.GAMEPAD_3:
                if (Input.GetAxisRaw("Player3Horizontal") > 0) { x = Input.GetAxisRaw("Player3Horizontal"); }
                if (Input.GetAxisRaw("Player3Horizontal") < 0) { x = Input.GetAxisRaw("Player3Horizontal"); }
                if (Input.GetAxisRaw("Player3Vertical") > 0) { y = Input.GetAxisRaw("Player3Vertical"); }
                if (Input.GetAxisRaw("Player3Vertical") < 0) { y = Input.GetAxisRaw("Player3Vertical"); }
                break;
            case Gamepads.GAMEPAD_4:
                if (Input.GetAxisRaw("Player4Horizontal") > 0) { x = Input.GetAxisRaw("Player4Horizontal"); }
                if (Input.GetAxisRaw("Player4Horizontal") < 0) { x = Input.GetAxisRaw("Player4Horizontal"); }
                if (Input.GetAxisRaw("Player4Vertical") > 0) { y = Input.GetAxisRaw("Player4Vertical"); }
                if (Input.GetAxisRaw("Player4Vertical") < 0) { y = Input.GetAxisRaw("Player4Vertical"); }
                break;
        }
        return new Vector2(x, y);
    }

    //
    // Animations
    //

    void UpdateEyesColor()
    {
        // Toggle eyes color
        ClosedEyesTimer += Time.deltaTime;
        while (eyesOpened ? ClosedEyesTimer >= OpenedEyesTime : ClosedEyesTimer >= ClosedEyesTime)
        {
            if (eyesOpened)
            {
                ClosedEyesTimer -= OpenedEyesTime;
            }
            else
            {
                ClosedEyesTimer -= ClosedEyesTime;
            }
            eyesOpened = !eyesOpened;

            if (!eyesOpened)
            {
                spriteRendered.color = Color.black;
            }
            else
            {
                spriteRendered.color = Color.white;
            }
        }
    }
    void DetectPlayerAnimationDirection()
    {
        float x = 0;
        float y = 0;

        if (rb.velocity.magnitude < 0.01f)
        {
            SetAnimDirection(false, false, false, true);
            anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash);
            // Keep same anim Direction
        }
        else if (DisableMovementTimer > 0 || GameIsThrowning)
        {
            if (Mathf.Abs(rb.velocity.x) >= Mathf.Abs(rb.velocity.y))
            {
                if (rb.velocity.x >= 0)
                {
                    SetAnimDirection(false, false, false, true);
                }
                else
                {
                    SetAnimDirection(false, false, true, false);
                }
            }
            else //if (Mathf.Abs(rb.velocity.x) < Mathf.Abs(rb.velocity.y))
            {
                if (rb.velocity.y >= 0)
                {
                    SetAnimDirection(true, false, false, false);
                }
                else
                {
                    SetAnimDirection(false, true, false, false);
                }
            }
        }
        else if (Mathf.Abs(GetXMovement()) < DeadZone && Mathf.Abs(GetYMovement()) < DeadZone)
        {
            SetAnimDirection(false, false, false, true);
            // Keep same anim Direction
        }
        else if (Mathf.Abs(GetXMovement()) > Mathf.Abs(GetYMovement()))
        {
            if (GetXMovement() >= 0)
            {
                SetAnimDirection(false, false, false, true);
            }
            else
            {
                SetAnimDirection(false, false, true, false);
            }
        }
        else //if (Mathf.Abs(GetXMovement()) <= Mathf.Abs(GetYMovement()))
        {
            if (GetYMovement() >= 0)
            {
                SetAnimDirection(true, false, false, false);
            }
            else
            {
                SetAnimDirection(false, true, false, false);
            }
        }

        anim.SetBool("PullingCable", DisableMovementTimer > 0);
        anim.SetFloat("WalkingSpeed", rb.velocity.magnitude * 0.89f);
        anim.SetBool("HasCable", hasCable);
    }
    void SetAnimDirection(bool up, bool down, bool left, bool right)
    {
        if (isSpaceShip)
        {
            if (rb)
            {
                float angle = Vector2.Angle(Vector2.up, new Vector2(rb.velocity.x, rb.velocity.y));
                Vector2 normVel = rb.velocity.normalized;
                normVel = ThrowDirection.normalized;
                angle = Mathf.Atan2(normVel.x, normVel.y) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
                rb.rotation = -angle;
                rb.angularVelocity = 0;
            }
        }
        else
        {
            anim.SetBool("WalkingUp", up);
            anim.SetBool("WalkingDown", down);
            anim.SetBool("WalkingLeft", left);
            anim.SetBool("WalkingRight", right);
        }
        
    }
}