using UnityEngine;
using System.Collections;

public class CoinGeneration : MonoBehaviour
{
    public GameObject limitCurve;
    private Rigidbody topLimit;
    private Rigidbody bottomLimit;
    private float spawnAbscissa = 5f;
    [SerializeField] GameObject coinModel;
    [SerializeField] GameObject bigCoinModel;
    [SerializeField] GameObject badCoinModel;
    [SerializeField] AnimationCurve curve1;
    [SerializeField] AnimationCurve curve2;
    [SerializeField] AnimationCurve curve3;
    [SerializeField] AnimationCurve curve4;
    [SerializeField] AnimationCurve curve5;
    [SerializeField] AnimationCurve curve6;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("randomSpawn");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator randomSpawn()
    {   
        yield return new WaitForSeconds(2);
        while(true)
        {
            
            int randomInt = Random.Range(0, 9);
            switch (randomInt)
            {
                case 0:
                    CreateRectangle(2, 8, Random.value, 0.1f, 0.1f, 0.1f);
                    yield return new WaitForSeconds(Random.Range(2,6));
                    break;
                case 1:
                    CreateRectangle(5, 3, Random.value, 0.1f, 0.1f, 0.1f);
                    yield return new WaitForSeconds(Random.Range(1,5));
                    break;
                case 2:
                    CreateCurve(10, 0.4f, Random.value, curve1, 0.2f, 0.2f);
                    yield return new WaitForSeconds(Random.Range(3,7));
                    break;
                case 3:
                    CreateCurve(20, 0.4f, Random.value, curve2, 0.2f, 0.2f);
                    yield return new WaitForSeconds(Random.Range(5,9));
                    break;
                case 4:
                    CreateCurve(30, 0.4f, Random.value, curve3, 0.1f, 0.2f);
                    yield return new WaitForSeconds(Random.Range(4,8));
                    break;
                case 5:
                    CreateCurve(8, 0.4f, Random.value, curve4, 0.3f, 0.5f);
                    yield return new WaitForSeconds(Random.Range(3,7));
                    break;
                case 6:
                    float r = Random.value;
                    CreateCurve(5, 0.4f, r, curve5, 0.3f, 0.5f);
                    CreateCurve(5, 0.4f, r, curve6, 0.3f, 0.5f);
                    yield return new WaitForSeconds(Random.Range(2,6));
                    break;
                case 7:
                    CreateWall(10);
                    yield return new WaitForSeconds(Random.Range(2,6));
                    break;
                case 8:
                    CreateWall(10);
                    yield return new WaitForSeconds(Random.Range(2,6));
                    break;
                default:
                    break;
            }
        }
    }

    void CreateCoin(float height, float probabilityBig){
        GameObject newCoins;
        if (probabilityBig == -1f){
            newCoins = Instantiate(badCoinModel);
        } else if (Random.value < probabilityBig){
            newCoins = Instantiate(bigCoinModel);
        } else {
            newCoins = Instantiate(coinModel);
        } 
        newCoins.transform.position = new Vector3(spawnAbscissa, GetEquivalent(height), 0f);
        newCoins.SetActive(true);
    }

    void CreateCoins(float[] coins, float probabilityBig){
        for (int i = 0; i < coins.Length; i++){
            CreateCoin(coins[i], probabilityBig);
        }
    }

    Vector3[] GetLimits(){
        Vector3[] limits = limitCurve.GetComponent<RandomCurves>().GetLatestPoints();
        spawnAbscissa = limits[0].x;
        return limits;
    }

    float GetEquivalent(float height){
        Vector3[] limits = GetLimits();
        return Mathf.Lerp(limits[0].y, limits[1].y, height);
    }


    
    IEnumerator CoinsGeneration(float[][] coins, float horizontalSpace, float probabilityBig){
        /*
        for (int i = 0; i < coins.Length; i++){
            string rowText = string.Join(" ", coins[i]); // Convertit la ligne en texte
            Debug.Log(rowText); // Affichage dans la console Unity
        }*/

        for (int i = 0; i < coins.Length; i++){
            CreateCoins(coins[i], probabilityBig);
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

    void CreateRectangle(int height, int width, float position, float verticalSpace, float horizontalSpace, float probabilityBig){
        float[][] coins = CreateRectangleAux(height, width, position, verticalSpace);
        StartCoroutine(CoinsGeneration(coins, horizontalSpace, probabilityBig));
    }

    

    float[][] CreateCurveAux(int width, float height, float position, AnimationCurve curve){
        float[][] mat = new float[width][];
        
        float duration = curve.keys[curve.keys.Length - 1].time - curve.keys[0].time;
        for (int i = 0; i < width; i++){
            float x = ((float)i)/(width-1) * duration;
            float[] temp = {1 - curve.Evaluate(x)*height - position/2};
            mat[i] = temp;
        }

        return mat;

    }

    void CreateCurve(int width, float height, float position, AnimationCurve curve, float horizontalSpace, float probabilityBig){
        float[][] coins = CreateCurveAux(width, height, position, curve);
        StartCoroutine(CoinsGeneration(coins, horizontalSpace, probabilityBig));
    }


    

    

    float[][] CreateWallAux(int height){
        float[][] mat = new float[1][];
        mat[0] = new float[height];
        
        for (int i = 0; i < height; i++){
            mat[0][i] = ((float)i)/(height-1);
        }

        return mat;

    }

    void CreateWall(int height){
        float[][] coins = CreateWallAux(height);
        StartCoroutine(CoinsGeneration(coins, 0f, -1f));
    }

}
