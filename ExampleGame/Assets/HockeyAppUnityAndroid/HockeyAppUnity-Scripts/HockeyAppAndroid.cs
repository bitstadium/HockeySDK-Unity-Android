/*******************************************************************************
 *
 * Author: Christoph Wendt
 * 
 * Version: 1.1.0-beta.1
 *
 * Copyright (c) HockeyApp, Bit Stadium GmbH.
 * All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 * 
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;

public class HockeyAppAndroid : MonoBehaviour {
	
	protected const string HOCKEYAPP_BASEURL = "https://rink.hockeyapp.net/";
	protected const string HOCKEYAPP_CRASHESPATH = "api/2/apps/[APPID]/crashes/upload";
	protected const int MAX_CHARS = 199800;
	protected const string LOG_FILE_DIR = "/logs/";

	[Header("HockeyApp Setup")]
	public string appID = "your-hockey-app-id";
	public string packageID = "your-package-identifier";
	public string serverURL = "your-custom-server-url";

	[Header("Crashes & Exceptions")]
	public bool autoUploadCrashes = false;
	public bool exceptionLogging = true;

	[Header("Metrics")]
	public bool userMetrics = true;

	[Header("Version Updates")]
	public bool updateAlert = true;

	void Awake(){

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		DontDestroyOnLoad(gameObject);
		if(exceptionLogging == true  && IsConnected() == true)
		{
			List<string> logFileDirs = GetLogFiles();
			if(logFileDirs.Count > 0)
			{
				StartCoroutine(SendLogs(logFileDirs));
			}
		}
		string urlString = GetBaseURL();
		StartCrashManager(urlString, appID, updateAlert, userMetrics, autoUploadCrashes);
		#endif
	}
	
	void OnEnable(){
		
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if(exceptionLogging == true)
		{
			System.AppDomain.CurrentDomain.UnhandledException += OnHandleUnresolvedException;
			Application.logMessageReceived += OnHandleLogCallback;
		}
		#endif
	}
	
	void OnDisable(){
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if(exceptionLogging == true)
		{
			System.AppDomain.CurrentDomain.UnhandledException -= OnHandleUnresolvedException;
			Application.logMessageReceived -= OnHandleLogCallback;
		}
		#endif
	}

	/// <summary>
	/// Start HockeyApp for Unity.
	/// </summary>
	/// <param name="appID">The app specific Identifier provided by HockeyApp</param>
	protected void StartCrashManager(string urlString, string appID, bool updateManagerEnabled, bool userMetricsEnabled, bool autoSendEnabled) {

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"); 
		AndroidJavaClass pluginClass = new AndroidJavaClass("net.hockeyapp.unity.HockeyUnityPlugin"); 
		pluginClass.CallStatic("startHockeyAppManager", currentActivity, urlString, appID, updateManagerEnabled, userMetricsEnabled, autoSendEnabled);
		#endif
	}

	/// <summary>
	/// Get the version code of the app.
	/// </summary>
	/// <returns>The version code of the Android app.</returns>
	protected String GetVersionCode(){

		string versionCode = null;

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		AndroidJavaClass jc = new AndroidJavaClass("net.hockeyapp.unity.HockeyUnityPlugin"); 
		versionCode =  jc.CallStatic<string>("getVersionCode");
		#endif

		return versionCode;
	}

	/// <summary>
	/// Get the version name of the app.
	/// </summary>
	/// <returns>The version name of the Android app.</returns>
	protected String GetVersionName(){
		
		string versionName = null;
		
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		AndroidJavaClass jc = new AndroidJavaClass("net.hockeyapp.unity.HockeyUnityPlugin"); 
		versionName =  jc.CallStatic<string>("getVersionName");
		#endif
		
		return versionName;
	}

	/// <summary>
	/// Get the SDK version.
	/// </summary>
	/// <returns>The SDK version.</returns>
	protected String GetSdkVersion(){
		
		string sdkVersion = null;
		
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		AndroidJavaClass jc = new AndroidJavaClass("net.hockeyapp.unity.HockeyUnityPlugin"); 
		sdkVersion =  jc.CallStatic<string>("getSdkVersion");
		#endif
		
		return sdkVersion;
	}

	/// <summary>
	/// Get the name of the SDK.
	/// </summary>
	/// <returns>The name of the SDK.</returns>
	protected String GetSdkName(){
		
		string sdkName = null;
		
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		AndroidJavaClass jc = new AndroidJavaClass("net.hockeyapp.unity.HockeyUnityPlugin"); 
		sdkName =  jc.CallStatic<string>("getSdkName");
		#endif
		
		return sdkName;
	}


	/// <summary>
	/// Collect all header fields for the custom exception report.
	/// </summary>
	/// <returns>A list which contains the header fields for a log file.</returns>
	protected virtual List<string> GetLogHeaders() {

		List<string> list = new List<string>();

		#if (UNITY_ANDROID && !UNITY_EDITOR)

		list.Add("Package: " + packageID);

		string versionCode = GetVersionCode();
		list.Add("Version Code: " + versionCode);

		string versionName = GetVersionName();
		list.Add("Version Name: " + versionName);

		string[] versionComponents = SystemInfo.operatingSystem.Split('/');
		string osVersion = "Android: " + versionComponents[0].Replace("Android OS ", "");
		list.Add (osVersion);
		
		list.Add("Model: " + SystemInfo.deviceModel);

		list.Add("Date: " + DateTime.UtcNow.ToString("ddd MMM dd HH:mm:ss {}zzzz yyyy").Replace("{}", "GMT"));
		#endif

		return list;
	}

	/// <summary>
	/// Create the form data for a single exception report.
	/// </summary>
	/// <param name="log">A string that contains information about the exception.</param>
	/// <returns>The form data for the current crash report.</returns>
	protected virtual WWWForm CreateForm(string log){

		WWWForm form = new WWWForm();

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		byte[] bytes = null;
		using(FileStream fs = File.OpenRead(log)){
			
			if (fs.Length > MAX_CHARS)
			{
				string resizedLog = null;
				
				using(StreamReader reader = new StreamReader(fs)){
					
					reader.BaseStream.Seek( fs.Length - MAX_CHARS, SeekOrigin.Begin );
					resizedLog = reader.ReadToEnd();
				}
				
				List<string> logHeaders = GetLogHeaders();
				string logHeader = "";
				
				foreach (string header in logHeaders)
				{
					logHeader += header + "\n";
				}
				
				resizedLog = logHeader + "\n" + "[...]" + resizedLog;
				
				try
				{
					bytes = System.Text.Encoding.Default.GetBytes(resizedLog);
				}
				catch(ArgumentException ae)
				{
					if (Debug.isDebugBuild) 
					{
						Debug.Log("Failed to read bytes of log file: " + ae);
					}
				}
			}
			else
			{
				try
				{
					bytes = File.ReadAllBytes(log);
				}
				catch(SystemException se)
				{
					if (Debug.isDebugBuild) 
					{
						Debug.Log("Failed to read bytes of log file: " + se);
					}
				}
			}
		}
		
		if(bytes != null)
		{
			form.AddBinaryData("log", bytes, log, "text/plain");
		}
		
		#endif
		
		return form;
	}

	/// <summary>
	/// Get a list of all existing exception reports.
	/// </summary>
	/// <returns>A list which contains the filenames of the log files.</returns>
	protected virtual List<string> GetLogFiles() {
		
		List<string> logs = new List<string>();
		
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		string logsDirectoryPath = Application.persistentDataPath + LOG_FILE_DIR;
		
		try
		{
			if (Directory.Exists(logsDirectoryPath) == false)
			{
				Directory.CreateDirectory(logsDirectoryPath);
			}
			
			DirectoryInfo info = new DirectoryInfo(logsDirectoryPath);
			FileInfo[] files = info.GetFiles();
			
			if (files.Length > 0)
			{
				foreach (FileInfo file in files)
				{
					if (file.Extension == ".log")
					{
						logs.Add(file.FullName);
					}
					else
					{
						File.Delete(file.FullName);
					}
				}
			}
		}
		catch(Exception e)
		{
			if (Debug.isDebugBuild) 
			{
				Debug.Log("Failed to write exception log to file: " + e);
			}
		}
		#endif
		
		return logs;
	}

	/// <summary>
	/// Upload existing reports to HockeyApp and delete delete them locally.
	/// </summary>
	protected virtual IEnumerator SendLogs(List<string> logs){

		string crashPath = HOCKEYAPP_CRASHESPATH;
		string url = GetBaseURL() + crashPath.Replace("[APPID]", appID);

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		string sdkName = GetSdkName ();
		if (sdkName != null) {
			url+= "?sdk=" + WWW.EscapeURL(sdkName);
		}
		#endif

		foreach (string log in logs)
		{		
			WWWForm postForm = CreateForm(log);
			string lContent = postForm.headers["Content-Type"].ToString();
			lContent = lContent.Replace("\"", "");
			Dictionary<string,string> headers = new Dictionary<string,string>();
			headers.Add("Content-Type", lContent);
			WWW www = new WWW(url, postForm.data, headers);
			yield return www;

			if (String.IsNullOrEmpty (www.error)) 
			{
				try 
				{
					File.Delete (log);
				} 
				catch (Exception e) 
				{
					if (Debug.isDebugBuild) Debug.Log ("Failed to delete exception log: " + e);
				}
			}
		}
	}

	/// <summary>
	/// Write a single exception report to disk.
	/// </summary>
	/// <param name="logString">A string that contains the reason for the exception.</param>
	/// <param name="stackTrace">The stacktrace for the exception.</param>
	protected virtual void WriteLogToDisk(string logString, string stackTrace){

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		string logSession = DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss_fff");
		string log = logString.Replace("\n", " ");
		string[]stacktraceLines = stackTrace.Split('\n');
		
		log = "\n" + log + "\n";
		foreach (string line in stacktraceLines)
		{
			if(line.Length > 0)
			{
				log +="  at " + line + "\n";
			}
		}
		
		List<string> logHeaders = GetLogHeaders();
		using (StreamWriter file = new StreamWriter(Application.persistentDataPath + LOG_FILE_DIR + "LogFile_" + logSession + ".log", true))
		{
			foreach (string header in logHeaders)
			{
				file.WriteLine(header);
			}
			file.WriteLine(log);
		}
		#endif
	}

	/// <summary>
	/// Get the base url used for custom exception reports.
	/// </summary>
	/// <returns>A formatted base url.</returns>
	protected virtual string GetBaseURL() {
		
		string baseURL ="";
		
		#if (UNITY_ANDROID && !UNITY_EDITOR)

		string urlString = serverURL.Trim();
		if(urlString.Length > 0)
		{
			baseURL = urlString;
			
			if(baseURL[baseURL.Length -1].Equals("/") != true){
				baseURL += "/";
			}
		}
		else
		{
			baseURL = HOCKEYAPP_BASEURL;
		}
		#endif
		
		return baseURL;
	}
	
	/// <summary>
	/// Checks whether internet is reachable
	/// </summary>
	protected virtual bool IsConnected()
	{
		
		bool connected = false;
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		
		if  (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || 
		     (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork))
		{
			connected = true;
		}
		
		#endif
		
		return connected;
	}

	/// <summary>
	/// Handle a single exception. By default the exception and its stacktrace gets written to disk.
	/// </summary>
	/// <param name="logString">A string that contains the reason for the exception.</param>
	/// <param name="stackTrace">The stacktrace for the exception.</param>
	protected virtual void HandleException(string logString, string stackTrace){
		
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		WriteLogToDisk(logString, stackTrace);
		#endif
	}

	/// <summary>
	/// Callback for handling log messages.
	/// </summary>
	/// <param name="logString">A string that contains the reason for the exception.</param>
	/// <param name="stackTrace">The stacktrace for the exception.</param>
	/// <param name="type">The type of the log message.</param>
	public void OnHandleLogCallback(string logString, string stackTrace, LogType type){

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if(LogType.Assert == type || LogType.Exception == type || LogType.Error == type)	
		{	
			HandleException(logString, stackTrace);
		}	
		#endif
	}
	
	public void OnHandleUnresolvedException(object sender, System.UnhandledExceptionEventArgs args){

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if(args == null || args.ExceptionObject == null)
		{	
			return;	
		}

		if(args.ExceptionObject.GetType() == typeof(System.Exception))
		{	
			System.Exception e	= (System.Exception)args.ExceptionObject;
			HandleException(e.Source, e.StackTrace);
		}

		#endif
	}
}
