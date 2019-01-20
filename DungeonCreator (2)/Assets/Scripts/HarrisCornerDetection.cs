//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.

//public class HarrisCornerDetection : MonoBehaviour {

//    /// Process image looking for corners.
//    /// </summary>
//    /// 
//    /// <param name="image">Source image data to process.</param>
//    /// 
//    /// <returns>Returns list of found corners (X-Y coordinates).</returns>
//    /// 
//    /// <exception cref="UnsupportedImageFormatException">
//    ///   The source image has incorrect pixel format.
//    /// </exception>
//    /// 
//    public unsafe List<IntPoint> ProcessImage(UnmanagedImage image)
//    {
//        // check image format
//        if (
//            (image.PixelFormat != PixelFormat.Format8bppIndexed) & amp; &amp;
//        (image.PixelFormat != PixelFormat.Format24bppRgb) & amp; &amp;
//        (image.PixelFormat != PixelFormat.Format32bppRgb) & amp; &amp;
//        (image.PixelFormat != PixelFormat.Format32bppArgb)
//            )
//    {
//            throw new UnsupportedImageFormatException("Unsupported pixel format of the source image.");
//        }

//        // make sure we have grayscale image
//        UnmanagedImage grayImage = null;

//        if (image.PixelFormat == PixelFormat.Format8bppIndexed)
//        {
//            grayImage = image;
//        }
//        else
//        {
//            // create temporary grayscale image
//            grayImage = Grayscale.CommonAlgorithms.BT709.Apply(image);
//        }


//        // get source image size
//        int width = grayImage.Width;
//        int height = grayImage.Height;
//        int srcStride = grayImage.Stride;
//        int srcOffset = srcStride - width;


//        // 1. Calculate partial differences
//        float[,] diffx = new float[height, width];
//        float[,] diffy = new float[height, width];
//        float[,] diffxy = new float[height, width];


//        fixed (float* pdx = diffx, pdy = diffy, pdxy = diffxy)
//        {
//            byte* src = (byte*)grayImage.ImageData.ToPointer() + srcStride + 1;

//            // Skip first row and first column
//            float* dx = pdx + width + 1;
//            float* dy = pdy + width + 1;
//            float* dxy = pdxy + width + 1;

//            // for each line
//            for (int y = 1; y < height - 1; y++)
//            {
//                // for each pixel
//                for (int x = 1; x < width - 1; x++, src++, dx++, dy++, dxy++)
//                {
//                    // Convolution with horizontal differentiation kernel mask
//                    float h = ((src[-srcStride + 1] + src[+1] + src[srcStride + 1]) -
//                               (src[-srcStride - 1] + src[-1] + src[srcStride - 1])) * 0.166666667f;

//                    // Convolution vertical differentiation kernel mask
//                    float v = ((src[+srcStride - 1] + src[+srcStride] + src[+srcStride + 1]) -
//                               (src[-srcStride - 1] + src[-srcStride] + src[-srcStride + 1])) * 0.166666667f;

//                    // Store squared differences directly
//                    *dx = h * h;
//                    *dy = v * v;
//                    *dxy = h * v;
//                }

//                // Skip last column
//                dx++; dy++; dxy++;
//                src += srcOffset + 1;
//            }

//            // Free some resources which wont be needed anymore
//            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
//                grayImage.Dispose();
//        }


//        // 2. Smooth the diff images
//        if (sigma > 0.0)
//        {
//            float[,] temp = new float[height, width];

//            // Convolve with Gaussian kernel
//            convolve(diffx, temp, kernel);
//            convolve(diffy, temp, kernel);
//            convolve(diffxy, temp, kernel);
//        }


//        // 3. Compute Harris Corner Response Map
//        float[,] map = new float[height, width];

//        fixed (float* pdx = diffx, pdy = diffy, pdxy = diffxy, pmap = map)
//        {
//            float* dx = pdx;
//            float* dy = pdy;
//            float* dxy = pdxy;
//            float* H = pmap;
//            float M, A, B, C;

//            for (int y = 0; y < height; y++)
//            {
//                for (int x = 0; x < width; x++, dx++, dy++, dxy++, H++)
//                {
//                    A = *dx;
//                    B = *dy;
//                    C = *dxy;

//                    if (measure == HarrisCornerMeasure.Harris)
//                    {
//                        // Original Harris corner measure
//                        M = (A * B - C * C) - (k * ((A + B) * (A + B)));
//                    }
//                    else
//                    {
//                        // Harris-Noble corner measure
//                        M = (A * B - C * C) / (A + B + Accord.Math.Special.SingleEpsilon);
//                    }

//                    if (M > threshold)
//                    {
//                        *H = M; // insert value in the map
//                    }
//                }
//            }
//        }


//        // 4. Suppress non-maximum points
//        List<IntPoint> cornersList = new List<IntPoint>();

//        // for each row
//        for (int y = r, maxY = height - r; y < maxY; y++)
//        {
//            // for each pixel
//            for (int x = r, maxX = width - r; x < maxX; x++)
//            {
//                float currentValue = map[y, x];

//                // for each windows' row
//                for (int i = -r; (currentValue != 0) & amp; &amp; (i <= r); i++)
//            {
//            // for each windows' pixel
//            for (int j = -r; j <= r; j++)
//            {
//                if (map[y + i, x + j] > currentValue)
//                {
//                    currentValue = 0;
//                    break;
//                }
//            }
//        }

//        // check if this point is really interesting
//        if (currentValue != 0)
//        {
//            cornersList.Add(new IntPoint(x, y));
//        }
//    }
//}


//    return cornersList;
//}
//}
