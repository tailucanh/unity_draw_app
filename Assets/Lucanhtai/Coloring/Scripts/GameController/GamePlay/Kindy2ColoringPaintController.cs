using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Security.Cryptography;
using Lucanhtai.Observer;

namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringPaintController : MonoBehaviour
    {
        [SerializeField] private LayerMask paintLayerMask = 1 << 0; // to which layer our paint canvas is at (used in raycast)

        [Header("Brush Settings")]
        [SerializeField] private Color32 paintColor = new(255, 75, 75, 255);

        private int brushSize = 25; // default brush size
        private int brushSizeX1 = 48; // << 1
        private int brushSizeXbrushSize = 576; // x*x
        private int brushSizeX4 = 96; // << 2
        private int brushSizeDiv4 = 6; // >> 2 == /4

        private bool realTimeTexUpdate = true; // if set to true, ignore textureUpdateSpeed, and always update when textureNeedsUpdate gets set to true when drawing
        private float textureUpdateSpeed = 0.01f; // how often texture should be updated (0 = no delay, 1 = every one seconds)
        private float nextTextureUpdate = 0;

        [Header("Options")]
        [SerializeField] private DrawMode drawMode = DrawMode.Default;

        private const string MAIN_TEXTURE = "_MainTex";
        private Color32 clearColor = new(255, 255, 255, 255);

        [Header("Custom Brushes")]
        [SerializeField] private Texture2D[] customBrushes;
        private int selectedBrush = 0; // currently selected brush index
        private byte[] customBrushBytes;
        private int customBrushWidth;
        private int customBrushHeight;
        private int customBrushWidthHalf;
        private int customBrushHeightHalf;

        private Texture2D maskTex;

        private byte[] pixels; // byte array for texture painting, this is the image that we paint into.
        private byte[] maskPixels; // byte array for mask texture
        private byte[] clearPixels; // byte array for clearing texture

        private Texture2D drawingTexture;
        private float resolutionScaler = 0.5f; // 1 means screen resolution, 0.5f means half the screen resolution

        private int texWidth;
        private int texHeight;
        private Camera cam; // main camera reference
        private Renderer myRenderer;

        private RaycastHit hit;
        private bool wentOutside = false;
        private bool usingClearingImage = false; // did we have initial texture as maintexture, then use it as clear pixels array
        private Vector2 pixelUV; 
        private Vector2 pixelUVOld;
        private bool textureNeedsUpdate = false; // if we have modified texture
        private int idImage;
        private bool[,] visited;
        private List<List<Vector2Int>> alphaZeroRegions;
        private int[,] regionMap;
        private int regionIndex;
        
        [SerializeField] private RectTransform referenceArea;
        [SerializeField] private ColoringEffect coloringEffect;
        private EventSystem eventSystem;
        [SerializeField] private ColoringGuiding guidingHand;

       // public float PercentColoring { get; set; }
        public bool IsDrawing { get; set; } = false;

        private void Awake()
        {
            cam = Camera.main;
            myRenderer = GetComponent<Renderer>();
            GameObject go = GameObject.Find("EventSystem");
            if (go == null)
            {
                Debug.LogError("GameObject EventSystem is missing from scene, will have problems with the UI", gameObject);
            }
            else
            {
                eventSystem = go.GetComponent<EventSystem>();
                eventSystem.AddComponent<TouchInputModule>();
            }
        }

        private void Update()
        {
            MousePaint();
            if (textureNeedsUpdate && (realTimeTexUpdate || Time.time > nextTextureUpdate))
            {
                nextTextureUpdate = Time.time + textureUpdateSpeed;
                UpdateTexture();
            }
        }

        #region ---> Public function logic called from state <---
        public void InitializeEverything()
        {
            brushSizeX1 = brushSize << 1;
            brushSizeXbrushSize = brushSize * brushSize;
            brushSizeX4 = brushSizeXbrushSize << 2;

            SetPaintColor(paintColor);
            CreateFullScreenQuad();
            if (myRenderer.material.GetTexture(MAIN_TEXTURE) == null && !usingClearingImage) 
            {
                if (drawingTexture != null) Texture2D.DestroyImmediate(drawingTexture, true); 
                drawingTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
                myRenderer.material.SetTexture(MAIN_TEXTURE, drawingTexture);
                pixels = new byte[texWidth * texHeight * 4];

            }
            else
            { 
                usingClearingImage = true;
                texWidth = myRenderer.material.GetTexture(MAIN_TEXTURE).width;
                texHeight = myRenderer.material.GetTexture(MAIN_TEXTURE).height;

                if (drawingTexture != null) Texture2D.DestroyImmediate(drawingTexture, true); 
                drawingTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);

                ReadClearingImage();
                myRenderer.material.SetTexture(MAIN_TEXTURE, drawingTexture);
                pixels = new byte[texWidth * texHeight * 4];
            }

            drawingTexture.filterMode = FilterMode.Point;
            if (drawMode == DrawMode.CustomBrush) ReadCurrentCustomBrush();
            ClearImage();
            GameHelper.SetMultiTouchEnabled(false);
        }
        public void SetMaskImage(Texture2D texture, int id)
        {
            idImage = id;
            texture.filterMode = FilterMode.Bilinear;
            maskTex = texture;
            texWidth = texture.width;
            texHeight = texture.height;
            myRenderer.material.SetTexture("_MaskTex", texture);
            LoadPixelsFromFile(idImage);
            GetAreaTexture();
            Debug.Log("Lucanhtai texture set: " + texture.filterMode);
            textureNeedsUpdate = true;

        }
        public Texture2D GetScreenshot()
        {
            Texture2D getTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
            getTexture.SetPixelData<byte>(pixels, 0);
            getTexture.Apply(true);
            return getTexture;
        }
        public void SetPaintColor(Color32 color)
        {
            paintColor = color;
        }
        public void SetDrawMode(DrawMode drawMode)
        {
            this.drawMode = drawMode;
        }
        public async void SavePixelsToFile(Action OnDone)
        {
            string filePath = Application.persistentDataPath + "/savedPixelsColoringGame_" + idImage.ToString() + ".png";
            Texture2D textureToSave = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
            textureToSave.SetPixelData(pixels, 0);
            textureToSave.Apply();
            byte[] bytes = textureToSave.EncodeToPNG();
            await File.WriteAllBytesAsync(filePath, bytes);
            Destroy(textureToSave);
            OnDone();
        }
        #endregion

        #region ---> Logic to read partition image <---
        private void GetAreaTexture()
        {
            visited = new bool[texWidth, texHeight];
            alphaZeroRegions = new List<List<Vector2Int>>();
            regionMap = new int[texWidth, texHeight];
            int regionIndex = 0;
            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    if (!visited[x, y] && maskTex.GetPixel(x, y).a < 1f)
                    {
                        FloodFill(x, y, regionIndex);
                        regionIndex++;
                    }
                }
            }
        }
        private void FloodFill(int startX, int startY, int regionIndex)
        {
            List<Vector2Int> region = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(new Vector2Int(startX, startY));

            while (queue.Count > 0)
            {
                Vector2Int point = queue.Dequeue();
                if (visited[point.x, point.y])
                    continue;

                visited[point.x, point.y] = true;
                region.Add(point);
                regionMap[point.x, point.y] = regionIndex;
                foreach (var dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    Vector2Int nextPoint = point + dir;
                    if (nextPoint.x >= 0 && nextPoint.x < texWidth && nextPoint.y >= 0 && nextPoint.y < texHeight)
                    {
                        if (!visited[nextPoint.x, nextPoint.y] && maskTex.GetPixel(nextPoint.x, nextPoint.y).a < 1f)
                        {
                            queue.Enqueue(nextPoint);
                        }
                    }
                }
            }

            if (region.Count > 0)
            {
                alphaZeroRegions.Add(region);
            }
        }
        private int FindRegionIndexForPosition(int x, int y)
        {
            for (int regionIndex = 0; regionIndex < alphaZeroRegions.Count; regionIndex++)
            {
                foreach (Vector2Int point in alphaZeroRegions[regionIndex])
                {
                    if (point.x == x && point.y == y)
                    {
                        return regionIndex; 
                    }
                }
            }
            return -1;
        }
        private void ReadMaskImage()
        {
            Color32[] maskTexPixels = maskTex.GetPixels32();
            maskPixels = new byte[texWidth * texHeight * 4];
            int pixel = 0;
            for (int y = 0; y < texHeight; y++)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    Color32 c = maskTexPixels[y * texWidth + x];
                    maskPixels[pixel] = c.r;
                    maskPixels[pixel + 1] = c.g;
                    maskPixels[pixel + 2] = c.b;
                    maskPixels[pixel + 3] = c.a;
                    pixel += 4;
                }
            }
        }
        private void CreateFullScreenQuad()
        {
            if (referenceArea == null || GetComponent<MeshFilter>() == null)
                return;

            Mesh mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            Vector3[] corners = new Vector3[4];
            float nearClipOffset = 0.01f;
            referenceArea.GetWorldCorners(corners);

            Vector3[] vertices = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                vertices[i] = corners[i];
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = GetComponent<MeshFilter>().transform.InverseTransformPoint(vertices[i]);
                vertices[i].z = -Camera.main.transform.position.z + nearClipOffset;
            }

            mesh.vertices = vertices;

            mesh.uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.tangents = new[] { new Vector4(1.0f, 0.0f, 0.0f, -1.0f), new Vector4(1.0f, 0.0f, 0.0f, -1.0f), new Vector4(1.0f, 0.0f, 0.0f, -1.0f), new Vector4(1.0f, 0.0f, 0.0f, -1.0f) };

            if (myRenderer.GetComponent<MeshCollider>() == null) myRenderer.gameObject.AddComponent<MeshCollider>();
        }
        private async void LoadPixelsFromFile(int id)
        {
            string filePath = Application.persistentDataPath + "/savedPixelsColoringGame_" + id.ToString() + ".png";
            if (!File.Exists(filePath))
            {
                ReadMaskImage();
            }
            else
            {
                byte[] bytes = await File.ReadAllBytesAsync(filePath);
                Texture2D textureFromFile = new Texture2D(texWidth, texHeight);
                textureFromFile.LoadImage(bytes);
                Color32[] loadedColors = textureFromFile.GetPixels32();
                pixels = ConvertToByteArray(loadedColors);
                drawingTexture.LoadRawTextureData(pixels);
                drawingTexture.Apply(true);
                ReadMaskImage();
                myRenderer.material.SetTexture(MAIN_TEXTURE, drawingTexture);
                Destroy(textureFromFile);
            }
        }
        private bool IsPixelInRegion(int x, int y, int regionIndex)
        {
            return regionMap[x, y] == regionIndex;
        }
        private void LockAreaFill(int x, int y)
        {
            if (x >= texWidth) x = texWidth - 1;
            if (y >= texHeight) y = texHeight - 1;
            regionIndex = FindRegionIndexForPosition(x, y);
            Debug.Log("Lucanhtai vẽ vùng: " + regionIndex);
        }
        #endregion

        #region ---> Logic drawing <---
        private void MousePaint()
        {
            if (eventSystem.IsPointerOverGameObject()) return;
            if (eventSystem.currentSelectedGameObject != null) return;

            // catch first mousedown
            if (Input.GetMouseButtonDown(0))
            {
                guidingHand.ResetGuiding();
                // if lock area is used, we need to take full area before painting starts
                if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) return;
                LockAreaFill((int)(hit.textureCoord.x * texWidth), (int)(hit.textureCoord.y * texHeight));
            }

#if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                GetPositionByInput(Input.mousePosition);
            }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); 
            GetPositionByInput(touch.position);
        }
#endif
            if (Input.GetMouseButtonDown(0))
            {
                // take this position as start position
                if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, paintLayerMask)) return;

                pixelUVOld = pixelUV;
            }

            if (textureNeedsUpdate)
            {
                switch (drawMode)
                {
                    case DrawMode.Default: // drawing
                        DrawLine(pixelUVOld, pixelUV);
                        break;
                    case DrawMode.CustomBrush:
                        DrawLineWithBrush(pixelUVOld, pixelUV);
                        break;
                    case DrawMode.Eraser:
                        EraseWithBackgroundColorLine(pixelUVOld, pixelUV);
                        break;
                    default: // other modes
                        break;
                }
                pixelUVOld = pixelUV;
                textureNeedsUpdate = true;
            }

            // left mouse button released
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject hitObject = hit.collider.gameObject;

                    if (hitObject.GetComponent<RectTransform>() == null)
                    {
                        if (hit.collider.gameObject.GetComponent<RectTransform>() == null)
                        {
                            Vector3 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                            mousePosition.z = 0;
                            if (drawMode != DrawMode.Eraser)
                            {
                                coloringEffect.EffectPaint(mousePosition);
                            }
                            IsDrawing = true;
                        }
                        Kindy2ColoringEvent kindy2ColoringEvent = new Kindy2ColoringEvent(Kindy2ColoringState.PLAY_STATE.ToString(), null);
                        ObserverManager.TriggerEvent<Kindy2ColoringEvent>(kindy2ColoringEvent);
                    }
                }

            }

        }
        private void GetPositionByInput(Vector3 postion)
        {
            if (!Physics.Raycast(cam.ScreenPointToRay(postion), out hit, Mathf.Infinity, paintLayerMask)) { wentOutside = true; return; }

            pixelUVOld = pixelUV;
            pixelUV = hit.textureCoord;
            pixelUV.x *= texWidth;
            pixelUV.y *= texHeight;

            if (wentOutside) { pixelUVOld = pixelUV; wentOutside = false; }

            // lets paint where we hit
            switch (drawMode)
            {
                case DrawMode.Default: // brush
                    DrawCircle((int)pixelUV.x, (int)pixelUV.y);
                    break;

                case DrawMode.CustomBrush:
                    DrawCustomBrush((int)pixelUV.x, (int)pixelUV.y);
                    break;

                case DrawMode.Eraser:
                    EraseWithBackgroundColor((int)pixelUV.x, (int)pixelUV.y);
                    break;
                default:
                    Debug.LogError("Unknown drawMode");
                    break;
            }
            textureNeedsUpdate = true;
        }
        private void UpdateTexture()
        {
            textureNeedsUpdate = false;
            drawingTexture.SetPixelData<byte>(pixels, 0);
            drawingTexture.Apply(false);
        }
        private void DrawWithPixelTranslationAlgorithm(int pixel, Color32 color)
        {
            for (int offsetY = -2; offsetY <= 2; offsetY++)
            {
                for (int offsetX = -2; offsetX <= 2; offsetX++)
                {
                    int currentPixel = pixel + (offsetY * texWidth + offsetX) * 4;

                    if (currentPixel >= 0 && currentPixel < pixels.Length)
                    {
                        pixels[currentPixel] = color.r;
                        pixels[currentPixel + 1] = color.g;
                        pixels[currentPixel + 2] = color.b;
                        pixels[currentPixel + 3] = color.a;
                    }
                }
            }
        }
        private void DrawWithPixelTranslationAlgorithm(int pixel, int brushPixel)
        {
            for (int offsetY = -2; offsetY <= 2; offsetY++)
            {
                for (int offsetX = -2; offsetX <= 2; offsetX++)
                {
                    int currentPixel = pixel + (offsetY * texWidth + offsetX) * 4;

                    if (currentPixel >= 0 && currentPixel < pixels.Length)
                    {
                        pixels[currentPixel] = customBrushBytes[brushPixel];
                        pixels[currentPixel + 1] = customBrushBytes[brushPixel + 1];
                        pixels[currentPixel + 2] = customBrushBytes[brushPixel + 2];
                        pixels[currentPixel + 3] = customBrushBytes[brushPixel + 3];
                    }
                }
            }
        }
        #endregion

        #region ---> Logic draw circle <---
        private void DrawCircle(int x, int y)
        {
            int pixel = 0;
            for (int i = 0; i < brushSizeX4; i++)
            {
                int tx = (i % brushSizeX1) - brushSize;
                int ty = (i / brushSizeX1) - brushSize;

                if (tx * tx + ty * ty > brushSizeXbrushSize) continue;
                if (x + tx < 0 || y + ty < 0 || x + tx >= texWidth || y + ty >= texHeight) continue; // temporary fix for corner painting

                if (IsPixelInRegion(x + tx, y + ty, regionIndex))
                {
                    pixel = (texWidth * (y + ty) + x + tx) << 2;
                    if (maskPixels[pixel + 3] == 0)
                    {
                        DrawWithPixelTranslationAlgorithm(pixel, paintColor);
                    }
                }
            } // for area
        }
        private void DrawLine(int startX, int startY, int endX, int endY)
        {
            int x1 = endX;
            int y1 = endY;
            int tempVal = x1 - startX;
            int dx = (tempVal + (tempVal >> 31)) ^ (tempVal >> 31);
            tempVal = y1 - startY;
            int dy = (tempVal + (tempVal >> 31)) ^ (tempVal >> 31);


            int sx = startX < x1 ? 1 : -1;
            int sy = startY < y1 ? 1 : -1;
            int err = dx - dy;
            int pixelCount = 0;
            int e2;
            for (; ; ) // endless loop
            {

                pixelCount++;
                if (pixelCount > brushSizeDiv4)
                {
                    pixelCount = 0;
                    DrawCircle(startX, startY);
                }

                if (startX == x1 && startY == y1) break;
                e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    startX = startX + sx;
                }
                else if (e2 < dx)
                {
                    err = err + dx;
                    startY = startY + sy;
                }
            }
        } 
        private void DrawLine(Vector2 start, Vector2 end)
        {
            DrawLine((int)start.x, (int)start.y, (int)end.x, (int)end.y);
        }
        #endregion

        #region ---> Logic draw custom brush <---
        public void ReadCurrentCustomBrush()
        {
            customBrushWidth = customBrushes[selectedBrush].width;
            customBrushHeight = customBrushes[selectedBrush].height;
            customBrushBytes = new byte[customBrushWidth * customBrushHeight * 4];
            int pixel = 0;
            Color32[] brushPixel = customBrushes[selectedBrush].GetPixels32();
            for (int i = 0; i < brushPixel.Length; i++)
            {
                if (brushPixel[i].a != 0)
                {
                    brushPixel[i] = paintColor;
                }
            }
            for (int y = 0; y < customBrushHeight; y++)
            {
                for (int x = 0; x < customBrushWidth; x++)
                {
                    customBrushBytes[pixel] = brushPixel[x + y * customBrushWidth].r;
                    customBrushBytes[pixel + 1] = brushPixel[x + y * customBrushWidth].g;
                    customBrushBytes[pixel + 2] = brushPixel[x + y * customBrushWidth].b;
                    customBrushBytes[pixel + 3] = brushPixel[x + y * customBrushWidth].a;
                    pixel += 4;
                }
            }
            customBrushWidthHalf = (int)(customBrushWidth * 0.5f);
            customBrushHeightHalf = (int)(customBrushHeight * 0.5f);
        }
        private void DrawCustomBrush(int px, int py)
        {
            int startX = Mathf.Max(px - customBrushWidthHalf, 0);
            int startY = Mathf.Max(py - customBrushHeightHalf, 0);

            int endX = Mathf.Min(startX + customBrushWidth, texWidth);
            int endY = Mathf.Min(startY + customBrushHeight, texHeight);
            if (startX == 0 && startY == 0)
            {
                DrawCircle(startX, startY);
            }
            else
            {
                for (int y = startY; y < endY; y++)
                {
                    for (int x = startX; x < endX; x++)
                    {
                        int brushPixel = ((y - startY) * customBrushWidth + (x - startX)) << 2;
                        int pixel = (texWidth * y + x) << 2;
                        if (customBrushBytes[brushPixel + 3] > 0)
                        {
                            if (IsPixelInRegion(x, y, regionIndex))
                            {
                                if (maskPixels[pixel + 3] > 0 && maskPixels[pixel + 3] < 1)
                                {
                                    DrawWithPixelTranslationAlgorithm(pixel, brushPixel);
                                }
                                pixels[pixel] = customBrushBytes[brushPixel];
                                pixels[pixel + 1] = customBrushBytes[brushPixel + 1];
                                pixels[pixel + 2] = customBrushBytes[brushPixel + 2];
                                pixels[pixel + 3] = customBrushBytes[brushPixel + 3];
                            }
                        }
                    }
                }
            }
        }
        private void DrawLineWithBrush(Vector2 start, Vector2 end)
        {
            int x0 = (int)start.x;
            int y0 = (int)start.y;
            int x1 = (int)end.x;
            int y1 = (int)end.y;
            int tempVal = x1 - x0;
            int dx = (tempVal + (tempVal >> 31)) ^ (tempVal >> 31);
            tempVal = y1 - y0;
            int dy = (tempVal + (tempVal >> 31)) ^ (tempVal >> 31);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            int pixelCount = 0;
            int e2;
            for (; ; )
            {

                pixelCount++;
                if (pixelCount > brushSizeDiv4)
                {
                    pixelCount = 0;
                    DrawCustomBrush(x0, y0);
                }
                if (x0 == x1 && y0 == y1) break;
                e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }
                else if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }
            }
        }
        #endregion

        #region ---> Logic Erase <---
        private void EraseWithBackgroundColorLine(Vector2 start, Vector2 end)
        {
            int x0 = (int)start.x;
            int y0 = (int)start.y;
            int x1 = (int)end.x;
            int y1 = (int)end.y;
            int tempVal = x1 - x0;
            int dx = (tempVal + (tempVal >> 31)) ^ (tempVal >> 31); // http://stackoverflow.com/questions/6114099/fast-integer-abs-function
            tempVal = y1 - y0;
            int dy = (tempVal + (tempVal >> 31)) ^ (tempVal >> 31);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            int pixelCount = 0;
            int e2;
            for (; ; )
            {

                pixelCount++;
                if (pixelCount > brushSizeDiv4)
                {
                    pixelCount = 0;
                    EraseWithBackgroundColor(x0, y0);
                }

                if ((x0 == x1) && (y0 == y1)) break;
                e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    x0 = x0 + sx;
                }
                else if (e2 < dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }
            }
        }
        public void EraseWithBackgroundColor(int x, int y)
        {
            var origColor = paintColor;
            paintColor = clearColor;
            //var origSize = brushSize; // optional, have fixed eraser brush size temporarily while drawing
            //SetBrushSize(defaultEraserSize);
            DrawCircle(x, y);
            //SetBrushSize(origSize);
            paintColor = origColor;
        }
        public void ClearImage()
        {
            if (usingClearingImage)
            {
                ClearImageWithImage();
            }
            else
            {

                int pixel = 0;
                for (int y = 0; y < texHeight; y++)
                {
                    for (int x = 0; x < texWidth; x++)
                    {
                        pixels[pixel] = clearColor.r;
                        pixels[pixel + 1] = clearColor.g;
                        pixels[pixel + 2] = clearColor.b;
                        pixels[pixel + 3] = clearColor.a;
                        pixel += 4;
                    }
                }

                UpdateTexture();
            }
        } 
        private void ClearImageWithImage()
        {
            System.Array.Copy(clearPixels, 0, pixels, 0, clearPixels.Length);
            drawingTexture.LoadRawTextureData(clearPixels);
            drawingTexture.Apply(false);
        } 
        private void ReadClearingImage()
        {
            clearPixels = new byte[texWidth * texHeight * 4];

            drawingTexture.SetPixels32(((Texture2D)myRenderer.material.GetTexture(MAIN_TEXTURE)).GetPixels32());
            drawingTexture.Apply(false);

            int pixel = 0;
            Color32[] tempPixels = drawingTexture.GetPixels32();
            int tempCount = tempPixels.Length;

            for (int i = 0; i < tempCount; i++)
            {
                clearPixels[pixel] = tempPixels[i].r;
                clearPixels[pixel + 1] = tempPixels[i].g;
                clearPixels[pixel + 2] = tempPixels[i].b;
                clearPixels[pixel + 3] = tempPixels[i].a;
                pixel += 4;
            }
        }
        #endregion

        private byte[] ConvertToByteArray(Color32[] colors)
        {
            byte[] bytes = new byte[colors.Length * 4]; 

            for (int i = 0; i < colors.Length; i++)
            {
                bytes[i * 4] = colors[i].r;
                bytes[i * 4 + 1] = colors[i].g;
                bytes[i * 4 + 2] = colors[i].b;
                bytes[i * 4 + 3] = colors[i].a;
            }

            return bytes;
        }
       /* private float CalculatePercentage()
        {
            if (pixels == null || pixels.Length == 0)
            {
                LogMe.LogError("No pixels data available.");
                return 0f;
            }

            int totalPixels = pixels.Length / 4;
            int nonBlackWhitePixels = 0;

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte r = pixels[i];
                byte g = pixels[i + 1];
                byte b = pixels[i + 2];

                if (!(r == 0 && g == 0 && b == 0) && !(r == 255 && g == 255 && b == 255))
                {
                    nonBlackWhitePixels++;
                }
            }

            float percentage = (float)nonBlackWhitePixels / totalPixels * 100f;
            return percentage;
        }*/
        void OnDestroy()
        {
            if (drawingTexture != null) Texture2D.DestroyImmediate(drawingTexture, true);
            pixels = null;
            IsDrawing = false;
            maskPixels = null;
            clearPixels = null;
        }
    }
}