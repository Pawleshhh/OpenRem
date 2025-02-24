﻿using OpenRem.Common;

namespace OpenRem.Engine;

public class AudioSample
{
    public AudioSample(byte[] rawData, PcmEncoding pcmEncoding, int channelsCount)
    {
        RawData = rawData;
        PcmEncoding = pcmEncoding;
        ChannelsCount = channelsCount;
    }

    public byte[] RawData { get; }
    public PcmEncoding PcmEncoding { get; }
    public int ChannelsCount { get; }

    public AudioSample ToMono(Side side)
    {
        if (ChannelsCount != 2)
        {
            throw new InvalidOperationException("You can only convert stereo sample into mono");
        }

        if (RawData.Length < 2 || RawData.Length % 2 != 0)
        {
            throw new InvalidOperationException("Raw data must be a multiple of 2");
        }

        int dataSize = PcmEncodingHelper.ToByteLength(PcmEncoding);
        int offset = side == Side.Left ? 0 : (RawData.Length / 2);
        var monoSample = new List<byte>(dataSize);
        for (int i = 0; i < dataSize; i++)
        {
            monoSample.Add(RawData[offset + i]);
        }

        return new AudioSample(monoSample.ToArray(), PcmEncoding, 1);
    }
}