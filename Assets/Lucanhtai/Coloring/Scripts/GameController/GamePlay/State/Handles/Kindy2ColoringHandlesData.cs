using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace Lucanhtai.Coloring
{
    public class Kindy2ColoringHandlesData
    {
        private Kindy2ColoringDataColor dataColor;
        public void InitData(Kindy2ColoringDataColor dataColor)
        {
            this.dataColor = dataColor;
        }
        public int GetIndexByName(string itemName)
        {
            string[] objectItem = itemName.Split("_");
            return int.Parse(objectItem[1]);
        }

        public BaseButton GetButtonByName(List<BaseButton> list, string nameObject)
        {
            foreach (var item in list)
            {
                if (item.name.Equals(nameObject))
                {
                    return item;
                }
            }
            return null;
        }

        public void SelectedItemButton(ColoringImageColor item, int indexColor)
        {
            item.SetImageColor(dataColor.colorSelectWhite, dataColor.colorButtons[indexColor]);
        }
        public void UnSelectedItemButton(ColoringImageColor item)
        {
            item.SetImageColor(dataColor.colorSelectBrown, dataColor.colorSelectWhite);
        }
        public void SelectedItemPaint(ColoringDrawingPen item)
        {
            item.SetColorPen(dataColor.colorSelectBlue,dataColor.colorSelectWhite, dataColor.colorSelectWhite);
        }
        public void UnSelectedItemPaint(ColoringDrawingPen item)
        {
            item.SetColorPen(dataColor.colorSelectBrown, dataColor.colorSelectBrownBorder, dataColor.colorSelectBrownBorder);
        }
        public void ChangeColorPen(ColoringDrawingPen item, Color color)
        {
            item.SetColor(color);
        }
        public void EnableListButton(List<BaseButton> list, bool isEnable)
        {
            foreach (var item in list)
            {
                item.Enable(isEnable);
            }
        }
        public Color GetCurrentColorSelected(List<BaseButton> list)
        {
            foreach (var item in list)
            {
                if (item.GetComponent<Image>().color == dataColor.colorSelectWhite)
                {
                    return item.GetComponent<ColoringImageColor>().GetCurrentColor();
                }
            }
            return dataColor.colorSelectRed;

        }
        public void DeleteMusicBackground()
        {
            GameObject obj = GameObject.Find("Nhac BG");
            if (obj != null) GameObject.Destroy(obj, .1f);
        }
        public async void SaveStatusPlayGame(int idImage)
        {
            string filePath = Application.persistentDataPath + "/coloring_status.json";
            List<ImageDataStatus> imageStatuses;
            string dataJson = ReadFileJsonFromAppdataFullPath(filePath);

            if (!string.IsNullOrEmpty(dataJson))
                imageStatuses = JsonConvert.DeserializeObject<List<ImageDataStatus>>(dataJson);
            else
                imageStatuses = new List<ImageDataStatus>();

            var existingStatus = imageStatuses.Find(status => status.IdImage == idImage.ToString());
            if (existingStatus == null)
                imageStatuses.Add(new ImageDataStatus { IdImage = idImage.ToString(), IsStatus = true });

            string newJson = JsonConvert.SerializeObject(imageStatuses);
            await File.WriteAllTextAsync(filePath, newJson);
        }

        public bool CheckImageStatus(int idImage)
        {
            string filePath = Application.persistentDataPath + "/coloring_status.json";
            string dataJson = ReadFileJsonFromAppdataFullPath(filePath);
            if (!string.IsNullOrEmpty(dataJson))
            {
                var imageStatuses = JsonConvert.DeserializeObject<List<ImageDataStatus>>(dataJson);
                if (imageStatuses == null) return false;
                foreach (var imageStatus in imageStatuses)
                {
                    if (imageStatus.IdImage.Equals(idImage.ToString()) && imageStatus.IsStatus.ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool CheckImageDataSave(int idImage)
        {
            string filePath = Application.persistentDataPath + "/savedPixelsColoringGame_" + idImage.ToString() + ".png";

            if (File.Exists(filePath))
            {
                return true;
            }
            return false;
        }
        public static string ReadFileJsonFromAppdataFullPath(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return "";
            }

            return File.ReadAllText(path);
        }

    }


    public class ImageDataStatus
    {
        [JsonProperty("id_image", NullValueHandling = NullValueHandling.Ignore)] public string IdImage { set; get; }
        [JsonProperty("is_status", NullValueHandling = NullValueHandling.Ignore)] public bool IsStatus { set; get; }
    }

}