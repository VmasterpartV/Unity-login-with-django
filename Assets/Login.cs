using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class Login : MonoBehaviour
{
    [SerializeField] public InputField username = null;
    [SerializeField] public InputField pass = null;

    private readonly string Baseurl = "http://127.0.0.1:8000/api/";
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public class LoginTokens
    {
        public string access;
        public string refresh;
    }

    public void submit()
    {
        StartCoroutine(login());
    }

    private IEnumerator login()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", pass.text);

        UnityWebRequest w = UnityWebRequest.Post(Baseurl + "auth/login/", form);
        yield return w.SendWebRequest();

        if (w.isNetworkError || w.isHttpError)
        {
            Debug.Log(w.error);
        }
        else
        {
            var res = w.downloadHandler.text;

            LoginTokens tokens = new LoginTokens();
            tokens = JsonUtility.FromJson<LoginTokens>(res);
            Debug.Log(tokens.access);

            var parts = tokens.access.Split('.');
            JToken userID = null;
            if (parts.Length > 2)
            {
                var decode = parts[1];
                var padLength = 4 - decode.Length % 4;
                if (padLength < 4)
                {
                    decode += new string('=', padLength);
                }
                var bytes = System.Convert.FromBase64String(decode);
                var accessData = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
                JObject json = JObject.Parse(accessData);
                userID = json["id"];
                Debug.Log(userID.ToString());
            }

            /* GET USER */
            string getUserUrl = Baseurl + "players/" + userID.ToString() + "/";
            UnityWebRequest www = UnityWebRequest.Get(getUserUrl);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
}
