using System;

namespace CameraLib;

/// <summary>
/// Base exception for all camera-related errors.
/// </summary>
public class CameraException : Exception
{
    public CameraException(string message) : base(message) { }
    public CameraException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Thrown when a camera connection attempt fails.
/// </summary>
public class CameraConnectionException : CameraException
{
    public CameraConnectionException(string message) : base(message) { }
    public CameraConnectionException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Thrown when a camera disconnects unexpectedly during operation.
/// </summary>
public class CameraDisconnectedException : CameraException
{
    public CameraDisconnectedException(string message) : base(message) { }
    public CameraDisconnectedException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Thrown when attempting to capture a frame fails due to timeout or device error.
/// </summary>
public class FrameCaptureException : CameraException
{
    public FrameCaptureException(string message) : base(message) { }
    public FrameCaptureException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Thrown when the requested frame format is not supported by the camera.
/// </summary>
public class UnsupportedFormatException : CameraException
{
    public int RequestedWidth { get; }
    public int RequestedHeight { get; }
    public string RequestedFormat { get; }

    public UnsupportedFormatException(int width, int height, string format)
        : base($"Camera does not support format {width}x{height} ({format})")
    {
        RequestedWidth = width;
        RequestedHeight = height;
        RequestedFormat = format;
    }
}
