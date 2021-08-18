using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    [SerializeField] public InputField username = null;
    [SerializeField] public InputField pass = null;

    private readonly string Baseurl = "http://127.0.0.1:8000/api/v1.0/";
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

    private IEnumerator login ()
    {
        Debug.Log(username.text);
        Debug.Log("iniciar sesión");
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

            /* GET USER */
            string getUserUrl = Baseurl + "players/" + username.text + "/";
            UnityWebRequest www = UnityWebRequest.Get(getUserUrl);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            } else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
}
