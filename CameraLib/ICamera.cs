﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using OpenCvSharp;

namespace CameraLib
{
    public interface ICamera : IDisposable
    {
        public CameraDescription Description { get; set; }
        public bool IsRunning { get; }
        public FrameFormat? CurrentFrameFormat { get; }
        public double CurrentFps { get; }
        public int FrameTimeout { get; set; }

        public delegate void ImageCapturedEventHandler(ICamera camera, Mat image);
        public event ImageCapturedEventHandler? ImageCapturedEvent;

        public CancellationToken CancellationToken { get; }

        public Task<bool> GetImageData(int discoveryTimeout = 5000);
        public List<CameraDescription> DiscoverCameras(int discoveryTimeout);
        public Task<bool> Start(int width, int height, string format, CancellationToken token);
        public void Stop();
        public Task<Mat?> GrabFrame(CancellationToken token);
        public IAsyncEnumerable<Mat> GrabFrames(CancellationToken token);
        public FrameFormat GetNearestFormat(int width, int height, string format);
    }
}