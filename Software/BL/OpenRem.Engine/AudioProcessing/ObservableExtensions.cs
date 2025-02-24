﻿using OpenRem.Common;
using System.Reactive.Linq;

namespace OpenRem.Engine;

public static class ObservableExtensions
{
    public static IObservable<AudioSample> GetSample(this IObservable<byte> observable, PcmEncoding encoding,
        int channelsCount)
    {
        int bytesPerSide = PcmEncodingHelper.ToByteLength(encoding);
        int bufferSize = bytesPerSide * channelsCount;
        return observable.Buffer(bufferSize).Select(x => new AudioSample(x.ToArray(), encoding, channelsCount));
    }

    public static IObservable<AudioSample> StereoSample(this IObservable<byte> observable, PcmEncoding encoding)
    {
        return observable.GetSample(encoding, 2);
    }

    public static IObservable<AudioSample> ChannelSample(this IObservable<AudioSample> observable, int channel)
    {
        if (channel < 0 || channel > 1)
        {
            throw new InvalidOperationException("Currently not supported channel higher than two");
        }

        if (channel == 0)
        {
            return observable.SideSample(Side.Left);
        }

        return observable.SideSample(Side.Right);
    }

    public static IObservable<AudioSample> SideSample(this IObservable<AudioSample> observable, Side side)
    {
        return observable.Select(sample => sample.ToMono(side));
    }
}