using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public int width, height = 100;
    public Vector2 offset = new Vector2(0, 0);
    public float scale = 1;

    public Texture2D GetTexture()
    {
        return GenerateTexture(GenerateNoiseMap());
    }

    private float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[width, height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.PerlinNoise( x / scale + offset.x,  y / scale + offset.y);
            }
        }

        return noiseMap;
    }

    private Texture2D GenerateTexture(float[,] noiseMap)
    {
        Color[] colors = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colors[x + y * width] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        
        Texture2D texture2D = new Texture2D(width, height);
        texture2D.SetPixels(colors);
        texture2D.Apply();
        
        return texture2D;
    }
}