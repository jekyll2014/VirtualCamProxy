using System;
using System.Collections.Generic;
using System.Linq;

namespace CameraLib;

/// <summary>
/// Helper class providing common functionality for camera implementations.
/// </summary>
public static class CameraHelpers
{
    /// <summary>
    /// Finds the closest matching format to the requested parameters.
    /// </summary>
    /// <param name="availableFormats">Available frame formats from the camera.</param>
    /// <param name="width">Desired width.</param>
    /// <param name="height">Desired height.</param>
    /// <param name="format">Desired format string.</param>
    /// <returns>The nearest matching FrameFormat, or a default empty format if no formats are available.</returns>
    public static FrameFormat GetNearestFormat(IEnumerable<FrameFormat> availableFormats, int width, int height, string format)
    {
        var formats = availableFormats.ToArray();

        if (formats.Length == 0)
            return new FrameFormat(0, 0);

        if (formats.Length == 1)
            return formats[0];

        // Select format based on resolution
        FrameFormat? selectedFormat;
        if (width > 0 && height > 0)
        {
            var mpix = width * height;
            selectedFormat = formats.MinBy(n => Math.Abs(n.Width * n.Height - mpix));
        }
        else
        {
            selectedFormat = formats.MaxBy(n => n.Width * n.Height);
        }

        // Filter formats with matching resolution
        var matchingResolution = formats
            .Where(n =>
                n.Width == (selectedFormat?.Width ?? 0)
                && n.Height == (selectedFormat?.Height ?? 0))
            .ToArray();

        if (matchingResolution.Length == 0)
            return new FrameFormat(0, 0);

        // Try to match format string if provided
        if (!string.IsNullOrEmpty(format))
        {
            var matchingFormat = matchingResolution
                .Where(n => n.Format == format)
                .ToArray();

            if (matchingFormat.Length != 0)
                matchingResolution = matchingFormat;
        }

        // Return format with highest FPS
        return matchingResolution.MaxBy(n => n.Fps) ?? matchingResolution[0];
    }

    /// <summary>
    /// Validates that width and height parameters are non-negative.
    /// </summary>
    /// <param name="width">Width to validate.</param>
    /// <param name="height">Height to validate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if width or height is negative.</exception>
    public static void ValidateDimensions(int width, int height)
    {
        if (width < 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width cannot be negative");

        if (height < 0)
            throw new ArgumentOutOfRangeException(nameof(height), height, "Height cannot be negative");
    }
}
