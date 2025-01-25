using UnityEngine;
using System.Collections;

public class CoinGeneration : MonoBehaviour
{
    public GameObject testtop;
    public GameObject testbottom;
    private Rigidbody2D topLimit;
    private Rigidbody2D bottomLimit;
    private float spawnAbscissa = 5f;
    [SerializeField] GameObject coinModel;
    [SerializeField] AnimationCurve curve1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        topLimit = testtop.GetComponent<Rigidbody2D>();
        bottomLimit = testbottom.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P)){
            CreateRectangle(3,5,Random.value,0.1f,0.3f);
        }
    }

    void CreateCoin(float height){
        GameObject newCoins = Instantiate(coinModel);
        newCoins.transform.position = new Vector3(spawnAbscissa, GetEquivalent(height), 0f);
        newCoins.SetActive(true);
    }

    void CreateCoins(float[] coins){
        for (int i = 0; i < coins.Length; i++){
            CreateCoin(coins[i]);
        }
    }

    float[] GetLimits(){
        float[] limits = new float[2];
        limits[0] = topLimit.position.y;
        limits[1] = bottomLimit.position.y;
        return limits;
    }

    float GetEquivalent(float height){
        float[] limits = GetLimits();
        return Mathf.Lerp(limits[0], limits[1], height);
    }


    
    IEnumerator CoinsGeneration(float[][] coins, float horizontalSpace){
        for (int i = 0; i < coins.Length; i++){
            CreateCoins(coins[i]);
            yield return new WaitForSeconds(horizontalSpace);
        }
    }

    float[][] CreateRectangleAux(int height, int width, float position, float verticalSpace){
        float[] column = new float[height];
        float totalHeightSize = (height-1) * verticalSpace;

        for (int i = 0; i < height; i++){
            float pos = position - totalHeightSize/2 + i*verticalSpace;
            column[i] = pos;
        }

        float[][] mat = new float[width][];

        for (int i = 0; i < width; i++){
            mat[i] = column;
        }

        return mat;

    }

    void CreateRectangle(int height, int width, float position, float verticalSpace, float horizontalSpace){
        float[][] coins = CreateRectangleAux(height, width, position, verticalSpace);
        StartCoroutine(CoinsGeneration(coins, horizontalSpace));
    }

}
