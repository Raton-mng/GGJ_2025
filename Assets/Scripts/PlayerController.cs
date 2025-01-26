using System.Collections;
using UI.Menu;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject _hud;
    public GameObject playerHUD;
    private HealthManager _healthManager;
    private TurboManager _turboManager;
    public CoinManager coinManager;
    
    [SerializeField] private float baseHorizontalSpeed;
    [SerializeField] private float baseVerticalSpeed;
    [SerializeField] private float baseTurningSpeed;
    private float currentHorizontalSpeed;
    private float currentVerticalSpeed;
    private float currentTurningSpeed;

    [SerializeField] private float accelerateModifier;
    [SerializeField] private float turboFillDuration;

    private bool _isGoingUp;
    private float _boostingUse;

    private bool _isInversed;
    private bool _isPushedUpward;
    private float _pushForce;
    
    private bool _isDead;
    
    public int myID;

    public bool isDeath = false;
    private AnimationCurve deathUpCurve;

    [SerializeField] Sprite cursor1;
    [SerializeField] Sprite cursor2;
    [SerializeField] Sprite cursor3;
    [SerializeField] Sprite cursor4;
    private SpriteRenderer spriteRenderer;

    void Start(){
        playerHUD = Instantiate(_hud, PlayerManager.Instance.canvas);
        _turboManager = playerHUD.GetComponentInChildren<TurboManager>();
        _healthManager = playerHUD.GetComponentInChildren<HealthManager>();
        coinManager = playerHUD.GetComponentInChildren<CoinManager>();
        currentHorizontalSpeed = baseHorizontalSpeed;
        currentVerticalSpeed = baseVerticalSpeed;
        currentTurningSpeed = baseTurningSpeed;

        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log(myID);
        if (myID == 1){
            spriteRenderer.sprite = cursor1;
            playerHUD.transform.position = new Vector3(283, 895, 0);
        }
        else if (myID == 2){
            spriteRenderer.sprite = cursor2;
            playerHUD.transform.position = new Vector3(1618, 914, 0);
        }
        else if (myID == 3){
            spriteRenderer.sprite = cursor3;
            playerHUD.transform.position = new Vector3(283, 145, 0);
        }
        else if (myID == 4){
            spriteRenderer.sprite = cursor4;
            playerHUD.transform.position = new Vector3(1618, 145, 0);
        }

        InitDeathCurve();
    }

    void Update(){
        if (isDeath){
            return;
        }
        if(RandomCurves.Instance.IsPlayerBelowUpperCurve(transform.position))
        {
            if(_healthManager.TakeDamage())
            {
                DieUp();
            }
        }
        else if(RandomCurves.Instance.IsPlayerAboveLowerCurve(transform.position))
        {
            if (_healthManager.TakeDamage())
            {
                DieDown();
            }
        }
        else if(RandomCurves.Instance.IsPlayerInRedZone(transform.position))
        {
            _turboManager.GainTurbo(Time.deltaTime / turboFillDuration);
        }
        if (_boostingUse>0.03 && _turboManager.GetTurbo() > 0){
            Accelerate(baseHorizontalSpeed*2);
            _turboManager.UseTurbo();
        } 
        else if (_boostingUse<-0.03 && _turboManager.GetTurbo() > 0)
        {
            Accelerate(baseHorizontalSpeed*0.5f);
            _turboManager.UseTurbo();
        } 
        else 
        {
            if (transform.localPosition.x < -0.1){
                currentHorizontalSpeed = baseHorizontalSpeed*1.1f;
            } 
            else if (transform.localPosition.x > 0.1){
                currentHorizontalSpeed = baseHorizontalSpeed*0.9f;
            } 
            else {   
                currentHorizontalSpeed = baseHorizontalSpeed;
                Vector3 pos = transform.localPosition;
                pos.x = 0f;
                transform.localPosition = pos;
            }
        }
        if (_isGoingUp){
            GoUp(currentHorizontalSpeed, currentVerticalSpeed);
        } else {
            GoDown(currentHorizontalSpeed, currentVerticalSpeed);
        }
        
        Vector3 tempPos = transform.localPosition;
        tempPos.x += (currentHorizontalSpeed - baseHorizontalSpeed) * Time.deltaTime;
        transform.localPosition = tempPos;
    }


    public bool GoUp(float dx, float dy){
        float targetAngle = Mathf.Atan(dy/dx) * 180 / Mathf.PI;
        LookTo(targetAngle, currentTurningSpeed);

        Vector3 pos = transform.localPosition;
        pos.y += dy * Time.deltaTime;

        if (_isPushedUpward) pos.y += _pushForce * Time.deltaTime;
        
        transform.localPosition = pos;
        
        return false;
    }

    public bool GoDown(float dx, float dy){
        return GoUp(dx, -dy);
    }


    public void Accelerate(float newSpeed){
        currentHorizontalSpeed = newSpeed;

    }


    public void OnPauseMenu(InputAction.CallbackContext context)
    {
        Pause.Instance.OnPauseButton();
        Debug.Log("Pause");
    }

    public void LookTo(float target, float speed){
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0f, 0f, target),
            speed * Time.deltaTime
        );
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _isGoingUp = _isInversed ? !context.action.WasPerformedThisFrame() : context.action.WasPerformedThisFrame();
    }

    public void OnBoosting(InputAction.CallbackContext context)
    {
        _boostingUse = context.action.ReadValue<float>();
    }

    public void OnSelectMenu(InputAction.CallbackContext context)
    {
        print(context.action.WasPerformedThisFrame());
        ShopManager.Instance.MoveCursor(myID, context.action.WasPerformedThisFrame());
    }

    public void OnNavigateMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            float value = context.action.ReadValue<float>();
            print(value);

            if (value > 0)
            {
                ShopManager.Instance.MoveCursor(myID, true);
            }
            else if (value < 0)
            {
                ShopManager.Instance.MoveCursor(myID, false);
            }
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

    public IEnumerator PushUp(float timer, float pushForce)
    {
        _isPushedUpward = true;
        _pushForce = pushForce;
        yield return new WaitForSeconds(timer);
        _isPushedUpward = false;
    }

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
        float duration = deathUpCurve.keys[deathUpCurve.keys.Length - 1].time - deathUpCurve.keys[0].time;
        float top = 3.5f;
        
        while (true){
            if (currentTime >=1f){
                transform.Rotate(new Vector3(0f, 0, 500f) * Time.deltaTime);  
            }
            
            transform.localPosition = new Vector3(transform.localPosition.x-Time.deltaTime*2f, startY + deathUpCurve.Evaluate(currentTime)*top, transform.localPosition.z);
            yield return null;
            currentTime += Time.deltaTime;
            if (currentTime > duration){
                break;
            }
        }
    }

    IEnumerator ExplodeDown(){
        float currentTime = 0f;
        float currentX = transform.localPosition.x;
        float currentY = transform.localPosition.y;
        float currentZ = transform.localPosition.z;
        
        while (true){
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
            if (currentTime > 6f){
                break;
            }
        }
    }


    public bool IsDead()
    {
        return _isDead;
    }
    
    void InitDeathCurve(){

        deathUpCurve = new AnimationCurve();

        Keyframe key1 = new Keyframe(0f, 0f, 0f, 0f); // Tangentes = 1 (pente constante)
        Keyframe key2 = new Keyframe(1f, 0f, 0f, 0f); // Même pente pour assurer la linéarité
        Keyframe key3 = new Keyframe(1.1f, 1f, -5f, 5f); // Même pente pour assurer la linéarité
        Keyframe key4 = new Keyframe(5f, -5f, 0f, 0f); // Même pente pour assurer la linéarité

        // Ajouter les keyframes à la courbe

        deathUpCurve.AddKey(key1);      // t=0, valeur=0
        deathUpCurve.AddKey(key2);      // t=1, valeur=1
        deathUpCurve.AddKey(key3);      // t=1, valeur=1
        deathUpCurve.AddKey(key4);      // t=1, valeur=1
    }
}
