using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AuthApi: ApiBase
{

	public delegate void LoginFailed(string errorMsg);
	public delegate void LoggedIn();
	public LoggedIn OnLoggedIn;
	public LoginFailed OnLoginFailed;

	public delegate void SignupFailed(string errorMsg);
	public delegate void SignupSuccess();
	public SignupSuccess OnSignupSuccess;
	public SignupFailed OnSignupFailed;

	// the authentication token will be set when a user has logged in.
	private string authenticationToken = "";

	/// <summary>
	/// Does a POST request to the backend, trying to get an authentication token. On succes, it will save the auth token for further use. On success, the OnLoggedIn
	/// delegate will be called. On fail, the OnLoginFailed delegate will be called.
	/// </summary>
	/// <param name="uuid"></param>
	/// <param name="password"></param>
	public void Login(string username, string password)
	{
		WWWForm form = new WWWForm();
		form.AddField("uuid", username);
		form.AddField("password", password);
		backendManager.Send(RequestType.Post, "login/", form, OnLoginResponse);
	}

	private void OnLoginResponse(ResponseType responseType, JToken responseData, string callee)
	{
		if (responseType == ResponseType.Success)
		{
			authenticationToken = responseData.Value<string>("token");
			if (OnLoggedIn != null)
			{
				OnLoggedIn();
			}
		}
		else if (responseType == ResponseType.ClientError)
		{
			if (OnLoginFailed != null)
			{
				OnLoginFailed("Could not reach the server. Please try again later.");
			}
		}
		else
		{
			JToken fieldToken = responseData["non_field_errors"];
			if (fieldToken == null || !fieldToken.HasValues)
			{
				if (OnLoginFailed != null)
				{
					OnLoginFailed("Login failed: unknown error.");
				}
			}
			else
			{
				string errors = "";
				JToken[] fieldValidationErrors = fieldToken.Values().ToArray();
				foreach (JToken validationError in fieldValidationErrors)
				{
					errors += validationError.Value<string>();
				}
				if (OnLoginFailed != null)
				{
					OnLoginFailed("Login failed: " + errors);
				}
			}
		}
	}


	/// <summary>
	/// Does a POST request to the backend, trying to get an authentication token. On succes, it will save the auth token for further use. On success, the OnLoggedIn
	/// delegate will be called. On fail, the OnLoginFailed delegate will be called.
	/// </summary>
	/// <param name="uuid"></param>
	/// <param name="email"></param>
	/// <param name="password"></param>
	public void Signup(string username, string email, string password)
	{
		WWWForm form = new WWWForm();
		form.AddField("uuid", username);
		form.AddField("email", email);
		form.AddField("password", password);
		backendManager.Send(RequestType.Post, "signup/", form, OnSignupResponse);
	}

	private void OnSignupResponse(ResponseType responseType, JToken responseData, string callee)
	{
		if (responseType == ResponseType.Success)
		{
			if (OnSignupSuccess != null)
			{
				OnSignupSuccess();
			}
		}
		else if (responseType == ResponseType.ClientError)
		{
			if (OnSignupFailed != null)
			{
				OnSignupFailed("Could not reach the server. Please try again later.");
			}
		}
		else if (responseType == ResponseType.RequestError)
		{
			string errors = "";
			JObject obj = (JObject)responseData;
			foreach (KeyValuePair<string, JToken> pair in obj)
			{
				errors += "[" + pair.Key + "] ";
				foreach (string errStr in pair.Value)
				{
					errors += errStr;
				}
				errors += '\n';
			}
			if (OnSignupFailed != null)
			{
				OnSignupFailed(errors);
			}
		}
	}
}
