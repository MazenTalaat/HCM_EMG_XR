/*
 * MVIC_Client.cs
 * Attach this to any GameObject in your scene.
 *
 * - GetMVICValues()  : Coroutine that calls /api/values   (GET)   and returns float[]
 * - SendMVICValues() : Coroutine that calls /api/values/i (PUT)   for every element
 *
 * The Node server script must be running on the same machine (or adjust mvicServerBaseUrl for LAN).
 */

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class MVIC_Client : MonoBehaviour
{
    // Base URL for the MVIC server endpoint
    private string mvicServerBaseUrl = EndPoints.MVICServerUrl;

    /// <summary>
    /// Coroutine: Reads all 6 MVIC values from the server.
    /// </summary>
    /// <param name="onSuccess">Callback with float[] result</param>
    /// <param name="onError">Callback with error message</param>
    public IEnumerator GetMVICValues(Action<float[]> onSuccess, Action<string> onError = null)
    {
        using UnityWebRequest request = UnityWebRequest.Get($"{mvicServerBaseUrl}/api/values");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        float[] mvicValues = ParseFloatArray(request.downloadHandler.text);
        onSuccess?.Invoke(mvicValues);
    }

    /// <summary>
    /// Coroutine: Sends new MVIC values to the server (updates each index individually).
    /// </summary>
    /// <param name="mvicValues">Array of 6 MVIC float values</param>
    /// <param name="onSuccess">Callback on success</param>
    /// <param name="onError">Callback with error message</param>
    public IEnumerator SendMVICValues(float[] mvicValues,
                                      Action onSuccess = null,
                                      Action<string> onError = null)
    {
        if (mvicValues == null || mvicValues.Length != 6)
        {
            onError?.Invoke("MVIC array must contain exactly 6 floats.");
            yield break;
        }

        // The Node API updates one index at a time: PUT /api/values/:idx
        for (int i = 0; i < mvicValues.Length; i++)
        {
            string url = $"{mvicServerBaseUrl}/api/values/{i}";
            string jsonBody = $"{{\"value\":{mvicValues[i].ToString(CultureInfo.InvariantCulture)}}}";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            using UnityWebRequest request = new UnityWebRequest(url, "PUT");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Index {i} failed: {request.error}");
                yield break;
            }
        }

        onSuccess?.Invoke();
    }

    /// <summary>
    /// Coroutine: Reads the currently-selected muscle filter from the server.
    /// Returns -1 ? “All” (all six MVIC values are accepted).
    /// Returns 0-5 ? Only that muscle index is being stored.
    /// Endpoint exposed by the Node server: GET /api/current-muscle
    /// </summary>
    /// <param name="onSuccess">Callback with int result (-1 | 0-5)</param>
    /// <param name="onError">Callback with error message (optional)</param>
    public IEnumerator GetMuscleFilter(Action<int> onSuccess, Action<string> onError = null)
    {
        using UnityWebRequest request =
            UnityWebRequest.Get($"{mvicServerBaseUrl}/api/current-muscle");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(request.error);
            yield break;
        }

        // ----------------------------------------------------------------
        // Expected JSON payload: {"filter":"all"}   or   {"filter":3}
        // We parse it with a *tiny* manual routine to avoid extra packages.
        // ----------------------------------------------------------------
        string json = request.downloadHandler.text.Trim();
        int filterIndex = -1;                 // default = All

        try
        {
            // crude but sufficient: find the ':' then trim trailing chars
            int colon = json.IndexOf(':');
            if (colon > 0)
            {
                string valuePart = json
                    .Substring(colon + 1)
                    .TrimEnd('}', ' ', '\n', '\r', '\t')
                    .Trim('"');

                if (valuePart != "all")
                    int.TryParse(valuePart, out filterIndex); // becomes 0-5 if valid
            }
        }
        catch
        {
            // keep default (-1) on any parse failure
        }

        onSuccess?.Invoke(filterIndex);
    }


    /// <summary>
    /// Helper: Parses a JSON float array string (e.g. "[1.0,2.0,...]") into a float[].
    /// </summary>
    /// <param name="json">JSON array string</param>
    /// <returns>Parsed float array</returns>
    private static float[] ParseFloatArray(string json)
    {
        json = json.Trim().TrimStart('[').TrimEnd(']');
        string[] tokens = json.Split(',', StringSplitOptions.RemoveEmptyEntries);

        float[] result = new float[tokens.Length];
        for (int i = 0; i < tokens.Length; i++)
        {
            float.TryParse(tokens[i],
                           NumberStyles.Float,
                           CultureInfo.InvariantCulture,
                           out result[i]);
        }
        return result;
    }

}
