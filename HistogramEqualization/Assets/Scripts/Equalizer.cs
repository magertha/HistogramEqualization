using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Equalizer : MonoBehaviour
{
    public Texture2D sourceTexture; //y�klenen g�rselin texture'u
    public RawImage raw; //ekrandaki g�rsel

    void Start()
    {
        sourceTexture = raw.texture as Texture2D; //raw image'�n texture componentini alma
        Texture2D equalizedTexture = EqualizeHistogram(sourceTexture);
    }

    public void OnButtonClick() //buton onClick fonksiyonu
    {
        Texture2D texture = raw.texture as Texture2D;
        if (texture != null)
        {
            Texture2D equalizedTexture = EqualizeHistogram(texture); //fonksiyonu uygulama
            raw.texture = equalizedTexture; //ekrandaki resmi equalized olan ile de�i�tirme
        }
        else
        {
            Debug.LogError("Texture2D de�il");
        }
    }

    public Texture2D EqualizeHistogram(Texture2D originalTexture)
    {
        //y�klenen g�rselden pikseller� �ekiyoruz
        Color[] originalColors = originalTexture.GetPixels();
        Color[] newColors = new Color[originalColors.Length];

        float[] values = new float[originalColors.Length];
        int[] histogram = new int[256];
        float[] cumulativeDistribution = new float[256];

        //histogram hesaplama k�sm�
        for (int i = 0; i < originalColors.Length; i++)
        {
            Color color = originalColors[i];
            float max = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
            values[i] = max;
            int histogramIndex = (int)(max * 255);
            histogram[histogramIndex]++;
        }

        //k�m�latif da��l�m fonk. (CDF) hesaplama k�sm�
        int total = values.Length;
        for (int i = 0; i < 256; i++)
        {
            if (i == 0)
                cumulativeDistribution[i] = (float)histogram[i] / total;
            else
                cumulativeDistribution[i] = cumulativeDistribution[i - 1] + (float)histogram[i] / total;
        }

        //renkleri yeni equalized de�erlere at�yoruz
        for (int i = 0; i < originalColors.Length; i++)
        {
            Color oldColor = originalColors[i];
            float oldValue = values[i];
            int oldIndex = (int)(oldValue * 255);
            float newValue = cumulativeDistribution[oldIndex];
            float scaleFactor = newValue / oldValue;

            //her kanala scaleFactor'u uyguluoruz
            newColors[i] = new Color(oldColor.r * scaleFactor, oldColor.g * scaleFactor, oldColor.b * scaleFactor);
        }

        //yeni olu�turdu�umuz texture'un pixellerini ayarl�yoruz
        Texture2D newTexture = new Texture2D(originalTexture.width, originalTexture.height);
        newTexture.SetPixels(newColors);
        newTexture.Apply();
        return newTexture;
    }

}
