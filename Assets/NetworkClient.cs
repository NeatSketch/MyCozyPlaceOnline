using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkClient : MonoBehaviour
{
    //public UnityEngine.UI.Text testText;

    public string hostname;
    public string username;

    public string accessoryHead;
    public string accessoryNeck;
    public string accessoryButt;

    public float normalRequestDelay = 1f;
    public float minRequestDelay = 0.25f;
    public float minLocalPlayerRotationDeltaForEarlySync = 30f;

    private float lastRequestTime;

    private bool ForceRequest
    {
        get
        {
            float timeSinceLastRequest = Time.timeSinceLevelLoad - lastRequestTime;
            if (timeSinceLastRequest > minRequestDelay && CharacterMovement.RotationDelta > minLocalPlayerRotationDeltaForEarlySync)
            {
                return true;
            }

            return false;
        }
    }

    public WorldMap worldMap;
    public Character localPlayerCharacter;

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
        public bool updProps;
        public string username;
        public string authToken;
        public float positionX;
        public float positionZ;
        public float velocityX;
        public float velocityZ;
        public string accHead;
        public string accNeck;
        public string accButt;
    }

    private class SetBlockRequestData : Request
    {
        public string username;
        public string authToken;
        public int positionX;
        public int positionZ;
        public int blockType;
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
        public float posX;
        public float posZ;
        public string accHead;
        public string accNeck;
        public string accButt;
    }

    [System.Serializable]
    private class Layer
    {
        public List<Chunk> chunks;
    }

    [System.Serializable]
    private class Chunk
    {
        public List<Entity> entities;
        public int x;
        public int z;
    }

    [System.Serializable]
    private class Entity
    {
        public string id;
        public int type;
        public string name;
        public float posX;
        public float posZ;
        public float velX;
        public float velZ;
        public int blockType;
        public string accHead;
        public string accNeck;
        public string accButt;
    }

    private string authToken;

    void Start()
    {
        StartCoroutine(NetworkSync());
    }

    void Update()
    {
        
    }

    private bool updProps = false;

    IEnumerator NetworkSync()
    {
        UnityWebRequest loginRequest = SendRequest
        (
            new LoginRequestData
            {
                action = "login",
                username = username
            }
        );

        while (!loginRequest.isDone)
        {
            yield return null;
        }

        if (!IsSuccess(loginRequest))
        {
            Debug.LogError(loginRequest.error);

            //testText.text = "Error";
        }
        else
        {
            string response = loginRequest.downloadHandler.text;

            LoginResponseData loginResponseData = JsonUtility.FromJson<LoginResponseData>(response);

            authToken = loginResponseData.payload.authToken;

            localPlayerCharacter.Username = username;

            Debug.Log("response = " + response);
            Debug.Log("Error text = " + loginResponseData.errorText);
            Debug.Log("authToken = " + loginResponseData.payload.authToken);

            //testText.text = response + "\n\nDone";
        }

        while (true)
        {

            float _delay = 0;
            while(_delay < normalRequestDelay || ForceRequest)
            {
                _delay += Time.deltaTime;
                yield return null;
            }

            lastRequestTime = Time.timeSinceLevelLoad;

            UnityWebRequest unityWebRequest = SendRequest
            (
                new UpdateRequestData
                {
                    action = "update",
                    username = username,
                    authToken = authToken,
                    positionX = localPlayerCharacter.transform.position.x,
                    positionZ = localPlayerCharacter.transform.position.z,
                    velocityX = localPlayerCharacter.CurrentVelocity.x,
                    velocityZ = localPlayerCharacter.CurrentVelocity.z,
                    accHead = accessoryHead,
                    accNeck = accessoryNeck,
                    accButt = accessoryButt,
                    updProps = updProps
                }
            );

            while (!unityWebRequest.isDone)
            {
                yield return null;
            }

            if (!IsSuccess(unityWebRequest))
            {
                Debug.LogError(unityWebRequest.error);

                //testText.text = "Error";
            }
            else
            {
                string response = unityWebRequest.downloadHandler.text;

                UpdateResponseData updateResponseData = JsonUtility.FromJson<UpdateResponseData>(response);

                if (!updProps)
                {
                    float posX = updateResponseData.payload.posX;
                    float posZ = updateResponseData.payload.posZ;
                    localPlayerCharacter.transform.position = new Vector3(posX, 0f, posZ);

                    updProps = true;
                }

                foreach (Layer layer in updateResponseData.payload.layers)
                {
                    WorldChunkModel[,] worldChunks = new WorldChunkModel[3, 3];
                    for (int i = 0; i < 9; i++)
                    {
                        WorldChunkModel worldChunk = new WorldChunkModel();

                        worldChunk.x = layer.chunks[i].x;
                        worldChunk.z = layer.chunks[i].z;

                        worldChunk.entityModels = new List<EntityModel>();
                        foreach (Entity entity in layer.chunks[i].entities)
                        {
                            EntityModel entityModel = null;

                            switch (entity.type)
                            {
                                case 0:
                                    EntityModel_Player playerEntity = new EntityModel_Player();
                                    playerEntity.nickname = entity.name;
                                    playerEntity.velocity = new Vector2(entity.velX, entity.velZ);
                                    entityModel = playerEntity;
                                    break;
                                case 1:
                                    EntityModel_Block blockEntity = new EntityModel_Block();
                                    blockEntity.blockType = (entity.blockType > 0 && entity.blockType < 4) ? (WorldMap.BlockType)entity.blockType : 0;
                                    entityModel = blockEntity;
                                    break;
                            }

                            entityModel.id = entity.id;
                            entityModel.position = new Vector2(entity.posX, entity.posZ);

                            worldChunk.entityModels.Add(entityModel);
                        }

                        worldChunks[i % 3, i / 3] = worldChunk;
                    }
                    worldMap.SetLayer(0, worldChunks);
                }

                Debug.Log("response = " + response);
                Debug.Log("Error text = " + updateResponseData.errorText);

                //testText.text = response + "\n\nDone";
            }

        }
    }

    public void SetBlock(int posX, int posZ, int blockType)
    {
        UnityWebRequest setBlockRequest = SendRequest
        (
            new SetBlockRequestData
            {
                action = "setBlock",
                username = username,
                positionX = posX,
                positionZ = posZ
            }
        );
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

        UnityWebRequest unityWebRequest = UnityWebRequest.Post(string.Format("http://{0}/", hostname), "");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
        unityWebRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        unityWebRequest.uploadHandler.contentType = "application/json";
        //unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        unityWebRequest.SendWebRequest();

        return unityWebRequest;
    }

    public GameObject testForm;
    public UnityEngine.UI.InputField usernameText;
    public UnityEngine.UI.InputField hostnameText;

    public void TestGame()
    {
        username = usernameText.text;
        hostname = hostnameText.text;
        enabled = true;
        testForm.SetActive(false);
    }

}
