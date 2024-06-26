﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace TrolleyTest
{
    //CODE PARTIALLY BASED ON https://github.com/0xDrMoe/TownofHost-Enhanced/blob/main/Modules/Utils.cs#L2866

    public static class Utils
    {

        public static void AddHat(string prodId, Sprite MainImage, Sprite FloorImage, bool inFront)
        {
            HatBehaviour hat = new HatBehaviour();
            hat.InFront = inFront;
            hat.MainImage = MainImage;
            hat.FloorImage = FloorImage;
            hat.ProductId = prodId;
            hat.StoreName = "";
            HatManager.Instance.AllHats.Add(hat);
        }

        public static Sprite LoadSprite(string path, float pixelsPerUnit = 1f)
        {
            Sprite sprite = null;
            try
            {
                Texture2D texture = LoadTextureFromResources(path);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
                sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading sprite: " + e.Message);
            }
            return sprite;
        }

        public static Texture2D LoadTextureFromResources(string path)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            ImageConversion.LoadImage(texture, ms.ToArray(), false);
            return texture;
        }

        public static AudioClip LoadAudioClipFromResources(string path, int bytesPerSample)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);

            AudioClip loadedAudioClip = AudioUtility.ToAudioClip(ms.ToArray(), 44100, bytesPerSample, "LoadedAudioClip");

            return loadedAudioClip;
        }
    }
}
