using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public InputField usernameInputField;
    public Text errorText;
    public Button loginButton;
    public NetworkClient networkClient;

    public InputField hostnameInputField;

    private void Awake()
    {
        networkClient.errorHandler += OnNetworkError;
        networkClient.loginHandler += OnLoginSuccess;
    }

    public void ClearErrorText()
    {
        errorText.text = "";
    }

    public void Login()
    {
        string username = usernameInputField.text;
        if (username.Length < 3)
        {
            errorText.text = "Too short!";
        }
        else
        {
            errorText.text = "Logging in...";
            usernameInputField.interactable = false;
            loginButton.interactable = false;

            networkClient.username = username;
            networkClient.hostname = hostnameInputField.text;
            networkClient.StartClient();
        }
    }

    public void OnNetworkError()
    {
        this.gameObject.SetActive(true);
        errorText.text = "Network error!";
        usernameInputField.interactable = true;
        loginButton.interactable = true;
    }

    public void OnLoginSuccess()
    {
        Debug.Log("login success");
        this.gameObject.SetActive(false);
        errorText.text = "";
        usernameInputField.interactable = true;
        loginButton.interactable = true;
    }
}
