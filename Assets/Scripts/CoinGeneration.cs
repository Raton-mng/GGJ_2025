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
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(1, 5));
            CreateCurve(5, 0.4f, Random.value, curve1, 0.3f, 0.5f);
        }
    }

    void CreateCoin(float height, float probabilityBig){
        GameObject newCoins;
        if (probabilityBig == -1){
            newCoins = Instantiate(badCoinModel);
        }
        if (Random.value < probabilityBig){
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

        for (int i = 0; i < width; i++){
            float x = ((float)i)/(width-1);
            float[] temp = {1 - curve.Evaluate(x)*height - position/2};
/*            Debug.Log(string.Format("{0}", curve.Evaluate(x)*height));
*/            mat[i] = temp;
        }

        return mat;

    }

    void CreateCurve(int width, float height, float position, AnimationCurve curve, float horizontalSpace, float probabilityBig){
        float[][] coins = CreateCurveAux(width, height, position, curve);
        StartCoroutine(CoinsGeneration(coins, horizontalSpace, probabilityBig));
    }

}
