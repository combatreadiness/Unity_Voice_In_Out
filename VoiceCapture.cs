using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class VoiceCapture : MonoBehaviour
{

    bool IsDestroyed = false;
    public const int lengthSec = 60;
    public const int frequency = 44100;

    
    private AudioClip mic;
    private int curPos, lastPos;

    Queue<byte[]> queue;
    void Start()
    {
        int minfreq, maxfreq;
        
        queue = gameObject.GetComponent<VoicePlayer>().queue;
        
        Microphone.GetDeviceCaps(null, out minfreq, out maxfreq);
        Debug.Log("minfreq : " + minfreq);
        Debug.Log("maxfreq : " + maxfreq);

        mic = Microphone.Start(null, true, lengthSec, frequency);

        runTask();

    }

    public async void runTask()
    {
       while (!IsDestroyed)
        {
             Record();

            await Task.Delay(100);
        }
        return;

    }

 private void Record() {

        if ((curPos = Microphone.GetPosition(null)) > 0)
        {
            if (lastPos > curPos)
                lastPos = 0;

            if (curPos - lastPos > 0)
            {
                int len = (curPos - lastPos) * mic.channels;

                float[] data = new float[len];
                mic.GetData(data, lastPos);

                byte[] bytes = ToByteArray(data);

//                Debug.Log("### MicrophoneManager > mic data: " + bytes.Length);
                queue.Enqueue(bytes);

            }
            lastPos = curPos;
        }
    }
    public static byte[] ToByteArray(float[] floatArray)
    {
        int len = floatArray.Length * sizeof(float);
        byte[] byteArray = new byte[len];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    private void OnDestroy()
    {
        IsDestroyed = true;
    }
}
