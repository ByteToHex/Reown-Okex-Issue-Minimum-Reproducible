//	Copyright (c) 2012 Calvin Rien
//        http://the.darktable.com
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
//  derived from Gregorio Zanon's script
//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734
//
// =============================================================================
//
// This script has been modified by DTT.
// https://www.d-tt.nl/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SavWav
{
    private const int HEADER_SIZE = 44;

    public static void Save(string filename, string path, AudioClip clip)
    {
        if (!filename.ToLower().EndsWith(".wav"))
            filename += ".wav";

        string filepath = Path.Combine(path, filename);

        // Make sure directory exists if user is saving to sub dir.
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));

        using (FileStream fileStream = CreateEmpty(filepath))
        {
            ConvertAndWrite(fileStream, clip);

            WriteHeader(fileStream, clip);
        }
    }

    public static AudioClip TrimSilence(AudioClip clip, float min)
    {
        float[] samples = new float[clip.samples];

        clip.GetData(samples, 0);

        return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
    }

    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz) => TrimSilence(samples, min, channels, hz, false, false);


    public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream)
    {
        int i;

        for (i = 0; i < samples.Count; i++)
            if (Mathf.Abs(samples[i]) > min)
                break;

        if (samples.Count > i)
            samples.RemoveRange(0, i);

        for (i = samples.Count - 1; i > 0; i--)
            if (Mathf.Abs(samples[i]) > min)
                break;

        if (samples.Count > (samples.Count - i))
            samples.RemoveRange(i, samples.Count - i);

        AudioClip clip = AudioClip.Create("TempClip", samples.Count, channels, hz, stream);

        clip.SetData(samples.ToArray(), 0);

        return clip;
    }

    private static FileStream CreateEmpty(string filepath)
    {
        FileStream fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        // Preparing the header.
        for (int i = 0; i < HEADER_SIZE; i++)
            fileStream.WriteByte(emptyByte);

        return fileStream;
    }

    private static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        // Modification that fixes the length of the saved WAV.
        float[] samples = new float[clip.samples * clip.channels];

        clip.GetData(samples, 0);

        // Converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]
        Int16[] intData = new Int16[samples.Length];

        // bytesData array is twice the size ofd ataSource array because a float converted in Int16 is 2 bytes.
        Byte[] bytesData = new Byte[samples.Length * 2];

        // To convert float to Int16.
        float rescaleFactor = 32767; 

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    public static void WriteHeader(Stream fileStream, AudioClip clip)
    {
        int hz = clip.frequency;
        int channels = clip.channels;
        int samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); 
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);
    }
}