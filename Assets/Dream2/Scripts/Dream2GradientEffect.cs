using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Dream2GradientEffect : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public Color topColor = Color.red;    // 顶部颜色
    public Color bottomColor = Color.black; // 底部颜色
    
    void Start()
    {
        Texture2D texture = new Texture2D(width, height);
        
        // 填充渐变
        for (int y = 0; y < height; y++)
        {
            float t = y / (float)(height - 1); // 0在底部，1在顶部
            Color color = Color.Lerp(bottomColor, topColor, t);
            
            // 填充整行
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;
        
        // 应用到UI Image
        GetComponent<Image>().sprite = Sprite.Create(
            texture, 
            new Rect(0, 0, width, height), 
            new Vector2(0.5f, 0.5f)
        );
    }
}