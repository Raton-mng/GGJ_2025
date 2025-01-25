using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject _hud;
    private TurboManager _turboManager;
    public CoinManager coinManager;

    [SerializeField] private float baseHorizontalSpeed;
    [SerializeField] private float baseVerticalSpeed;
    [SerializeField] private float baseTurningSpeed;
    private float currentHorizontalSpeed;
    private float currentVerticalSpeed;
    private float currentTurningSpeed;

    [SerializeField] private float accelerateModifier;
    [SerializeField] private float decelerateModifier;

    private bool _isGoingUp;
    private float _boostingUse;

    private bool _isInversed;
    private bool _isDead;
    
    public int myID;

    private AnimationCurve deathUpCurve;

    void Start(){
        /*GameObject playerHUD = Instantiate(_hud);
        _turboManager = playerHUD.GetComponentInChildren<TurboManager>();
        coinManager = playerHUD.GetComponentInChildren<CoinManager>();*/
        currentHorizontalSpeed = baseHorizontalSpeed;
        currentVerticalSpeed = baseVerticalSpeed;
        currentTurningSpeed = baseTurningSpeed;

        InitDeathCurve();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.O)){
            DieUp();
        }
        if (_boostingUse>0.03 && _turboManager.GetTurbo() > 0){
            Accelerate(baseHorizontalSpeed*2);
            _turboManager.UseTurbo();
        } else if (_boostingUse<-0.03 && _turboManager.GetTurbo() > 0)
        {
            Accelerate(baseHorizontalSpeed*0.75f);
            _turboManager.UseTurbo();
        } else {
            if (transform.localPosition.x < -0.1){
                currentHorizontalSpeed = baseHorizontalSpeed*1.1f;
            } else if (transform.localPosition.x > 0.1){
                currentHorizontalSpeed = baseHorizontalSpeed*0.9f;
            } else {   
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
        transform.localPosition = pos;
        
        return false;
    }

    public bool GoDown(float dx, float dy){
        return GoUp(dx, -dy);
    }


    public void Accelerate(float newSpeed){
        currentHorizontalSpeed = newSpeed;

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

    public void OnMenuing(InputAction.CallbackContext context)
    {
        Debug.Log(context.action.ReadValue<float>());
    }

    public void OnBoosting(InputAction.CallbackContext context)
    {
        _boostingUse = context.action.ReadValue<float>();
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

    private void DieUp(){
        StartCoroutine(ExplodeUp());
    }

    IEnumerator ExplodeUp(){
        float currentTime = 0f;
        float startY = transform.localPosition.y;
        float duration = deathUpCurve.keys[deathUpCurve.keys.Length - 1].time - deathUpCurve.keys[0].time;
        float top = 2f;
        
        while (true){
            transform.localPosition = new Vector3(transform.localPosition.x, startY + deathUpCurve.Evaluate(currentTime)*top, transform.localPosition.z);
            yield return null;
            currentTime += Time.deltaTime;
            if (currentTime > duration){
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
        // Ajouter une première section, qui commence en 0 et augmente rapidement jusqu'à 1 en 1 seconde
        // Utilisation d'une fonction exponentielle pour une montée rapide et un ralentissement vers 1
        deathUpCurve.AddKey(0f, 0f);      // t=0, valeur=0
        deathUpCurve.AddKey(0.1f, 1f);      // t=1, valeur=1

        // Ajouter une seconde section linéaire, de 1 à -5 en 15 secondes
        deathUpCurve.AddKey(0.1f, 1f);      // t=1, valeur=1 (point de départ de la descente)
        deathUpCurve.AddKey(5f, -5f);    // t=16, valeur=-5 (fin de la descente)

        // Personnaliser les tangentes pour la première section pour ralentir avant d'atteindre 1
        Keyframe key1 = deathUpCurve.keys[1];
        key1.inTangent = -5f;  // Pour créer un effet de ralenti avant de stabiliser à 1
        key1.outTangent = 5f;  // Tangente sortante pour une montée rapide
        deathUpCurve.MoveKey(1, key1);

        // Personnaliser la tangente de la seconde section pour la rendre linéaire
        Keyframe key2 = deathUpCurve.keys[2];
        key2.inTangent = 0f;   // Tangente linéaire
        key2.outTangent = 0f;  // Tangente linéaire
        deathUpCurve.MoveKey(2, key2);
    }
}
