using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkClient : MonoBehaviour
{
    public UnityEngine.UI.Text testText;

    private class RequestData {}

    private class LoginRequestData : RequestData
    {
        public string username;
    }

    void Start()
    {
        StartCoroutine(NetworkSync());
    }

    void Update()
    {
        
    }

    IEnumerator NetworkSync()
    {
        while (true)
        {

            yield return new WaitForSeconds(1f);

            UnityWebRequest unityWebRequest = SendRequest
            (
                new LoginRequestData
                {
                    username = "Neat"
                }
            );

            while (!unityWebRequest.isDone)
            {
                yield return null;
            }

            if (!IsSuccess(unityWebRequest))
            {
                Debug.LogError(unityWebRequest.error);

                testText.text = "Error";
            }
            else
            {
                string response = unityWebRequest.downloadHandler.text;

                testText.text = response + "\n\nDone";
            }

        }
    }

    public static bool IsSuccess(UnityWebRequest request)
    {
        if (request.isNetworkError) { return false; }

        if (request.responseCode == 0) { return true; }
        if (request.responseCode == (long)System.Net.HttpStatusCode.OK) { return true; }

        return false;
    }

    private UnityWebRequest SendRequest(RequestData request)
    {
        string data = JsonUtility.ToJson(request);

        UnityWebRequest unityWebRequest = UnityWebRequest.Post("http://localhost/", "");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
        unityWebRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        unityWebRequest.uploadHandler.contentType = "application/json";
        //unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        unityWebRequest.SendWebRequest();

        return unityWebRequest;
    }

}
