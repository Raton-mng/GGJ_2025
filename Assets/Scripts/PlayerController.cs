using System;
using System.Collections;
using UI.Menu;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public GameObject _hud;
    //public GameObject playerHUD;
    private HealthManager _healthManager;
    private TurboManager _turboManager;
    public CoinManager coinManager;
    
    [SerializeField] private float baseHorizontalSpeed;
    [SerializeField] private float baseTurningSpeed;

    [SerializeField] private float accelerateModifier;
    [SerializeField] private float turboFillDuration;

    private bool _isGoingUp;

    private bool _isInversed;
    
    private bool _isDead;
    
    public int myID;

    public bool isDeath = false;
    private AnimationCurve deathUpCurve;

    [SerializeField] Sprite cursor1;
    [SerializeField] Sprite cursor2;
    [SerializeField] Sprite cursor3;
    [SerializeField] Sprite cursor4;
    [SerializeField] private SpriteRenderer spriteRenderer;


    [Header("Mouvement Vertical")]
    public float baseVerticalSpeed = 5f; // Vitesse de base
    public float investmentMultiplier = 0.1f; // Influence de l'investissement sur la vitesse

    [Header("Mouvement Horizontal (Boost)")]
    public float horizontalSpeed = 5f;
    public float returnSpeed = 2f; // Vitesse de retour vers X=0

    [Header("Rotation")]
    public float rotationAngle = 20f; // Angle d'inclinaison max
    public float rotationSpeed = 10f; // Vitesse de rotation

    [Header("Take Damage")]
    public float takeDamageDuration = 1f;
    public float blinkDamageDuration = 0.1f;

    private bool isMovingUp = false;
    private bool hasMoved = false; // Indique si OnMove a été utilisé
    private float currentX = 0f;
    private float currentRotation = 0f;
    private float horizontalInput = 0f;

    private void Awake()
    {
        _healthManager = _hud.GetComponentInChildren<HealthManager>();
        _turboManager = _hud.GetComponentInChildren<TurboManager>();
        coinManager = _hud.GetComponentInChildren<CoinManager>();

    }

    public GameObject GetHUD()
    {
        return _hud;
    }

    
    public HealthManager GetHealthManager()
    {
        return _healthManager;
    }

    void Start(){

        if (myID == 0)
        {
            spriteRenderer.sprite = cursor1;
            _hud.GetComponent<RectTransform>().anchoredPosition = new Vector3(1625, 940, 0);

        }
        else if (myID == 1)
        {
            spriteRenderer.sprite = cursor2;
            _hud.GetComponent<RectTransform>().anchoredPosition = new Vector3(280, 940, 0);


        }
        else if (myID == 2)
        {
            spriteRenderer.sprite = cursor3;
            _hud.GetComponent<RectTransform>().anchoredPosition = new Vector3(728, 940, 0);

        }
        else if (myID == 3)
        {
            spriteRenderer.sprite = cursor4;
            _hud.GetComponent<RectTransform>().anchoredPosition = new Vector3(1176, 940, 0);

        }
        transform.position = new Vector3(0, myID * 4, 0);

        InitDeathCurve();
    }

    private void OnEnable()
    {
        spriteRenderer.enabled = true;
    }

    void Update(){
        if (isDeath){
            return;
        }
        if(RandomCurves.Instance.IsPlayerBelowUpperCurve(transform.position))
        {
            StartCoroutine(BlinkTakeDamage());
            if (_healthManager.TakeDamage())
            {
                DieUp();
            }
        }
        else if(RandomCurves.Instance.IsPlayerAboveLowerCurve(transform.position))
        {
            StartCoroutine(BlinkTakeDamage());
            if (_healthManager.TakeDamage())
            {
                DieDown();
            }
        }
        else if(RandomCurves.Instance.IsPlayerInRedZone(transform.position))
        {
            _turboManager.GainTurbo(Time.deltaTime / turboFillDuration);
        }
        // Calcul de la vitesse ajustée par l'investissement
        float adjustedVerticalSpeed = baseVerticalSpeed * (1 + InvestmentManager.Instance.GetInvestAmountByPlayer(myID) * investmentMultiplier);

        // Mouvement vertical instantané
        float verticalMove = 0f;
        if (hasMoved)
        {
            verticalMove = isMovingUp ? adjustedVerticalSpeed : -adjustedVerticalSpeed;
        }

        transform.position += new Vector3(0, verticalMove * Time.deltaTime, 0);

        // Gérer la rotation (inclinaison vers le haut/bas)
        float targetRotation = 0f;
        if (hasMoved)
        {
            targetRotation = isMovingUp ? rotationAngle : -rotationAngle;
        }
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, 0, currentRotation);

        // Mouvement horizontal fluide
        if (_turboManager.GetTurbo() > 0)
        {
            currentX += horizontalInput * horizontalSpeed * Time.deltaTime;
            if(horizontalInput != 0)
            {
                _turboManager.UseTurbo();
            }
        }
        if(Mathf.Abs(horizontalInput) < 0.01f) {
            

            // Retour progressif vers X=0
            if(Mathf.Abs(currentX) > 0.01f)
            {
                float returnDirection = -Mathf.Sign(currentX); // Vers 0
                float returnAmount = returnSpeed * Time.deltaTime;

                if (Mathf.Abs(currentX) < returnAmount) currentX = 0f;
                else currentX += returnDirection * returnAmount;
            }
        }

        // Appliquer le mouvement horizontal
        transform.position = new Vector3(currentX, transform.position.y, transform.position.z);


        
    }


    public void OnPauseMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) Pause.Instance.OnPauseButton();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        isMovingUp = _isInversed ? !context.action.WasPerformedThisFrame() : context.action.WasPerformedThisFrame();
        hasMoved = true; // Dès qu'on appuie sur OnMove, on considère que le joueur a commencé à jouer

    }

    public void OnBoosting(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
           
    }

    public void OnSelectMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!ShopManager.ShopActive) InvestmentManager.Instance.InvestInPLayer(myID);
            else ShopManager.Instance.ActivateCardEffect(myID);
        }
    }

    public void OnNavigateMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            //print(value);
            if(!ShopManager.ShopActive) InvestmentManager.Instance.MoveCursor(myID, context.action.ReadValue<float>());
            else ShopManager.Instance.MoveCursor(myID, context.action.ReadValue<float>());

        }
    }

    public IEnumerator BoostTheBoost(float boostModifierModifier, float timer)
    {
        float baseBoost = accelerateModifier;
        accelerateModifier *= boostModifierModifier;
        yield return new WaitForSeconds(timer);
        accelerateModifier = baseBoost;
    }
    
    public IEnumerator InverseControls(float timer)
    {
        _isGoingUp = !_isGoingUp;
        _isInversed = true;
        yield return new WaitForSeconds(timer);
        _isInversed = false;
    }

    /*public IEnumerator PushUp(float timer, float pushForce)
    {
        _isPushedUpward = true;
        _pushForce = pushForce;
        yield return new WaitForSeconds(timer);
        _isPushedUpward = false;
    }*/

    private void DieUp(){
        isDeath = true;
        StartCoroutine(ExplodeUp());
    }

    private void DieDown(){
        isDeath = true;
        StartCoroutine(ExplodeDown());
    }

    IEnumerator ExplodeUp(){
        float currentTime = 0f;
        float startY = transform.localPosition.y;
        float top = 3.5f;
        
        float timeBeforeDisappear = 6f;

        while (timeBeforeDisappear > 0){
            timeBeforeDisappear -= Time.deltaTime;
            if (currentTime >=1f){
                transform.Rotate(new Vector3(0f, 0, 500f) * Time.deltaTime);  
            }
            
            transform.localPosition = new Vector3(transform.localPosition.x-Time.deltaTime*2f, startY + deathUpCurve.Evaluate(currentTime)*top, transform.localPosition.z);
            yield return null;
            currentTime += Time.deltaTime;
        }
        PlayerManager.Instance.SomeoneDied(gameObject);

    }

    IEnumerator ExplodeDown(){
        float currentTime = 0f;
        float currentX = transform.localPosition.x;
        float currentY = transform.localPosition.y;
        float currentZ = transform.localPosition.z;
        
        float timeBeforeDisappear = 6f;

        while (timeBeforeDisappear > 0){
            timeBeforeDisappear -= Time.deltaTime;
            transform.localPosition = new Vector3(currentX, currentY, currentZ);
            if (currentTime >=1f){
                currentY -= Time.deltaTime * 4f; 
                transform.localPosition = new Vector3(currentX, currentY, currentZ);
            } else {
                transform.localPosition = new Vector3(currentX + (Random.value * 0.5f - 0.25f), currentY + (Random.value * 0.5f - 0.25f), currentZ + (Random.value * 0.5f - 0.25f));
            }
            
            yield return null;
            currentTime += Time.deltaTime;
            currentX -= Time.deltaTime*2f;
            
        }
        PlayerManager.Instance.SomeoneDied(gameObject);
    }


    public bool IsDead()
    {
        return _isDead;
    }
    
    void InitDeathCurve(){

        deathUpCurve = new AnimationCurve();

        Keyframe key1 = new Keyframe(0f, 0f, 0f, 0f); // Tangentes = 1 (pente constante)
        Keyframe key2 = new Keyframe(1f, 0f, 0f, 0f); // Même pente pour assurer la linéarité
        Keyframe key3 = new Keyframe(1.2f, 3f, 0f, -1f); // Même pente pour assurer la linéarité
        Keyframe key4 = new Keyframe(15f, -40f, -1f, -1f); // Même pente pour assurer la linéarité

        // Ajouter les keyframes à la courbe

        deathUpCurve.AddKey(key1);      // t=0, valeur=0
        deathUpCurve.AddKey(key2);      // t=1, valeur=1
        deathUpCurve.AddKey(key3);      // t=1, valeur=1
        deathUpCurve.AddKey(key4);      // t=1, valeur=1
    }

    private IEnumerator BlinkTakeDamage()
    {
        float elapsed = 0f;
        float blinkDuration = 0f;
        while (elapsed < takeDamageDuration)
        {
            elapsed += Time.deltaTime;
            blinkDuration += Time.deltaTime;
            if (blinkDuration >= blinkDamageDuration)
            {
                blinkDuration = 0f;
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return null;
        }
    }
}
