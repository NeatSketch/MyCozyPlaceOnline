using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkClient : MonoBehaviour
{
    public UnityEngine.UI.Text testText;

    /*private class Request<T>
    {
        [SerializeField]
        private string action;

        [SerializeField]
        private T payload;

        public T Payload
        {
            set
            {
                payload = value;
                Debug.Log(value.GetType().Name);
                switch (value.GetType().Name)
                {
                    case "LoginRequestData":
                        action = "login";
                        break;
                    case "UpdateRequestData":
                        action = "update";
                        break;
                }
            }
        }
    }*/

    private class Request
    {
        public string action;
    }

    private class LoginRequestData : Request
    {
        public string username;
    }

    private class UpdateRequestData : Request
    {
        public string username;
        public string authToken;
        public float positionX;
        public float positionZ;
        public float velocityX;
        public float velocityZ;
    }

    private class ResponseData
    {
        public string status;
        public int errorCode;
        public string errorText;
    }

    private class LoginResponseData : ResponseData
    {
        public LoginResponsePayload payload;
    }

    private class UpdateResponseData : ResponseData
    {
        public UpdateResponsePayload payload;
    }

    [System.Serializable]
    private class LoginResponsePayload
    {
        public string authToken;
    }

    [System.Serializable]
    private class UpdateResponsePayload
    {
        public List<Layer> layers;
    }

    [System.Serializable]
    private class Layer
    {
        public List<Entity> entities;
    }

    [System.Serializable]
    private class Entity
    {
        public int type;
        public string name;
        public float posX;
        public float posZ;
        public float velX;
        public float velZ;
    }

    private string authToken;

    void Start()
    {
        StartCoroutine(NetworkSync());
    }

    void Update()
    {
        
    }

    IEnumerator NetworkSync()
    {
        UnityWebRequest loginRequest = SendRequest
        (
            new LoginRequestData
            {
                action = "login",
                username = "Neat"
            }
        );

        while (!loginRequest.isDone)
        {
            yield return null;
        }

        if (!IsSuccess(loginRequest))
        {
            Debug.LogError(loginRequest.error);

            testText.text = "Error";
        }
        else
        {
            string response = loginRequest.downloadHandler.text;

            LoginResponseData loginResponseData = JsonUtility.FromJson<LoginResponseData>(response);

            authToken = loginResponseData.payload.authToken;

            Debug.Log("response = " + response);
            Debug.Log("Error text = " + loginResponseData.errorText);
            Debug.Log("authToken = " + loginResponseData.payload.authToken);

            testText.text = response + "\n\nDone";
        }

        while (true)
        {

            yield return new WaitForSeconds(1f);

            UnityWebRequest unityWebRequest = SendRequest
            (
                new UpdateRequestData
                {
                    action = "update",
                    username = "Neat",
                    authToken = authToken
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

                UpdateResponseData updateResponseData = JsonUtility.FromJson<UpdateResponseData>(response);



                Debug.Log("response = " + response);
                Debug.Log("Error text = " + updateResponseData.errorText);

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

    private UnityWebRequest SendRequest(Request request)
    {
        string data = JsonUtility.ToJson
        (
            request
        );

        UnityWebRequest unityWebRequest = UnityWebRequest.Post("http://localhost/", "");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
        unityWebRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        unityWebRequest.uploadHandler.contentType = "application/json";
        //unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        unityWebRequest.SendWebRequest();

        return unityWebRequest;
    }

}
