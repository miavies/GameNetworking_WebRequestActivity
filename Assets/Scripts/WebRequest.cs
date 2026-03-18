using System.Collections;
using System.Text;
using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WebRequest : MonoBehaviour
{
    [Header("Register / Login UI")]
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TMP_InputField confirmPassword;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private GameObject forgotPasswordBtn;
    [SerializeField] private GameObject createNewBtn;
    [SerializeField] private GameObject signInBtn;
    [SerializeField] private GameObject loginBtn;
    [SerializeField] private GameObject changePasswordBtn;
    [SerializeField] private GameObject createAccountBtn;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private TextMeshProUGUI error;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI killsCount;
    [SerializeField] private TextMeshProUGUI deathsCount;

    [Header("Object Enable")]
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject[] enemyObj;
    [SerializeField] private GameObject cameraObj;
    [SerializeField] private GameObject UIpanel;
    [SerializeField] private GameObject deleteAccountBtn;

    public string playerId;
    [SerializeField] private DeletePanel deletePanel;


    public void StartRegister()
    {
        StartCoroutine(Register());
    }

    public void StartLogin()
    {
        StartCoroutine(Login());
    }

    public void StartNewPassword()
    {
        StartCoroutine(NewPassword());
    }

    public void StartUpdateKills()
    {
        StartCoroutine(UpdateScore(1,0));
    }

    public void StartUpdateDeaths()
    {
        StartCoroutine(UpdateScore(0, 1));
    }

    public void StartDeleteAccount()
    {
        StartCoroutine(DeleteAccount());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);
        form.AddField("email", email.text);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/players/register", form);

        yield return www.SendWebRequest();

        Response response = JsonUtility.FromJson<Response>(www.downloadHandler.text);
        if (!response.success)
        {
            message.text = "";
            message.gameObject.SetActive(true);
            message.text = response.message;

            error.text = "";
            error.gameObject.SetActive(true);
            string err = "";
            if (response.error != null && response.error.Length > 0)
            {
                err += "\n" + string.Join("\n", response.error);
            }
            error.text = err;

            yield break;
        }

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            message.gameObject.SetActive(false);
            error.gameObject.SetActive(true);
            error.text = "Server Error: " + www.error;
            yield break;
        }

        
        username.gameObject.SetActive(true);
        password.gameObject.SetActive(true);
        confirmPassword.gameObject.SetActive(false);
        email.gameObject.SetActive(false);
        forgotPasswordBtn.gameObject.SetActive(true);
        createNewBtn.SetActive(true);
        signInBtn.SetActive(false);
        loginBtn.SetActive(true);
        changePasswordBtn.SetActive(false);
        createAccountBtn.SetActive(false);
        message.gameObject.SetActive(false);
        error.gameObject.SetActive(false);

        Debug.Log(www.downloadHandler.text);
    }

    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/players/login", form);

        yield return www.SendWebRequest();

        Response response = JsonUtility.FromJson<Response>(www.downloadHandler.text);
        Debug.Log("Success Bool: " + response.success);
        if (!response.success)
        {
            message.text = "";
            message.gameObject.SetActive(true);
            message.text = response.message;

            error.text = "";
            error.gameObject.SetActive(true);
            string err = "";
            if (response.error != null && response.error.Length > 0)
            {
                err += "\n" + string.Join("\n", response.error);
            }
            error.text = err;
            yield break;
        }

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            message.gameObject.SetActive(false);
            error.gameObject.SetActive(true);
            error.text = "Server Error: " + www.error;
            yield break;
        }

        PlayerData playerData = JsonUtility.FromJson<PlayerData>(www.downloadHandler.text);
        playerId = response.data._id;

        message.gameObject.SetActive(false);
        error.gameObject.SetActive(false);
        playerObj.SetActive(true);

        foreach (var enemy in enemyObj)
        {
            enemy.gameObject.SetActive(true);
        }

        cameraObj.SetActive(false);
        UIpanel.SetActive(false);

        deletePanel.loggedIn = true;
        Debug.Log(www.downloadHandler.text);

        StartCoroutine(UpdateScore(0,0));
    }

    IEnumerator NewPassword()
    {
        PasswordUpdateRequest requestData = new PasswordUpdateRequest
        {
            username = username.text,             
            newPassword = password.text,          
            confirmPassword = confirmPassword.text 
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);

        UnityWebRequest www = UnityWebRequest.Put($"http://localhost:3000/players/new-password", jsonToSend);
        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        Response response = JsonUtility.FromJson<Response>(www.downloadHandler.text);
        if (!response.success)
        {
            message.text = "";
            message.gameObject.SetActive(true);
            message.text = response.message;

            error.text = "";
            error.gameObject.SetActive(true);
            string err = "";
            if (response.error != null && response.error.Length > 0)
            {
                err += "\n" + string.Join("\n", response.error);
            }
            error.text = err;

            yield break;
        }

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            message.gameObject.SetActive(false);
            error.gameObject.SetActive(true);
            error.text = "Server Error: " + www.error;
            yield break;
        }

        username.gameObject.SetActive(true);
        password.gameObject.SetActive(true);
        confirmPassword.gameObject.SetActive(false);
        email.gameObject.SetActive(false);
        forgotPasswordBtn.gameObject.SetActive(true);
        createNewBtn.SetActive(true);
        signInBtn.SetActive(false);
        loginBtn.SetActive(true);
        changePasswordBtn.SetActive(false);
        createAccountBtn.SetActive(false);
        message.gameObject.SetActive(false);
        error.gameObject.SetActive(false);

        Debug.Log(www.downloadHandler.text);
    }

    IEnumerator UpdateScore(int kills, int deaths)
    {
        PlayerData updateData = new PlayerData
        {
            kills = kills,
            deaths = deaths
        };
        string json = JsonUtility.ToJson(updateData);
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);

        UnityWebRequest www = UnityWebRequest.Put($"http://localhost:3000/players/{playerId}", jsonToSend);
        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            StartCoroutine(GetScore());
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator GetScore()
    {
        UnityWebRequest www = UnityWebRequest.Get($"http://localhost:3000/players/{playerId}");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Response response = JsonUtility.FromJson<Response>(www.downloadHandler.text);

            killsCount.text = response.data.kills.ToString();
            deathsCount.text = response.data.deaths.ToString();

            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator DeleteAccount()
    {
        UnityWebRequest www = UnityWebRequest.Delete($"http://localhost:3000/players/{playerId}");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string _id;
    //public string username;
    public int kills;
    public int deaths;
}

[System.Serializable]
public class Response
{
    public bool success;
    public string message;
    public string[] error;
    public PlayerData data;
}

[System.Serializable]
public class PasswordUpdateRequest
{
    public string username;
    public string newPassword;
    public string confirmPassword;
}