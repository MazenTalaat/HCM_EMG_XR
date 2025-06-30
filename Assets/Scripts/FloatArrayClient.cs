/*
 * FloatArrayClient.cs
 * Attach this to any GameObject in your scene.
 *
 * • GetValues()  ? coroutine that calls /api/values   (GET)   and returns float[ ]
 * • SendValues() ? coroutine that calls /api/values/i (PUT)   for every element
 *
 * The Node server script from our previous steps must be running on the same
 * machine (or adjust serverBaseUrl to point to it on the LAN).
 */

using System;
using System.Collections;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FloatArrayClient : MonoBehaviour
{
    [Tooltip("Base URL of the Node server, e.g. http://localhost:8080")]
    [SerializeField] private string serverBaseUrl = "http://localhost:8080";

    // -----------------------------------------------------------------------
    // 1. READ the 6 floats  -------------------------------------------------
    // -----------------------------------------------------------------------
    public IEnumerator GetValues(Action<float[]> onSuccess, Action<string> onError = null)
    {
        using UnityWebRequest req = UnityWebRequest.Get($"{serverBaseUrl}/api/values");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(req.error);
            yield break;
        }

        float[] data = ParseFloatArray(req.downloadHandler.text);
        onSuccess?.Invoke(data);
    }

    // -----------------------------------------------------------------------
    // 2. SEND a new 6-float array  -----------------------------------------
    // -----------------------------------------------------------------------
    public IEnumerator SendValues(float[] values,
                                  Action onSuccess = null,
                                  Action<string> onError = null)
    {
        if (values == null || values.Length != 6)
        {
            onError?.Invoke("Array must contain exactly 6 floats.");
            yield break;
        }

        // The Node API updates one index at a time: PUT /api/values/:idx
        for (int i = 0; i < values.Length; i++)
        {
            string url = $"{serverBaseUrl}/api/values/{i}";
            string body = $"{{\"value\":{values[i].ToString(CultureInfo.InvariantCulture)}}}";
            byte[] raw = Encoding.UTF8.GetBytes(body);

            using UnityWebRequest req = new UnityWebRequest(url, "PUT");
            req.uploadHandler = new UploadHandlerRaw(raw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Index {i} failed ? {req.error}");
                yield break;
            }
        }

        onSuccess?.Invoke();
    }

    // -----------------------------------------------------------------------
    // Helper: quick ‘[x,x,…]’ ? float[ ] parser (no external JSON lib needed)
    // -----------------------------------------------------------------------
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
