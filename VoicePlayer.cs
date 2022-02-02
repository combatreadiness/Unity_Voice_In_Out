using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class VoicePlayer : MonoBehaviour
{
    bool IsDestroyed = false;

    public Queue<byte[]> queue = new Queue<byte[]>();

    private AudioSource audioSrc;

    int delay = 0;
    
    private const int frequency = 44100;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = gameObject.AddComponent<AudioSource>();
        runTask();
    }

 public async void runTask()
    {
        await Task.Delay(1000);

       while (!IsDestroyed)
        {
            Play();
            await Task.Delay(1);
        }
        return;
    }
    
    private void Play()
    {
         if (queue.Count > 0)
            {
                byte[] bytes = queue.Dequeue();
                
                float[] data = ToFloatArray(bytes);
                //Debug.Log("### AudioPlayManager > play data: " + bytes.Length);
                AudioClip clip = AudioClip.Create("", data.Length, 1, frequency, false);
                clip.SetData(data, 0);

                audioSrc.loop = false;
                audioSrc.clip = clip;
                delay = (int)(clip.length*1000);
                audioSrc.Play();
            }
        else if (queue.Count > 5)
        {
            queue.Clear();
            delay = 0;
        }
        else
        {
        
            delay = 0;
        }

    }

    public static float[] ToFloatArray(byte[] byteArray)
    {
        int len = byteArray.Length / sizeof(float);
        float[] floatArray = new float[len];
        Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
        return floatArray;
    }

    private void OnDestroy()
    {
        IsDestroyed = true;
    }
}

