using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;


[CustomEditor(typeof(LineDetection))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LineDetection myScript = (LineDetection)target;
        if (GUILayout.Button("Apply to Image"))
        {
            myScript.Apply();
        }
    }
}

public class LineDetection : MonoBehaviour  {
    
    
    //[Head("Takes an image and applies certain image processing methods to it, convert it to black and white, black pixels are converted into rooms")]
    public Texture2D map;
    //public float size;
    public Color[,] pixels;
    private Texture2D output;
    public bool greyScale, gaussianBlur3x3, gaussianBlur5x5, gaussianBlur3x32, gaussianBlur5x52, sobel3x3X, sobel3x3Y, flipcolors, threshhold;
    public float threshholdVal; //lower treshhold more black lines
    [HideInInspector]
    public bool redrawmap;
	// Use this for initialization
	public void Init () {
        Debug.Log(" INITING LIENDETECITTINTFNSDF");
        output = new Texture2D(map.width, map.height);
        pixels = new Color[map.width, map.height];
        SetPixelsValues();
        //Debug.Log(pixels[10, 10].r);
        //CalculateLineParts(pixels);
        //PrintCubes();
        Apply();
        redrawmap = false;
	}

    public void SetPixelsValues()
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                pixels[x, y] = map.GetPixel(x, y);
            }
        }
    }

    public void Apply()
    {
        SetPixelsValues();
        if (greyScale) GreyScale(pixels);
        if (gaussianBlur3x3) Convolution(pixels, GaussianMatrix3x3);
        if (gaussianBlur5x5) Convolution(pixels, GaussianMatrix5x5);
        if (gaussianBlur3x32) Convolution(pixels, GaussianMatrix3x3);
        if (gaussianBlur5x52) Convolution(pixels, GaussianMatrix5x5);
        if (sobel3x3X) Convolution(pixels, SobelMatrix3x3X);
        if (sobel3x3Y) Convolution(pixels, SobelMatrix3x3Y);
        if (threshhold) ThreshHold(pixels, threshholdVal);
        if(flipcolors) FlipColors(pixels);
        SavePicture();
        redrawmap = true;
    }

    void SavePicture()
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                output.SetPixel(x, y, pixels[x, y]);
            }
        }
        File.WriteAllBytes("temppic/"+"image.png",  output.EncodeToPNG());

    }

    void CalculateLineParts(Color[,] p)
    {

    }

    //deprecated
    //void PrintCubes()
    //{
    //    for (int x = 0; x < map.width; x++)
    //    {
    //        for (int y = 0; y < map.height; y++)
    //        {
    //            pixels[x, y] = map.GetPixel(x, y);
    //            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //            obj.transform.position = new Vector3(x, 0, y);
    //            MeshRenderer rend = obj.GetComponent<MeshRenderer>();
    //            rend.material.color = pixels[x, y];
    //        }
    //    }
    //}
	
	// Update is called once per frame
	void Update () {
		
	}
    //apply kernel to image
    private void Convolution(Color[,] Image, double[,] matrix)
    {
        double mSum = MatrixSum(matrix);
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y <map.height; y++)
            {
                double updatedColor = 0;
                for (int x2 = 0; x2 < matrix.GetLength(0); ++x2)
                {
                    for (int y2 = 0; y2 < matrix.GetLength(0); ++y2)
                    {
                        int x3 = x + x2;
                        int y3 = y + y2;

                        if (x3 >= map.width)
                            x3 = map.width - 1;
                        if (y3 >= map.height)
                            y3 = map.height - 1;

                        updatedColor += Image[x3, y3].r * matrix[x2, y2];
                    }
                }

                //stop overlighting
                if (mSum != 0)
                    updatedColor /= mSum;

                //edge detector
                if (updatedColor > 1)
                    updatedColor = 1;
                else if (updatedColor < 0)
                    updatedColor = 0;

                float t = (float)updatedColor;
                Image[x, y] = new Color(t, t, t);

            }
        }
    }

    private double MatrixSum(double[,] matrix) //square matrices only
    {
        double t = 0;
        for (int x2 = 0; x2 < matrix.GetLength(0); ++x2)
        {
            for (int y2 = 0; y2 < matrix.GetLength(1); ++y2)
            {
                t += matrix[x2, y2];
            }
        }

        return t;
    }

    private void GreyScale(Color[,] Image)
    {
        for (int x = 0; x < Image.GetLength(0); x++)
        {
            for (int y = 0; y < Image.GetLength(1); y++)
            {
                Color pixelColor = Image[x, y];                         // Get the pixel color at coordinate (x,y)
                float avg = (pixelColor.r + pixelColor.b + pixelColor.g) / 3;
                Color updatedColor = new Color(avg, avg, avg);
                Image[x, y] = updatedColor;                             // Set the new pixel color at coordinate (x,y)
            }
        }
    }

    //single threshhold
    private void ThreshHold(Color[,] Image, float threshhold)
    {
        for (int x = 0; x < Image.GetLength(0); x++)
        {
            for (int y = 0; y < Image.GetLength(1); y++)
            {
                if (Image[x, y].r > threshhold)
                    Image[x, y] = Color.black;
                else
                    Image[x, y] = Color.white;
            }
        }
    }

    private void FlipColors(Color[,] Image)
    {
        for (int x = 0; x < Image.GetLength(0); x++)
        {
            for (int y = 0; y < Image.GetLength(1); y++)
            {
                if (Image[x, y] == Color.black) Image[x, y] = Color.white;
                else if (Image[x, y] == Color.white) Image[x, y] = Color.black;
 
            }
        }
    }

    private double[,] GaussianMatrix5x5 = new double[,]
    {
           {1, 4, 7, 4, 1},
           {4, 16, 26, 16, 4},
           {7, 26 , 41, 26, 7},
           {4, 16, 26, 16, 4},
           {1, 4, 7, 4, 1}
    };

    private double[,] GaussianMatrix3x3 = new double[,]
    {
            {1, 2, 1 },
            {2, 4, 2 },
            {1, 2, 1 }
    };

    private double[,] SobelMatrix3x3X = new double[,]
    {
            {-1, 0, 1 },
            {-2, 0, 2 },
            {-1, 0, 1 }
    };

    private double[,] SobelMatrix3x3Y = new double[,]
    {
            {1, 2, 1 },
            {0, 0, 0 },
            {-1, -2, -1 }
    };
}
