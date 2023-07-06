using BepInEx;
using CSCore;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Utilla;
using UnityEngine.Windows.Speech;
using Photon.Pun;
using System.IO;
using Photon.Voice.PUN;
using POpusCodec.Enums;
using System.Runtime.InteropServices;
using GorillaLocomotion;
using GorillaNetworking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using ExitGames.Client.Photon;
using System.Collections;

namespace Meow
{
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private GameObject CatAsset;
        private GameObject Cat;

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            var Path = Assembly.GetExecutingAssembly().GetManifestResourceStream("Meow.Resources.meow");
            var Bundle = AssetBundle.LoadFromStream(Path);
            CatAsset = Bundle.LoadAsset("meow") as GameObject;

            Cat = Instantiate(CatAsset);
            Cat.transform.position = new Vector3(-59.1267f, 14.3f, - 41.5695f);
            Cat.transform.localScale = new Vector3(7f, 7f, 7f);
            Cat.transform.eulerAngles = new Vector3(31.3691f, 181.8325f, 340.4064f);
            GorillaSurfaceOverride Override0 = Cat.AddComponent<GorillaSurfaceOverride>();
            Override0.extraVelMultiplier = 1.3f;
            Override0.extraVelMaxMultiplier = 1.3f;
            Override0.overrideIndex = 3;

            Cat.GetComponent<MeshCollider>().isTrigger = true;
        }

        private AudioClip purrSound;
        private AudioSource audioSource;

        private bool isPurrSoundDownloaded = false;
        private string purrSoundFilePath = Path.Combine(Paths.PluginPath, "purrloop.wav");

        System.Collections.IEnumerator DownloadPurr()
        {
            if (isPurrSoundDownloaded)
            {
                purrSound = GetAudioClipFromFile(purrSoundFilePath);
                PlayPurr();
                yield break;
            }

            UnityWebRequest www = UnityWebRequest.Get("https://github.com/YourGitHub/YourRepo/raw/main/YourFolder/purrloop.wav");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("Failed to download purr sound: " + www.error);
                yield break;
            }

            File.WriteAllBytes(purrSoundFilePath, www.downloadHandler.data);

            purrSound = GetAudioClipFromFile(purrSoundFilePath);
            isPurrSoundDownloaded = true;

            PlayPurr();
        }

        void SavepurrSoundToFile(AudioClip audioClip)
        {
            var audioData = UnityEngine.AudioCompressionUtility.GetPCM(audioClip, false);
            File.WriteAllBytes(purrSoundFilePath, audioData);
        }

        AudioClip GetAudioClipFromFile(string filePath)
        {
            var audioData = File.ReadAllBytes(filePath);
            var audioClip = AudioClip.Create("PurrSound", audioData.Length / 2, 1, 44100, false);
            audioClip.SetData(ConvertByteArrayToFloatArray(audioData), 0);
            return audioClip;
        }

        float[] ConvertByteArrayToFloatArray(byte[] byteArray)
        {
            float[] floatArray = new float[byteArray.Length / 2];
            for (int i = 0; i < floatArray.Length; i++)
            {
                floatArray[i] = (float)BitConverter.ToInt16(byteArray, i * 2) / 32768.0f;
            }
            return floatArray;
        }

        void PlayPurr()
        {
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
            }

            audioSource.clip = purrSound;
            audioSource.Play();
        }
    }
}