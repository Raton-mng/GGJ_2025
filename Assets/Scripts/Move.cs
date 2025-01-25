using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float baseHorizontalSpeed;
    [SerializeField] private float baseVerticalSpeed;
    [SerializeField] private float baseTurningSpeed;
    private float currentHorizontalSpeed;
    private float currentVerticalSpeed;
    private float currentTurningSpeed;

    void Start(){
        currentHorizontalSpeed = baseHorizontalSpeed;
        currentVerticalSpeed = baseVerticalSpeed;
        currentTurningSpeed = baseTurningSpeed;
    }

    void Update(){
        if (Input.GetKey(KeyCode.Space)){
            Accelerate(baseHorizontalSpeed*2);
        } else if (Input.GetKey(KeyCode.C)){
            Accelerate(baseHorizontalSpeed*0.75f);
        } else {
            if (transform.position.x < -0.1){
                currentHorizontalSpeed = baseHorizontalSpeed*1.1f;
            } else if (transform.position.x > 0.1){
                currentHorizontalSpeed = baseHorizontalSpeed*0.9f;
            } else {   
                currentHorizontalSpeed = baseHorizontalSpeed;
                Vector3 pos = transform.position;
                pos.x = 0f;
                transform.position = pos;
            }
        }
        if (Input.GetKey(KeyCode.W)){
            GoUp(currentHorizontalSpeed, currentVerticalSpeed);
        } else {
            GoDown(currentHorizontalSpeed, currentVerticalSpeed);
        }
        
        Vector3 tempPos = transform.position;
        tempPos.x += (currentHorizontalSpeed - baseHorizontalSpeed) * Time.deltaTime;
        transform.position = tempPos;
    }


    public bool GoUp(float dx, float dy){
        float targetAngle = Mathf.Atan(dy/dx) * 180 / Mathf.PI;
        LookTo(targetAngle, currentTurningSpeed);

        Vector3 pos = transform.position;
        pos.y += dy * Time.deltaTime;
        transform.position = pos;
        
        return false;
    }

    public bool GoDown(float dx, float dy){
        return GoUp(dx, -dy);
    }


    public void Accelerate(float newSpeed){
        currentHorizontalSpeed = newSpeed;

    }

    public void Decelerate(float newSpeed){
        
    }


    public void LookTo(float target, float speed){
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0f, 0f, target),
            speed * Time.deltaTime
        );
    }
}
