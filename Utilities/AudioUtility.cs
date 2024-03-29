using UnityEngine;

public static class AudioUtility
{
    public static AudioClip ToAudioClip(byte[] audioData, int sampleRate, int bytesSample, string name = "AudioClip")
    {
        float[] floatData = ConvertByteArrayToFloatArray(audioData, bytesSample);
        AudioClip audioClip = ToAudioClipInternal(floatData, sampleRate, name);
        return audioClip;
    }

    private static float[] ConvertByteArrayToFloatArray(byte[] audioData, int bytesPerSample)
    {
        int floatArrayLength = audioData.Length / bytesPerSample;
        float[] floatData = new float[floatArrayLength];
        for (int i = 0; i < floatArrayLength; i++)
        {
            floatData[i] = (float)System.BitConverter.ToInt16(audioData, i * bytesPerSample) / 32768.0f;
        }
        return floatData;
    }

    private static AudioClip ToAudioClipInternal(float[] floatData, int sampleRate, string name)
    {
        int lengthSamples = floatData.Length;
        AudioClip audioClip = AudioClip.Create(name, lengthSamples, 1, sampleRate, false);
        audioClip.SetData(floatData, 0);
        return audioClip;
    }
}
