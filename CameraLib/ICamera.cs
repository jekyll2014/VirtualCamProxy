using OpenCvSharp;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CameraLib;

/// <summary>
/// Represents a camera device that can capture images.
/// </summary>
public interface ICamera : IDisposable
{
    public CameraDescription Description { get; set; }
    public bool IsRunning { get; }
    public FrameFormat? CurrentFrameFormat { get; }
    public double CurrentFps { get; }
    public int FrameTimeout { get; set; }

    /// <summary>
    /// Raised when the camera connects successfully.
    /// </summary>
    public delegate void CameraConnectedEventHandler(ICamera camera);
    public event CameraConnectedEventHandler? CameraConnectedEvent;

    /// <summary>
    /// Raised when the camera disconnects.
    /// </summary>
    public delegate void CameraDisconnectedEventHandler(ICamera camera);
    public event CameraDisconnectedEventHandler? CameraDisconnectedEvent;

    /// <summary>
    /// Raised when a new image frame is captured.
    /// </summary>
    /// <remarks>
    /// The Mat image is cloned before being passed to subscribers. Each subscriber receives
    /// their own copy and is responsible for disposing it when done.
    /// </remarks>
    public delegate void ImageCapturedEventHandler(ICamera camera, Mat image);
    public event ImageCapturedEventHandler? ImageCapturedEvent;

    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Retrieves image metadata from the camera.
    /// </summary>
    /// <param name="discoveryTimeout">Timeout in milliseconds.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public Task<bool> GetImageDataAsync(int discoveryTimeout = 5000);

    /// <summary>
    /// Discovers available cameras of this type.
    /// </summary>
    /// <param name="discoveryTimeout">Timeout in milliseconds.</param>
    /// <returns>List of discovered cameras.</returns>
    public List<CameraDescription> DiscoverCameras(int discoveryTimeout);

    /// <summary>
    /// Starts the camera with specified parameters.
    /// </summary>
    /// <param name="width">Desired frame width (0 for default).</param>
    /// <param name="height">Desired frame height (0 for default).</param>
    /// <param name="format">Desired pixel format (empty for default).</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>True if started successfully, false otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Width or height is negative.</exception>
    /// <exception cref="CameraConnectionException">Failed to connect to camera.</exception>
    /// <exception cref="ObjectDisposedException">Camera has been disposed.</exception>
    /// <remarks>
    /// This method is thread-safe but should not be called concurrently with Stop().
    /// Events (ImageCapturedEvent) fire on background threads; marshal to UI as needed.
    /// </remarks>
    public Task<bool> StartAsync(int width, int height, string format, CancellationToken token);

    /// <summary>
    /// Stops the camera.
    /// </summary>
    public void Stop();

    /// <summary>
    /// Grabs a single frame from the camera.
    /// </summary>
    /// <param name="token">Cancellation token.</param>
    /// <param name="width">Desired frame width (0 for current).</param>
    /// <param name="height">Desired frame height (0 for current).</param>
    /// <param name="format">Desired pixel format (empty for current).</param>
    /// <returns>The captured frame (caller must dispose), or null if capture failed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Width or height is negative.</exception>
    /// <exception cref="FrameCaptureException">Frame capture failed or timed out.</exception>
    /// <exception cref="ObjectDisposedException">Camera has been disposed.</exception>
    /// <exception cref="OperationCanceledException">Operation was canceled via token.</exception>
    /// <remarks>
    /// Caller is responsible for disposing the returned Mat.
    /// Returns null if capture fails; check camera connection if this occurs repeatedly.
    /// </remarks>
    public Task<Mat?> GrabFrameAsync(CancellationToken token, int width, int height, string format);

    /// <summary>
    /// Creates an async stream of frames from the camera.
    /// </summary>
    /// <param name="token">Cancellation token to stop the stream.</param>
    /// <returns>Async enumerable of Mat frames (caller must dispose each frame).</returns>
    /// <exception cref="ObjectDisposedException">Camera has been disposed.</exception>
    /// <remarks>
    /// Each Mat must be disposed by the caller. Use 'await foreach' with 'using':
    /// <code>
    /// await foreach (var frame in camera.GrabFrames(token))
    /// {
    ///     using (frame)
    ///     {
    ///         // Process frame
    ///     }
    /// }
    /// </code>
    /// </remarks>
    public IAsyncEnumerable<Mat> GrabFrames(CancellationToken token);

    /// <summary>
    /// Finds the closest matching format to the requested parameters.
    /// </summary>
    /// <param name="width">Desired width.</param>
    /// <param name="height">Desired height.</param>
    /// <param name="format">Desired format.</param>
    /// <returns>The nearest matching FrameFormat.</returns>
    public FrameFormat GetNearestFormat(int width, int height, string format);
}
