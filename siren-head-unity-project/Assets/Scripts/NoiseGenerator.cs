using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    public int width = 100, height = 100;
    public Vector2 offset = new Vector2(0, 0);
    public float scale = 30;

    public Texture2D GetTexture()
    {
        return GenerateTexture(GenerateNoiseMap());
    }

    public float GetNoiseAt(Vector2 samplePoint)
    {
        return Mathf.PerlinNoise( samplePoint.x / scale + offset.x,  samplePoint.y / scale + offset.y);
    }

    private float[,] GenerateNoiseMap()
    {
        float[,] noiseMap = new float[width, height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = GetNoiseAt(new Vector2(x, y));
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