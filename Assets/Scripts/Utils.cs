using Mediapipe;
using Mediapipe.Tasks.Components.Containers;
using System;
using System.Collections.Generic;

public static class LandmarkUtils
{
    /// <summary>
    /// Converts a list of NormalizedLandmark into a NormalizedLandmarkList.
    /// </summary>
    public static NormalizedLandmarkList ToLandmarkList(IReadOnlyList<NormalizedLandmarks> normalizedLandmarks)
    {
        var landmarkList = new NormalizedLandmarkList();

        if (normalizedLandmarks != null)
        {
            foreach (var landmarks in normalizedLandmarks)
            {
                foreach (var landmark in landmarks.landmarks)
                {
                    if (landmark != null)
                        {
                        landmarkList.Landmark.Add(new Mediapipe.NormalizedLandmark
                        {
                            X = landmark.x,
                            Y = landmark.y,
                            Z = landmark.z,
                            Visibility = landmark.visibility == null ? 0 : (float)landmark.visibility
                        });
                    }
                }
            }
        }

        return landmarkList;
    }

    /// <summary>
    /// Converts raw float arrays into a NormalizedLandmarkList.
    /// Each entry in coords should be (x, y, z, visibility).
    /// </summary>
    public static NormalizedLandmarkList FromFloatArray(float[] coords)
    {
        var landmarkList = new NormalizedLandmarkList();

        if (coords == null || coords.Length % 4 != 0)
            throw new System.ArgumentException("Coordinate array length must be a multiple of 4 (x, y, z, visibility).");

        for (int i = 0; i < coords.Length; i += 4)
        {
            landmarkList.Landmark.Add(new Mediapipe.NormalizedLandmark
            {
                X = coords[i],
                Y = coords[i + 1],
                Z = coords[i + 2],
                Visibility = coords[i + 3]
            });
        }

        return landmarkList;
    }
}