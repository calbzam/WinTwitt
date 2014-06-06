using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Diagnostics;
using Windows.Security.Cryptography.Core;
using WinTwitt.Utility;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using WinTwitt;

public class OAuthClient
{
    public enum RequestType
    {
        AccessToken,
        InitialToken
    }


    public static void PerformRequest(Dictionary<string, string> parameters, string url, string consumerSecret, string token, RequestType type)
    {
        string OAuthHeader = OAuthClient.GetOAuthHeader(parameters, "POST", url, consumerSecret, token);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "POST";
        request.Headers["Authorization"] = OAuthHeader;

        request.BeginGetResponse(new AsyncCallback(GetResponse), new object[] { request, type });
    }

    static void GetResponse(IAsyncResult result)
    {
        HttpWebRequest request = (HttpWebRequest)(((object[])result.AsyncState)[0]);
        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);

        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            RequestType currentType = (RequestType)(((object[])result.AsyncState)[1]);
            string completeString = reader.ReadToEnd();

            switch (currentType)
            {
                case RequestType.InitialToken:
                    {

                        string[] data = completeString.Split(new char[] { '&' });
                        int index = data[0].IndexOf("=");
                        App.Token = data[0].Substring(index + 1, data[0].Length - index - 1);
                        index = data[1].IndexOf("=");
                        App.TokenSecret = data[1].Substring(index + 1, data[1].Length - index - 1);


                        DefaultLaunch(App.Token); 
                        break;
                    }

                case RequestType.AccessToken:
                    {
                        string[] data = completeString.Split(new char[] { '&' });
                        int index = data[0].IndexOf("=");
                        App.Token = data[0].Substring(index + 1, data[0].Length - index - 1);
                        index = data[1].IndexOf("=");
                        App.TokenSecret = data[1].Substring(index + 1, data[1].Length - index - 1);
                        index = data[2].IndexOf("=");
                        App.UserID = data[2].Substring(index + 1, data[2].Length - index - 1);
                        index = data[3].IndexOf("=");
                        App.UserName = data[3].Substring(index + 1, data[3].Length - index - 1);
                        break;
                    }
            }
        }
    }

    public static void GetToken(IAsyncResult result)
    {

        HttpWebRequest request = (HttpWebRequest)result.AsyncState;
        HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result);

        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            string[] data = reader.ReadToEnd().Split(new char[] { '&' });
            int index = data[0].IndexOf("=");
            string token = data[0].Substring(index + 1, data[0].Length - index - 1);
            Debug.WriteLine("TOKEN OBTAINED");

            DefaultLaunch(token);

        }
    }

    // Launch the URI
    async static void DefaultLaunch(string token)
    {
        // The URI to launch
        string uriToLaunch = "http://api.twitter.com/oauth/authorize?oauth_token=" + token;

        // Create a Uri object from a URI string 
        var uri = new Uri(uriToLaunch);

        // Launch the URI
        var success = await Windows.System.Launcher.LaunchUriAsync(uri);

        if (success)
        {
            // URI launched
        }
        else
        {
            // URI launch failed
        }
    }

    public static string GetOAuthHeader(Dictionary<string, string> parameters, string httpMethod, string url, string consumerSecret, string tokenSecret)
    {
        parameters = parameters.OrderBy(x => x.Key).ToDictionary(v => v.Key, v => v.Value);

        string concat = string.Empty;

        string OAuthHeader = "OAuth ";
        foreach (string k in parameters.Keys)
        {
            concat += k + "=" + parameters[k] + "&";
            OAuthHeader += k + "=" + "\"" + parameters[k] + "\", ";
        }

        concat = concat.Remove(concat.Length - 1, 1);
        concat = StringHelper.EncodeToUpper(concat);

        concat = httpMethod + "&" + StringHelper.EncodeToUpper(url) + "&" + concat;



        HashAlgorithmProvider hashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
        IBuffer hash = hashProvider.HashData(CryptographicBuffer.ConvertStringToBinary(concat, BinaryStringEncoding.Utf8));
        string hashValue = CryptographicBuffer.EncodeToBase64String(hash);

    
        hashValue = hashValue.Replace("-", "");

        OAuthHeader += "oauth_signature=\"" + StringHelper.EncodeToUpper(hashValue) + "\"";

        return OAuthHeader;


    }
}