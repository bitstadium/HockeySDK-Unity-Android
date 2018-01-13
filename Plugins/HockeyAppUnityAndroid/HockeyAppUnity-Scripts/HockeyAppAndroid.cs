/*
 * Version: 5.1.1
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;

public class HockeyAppAndroid : MonoBehaviour
{
	private const string JAVA_UNITYPLAYER_CLASS = "com.unity3d.player.UnityPlayer";
	private const string JAVA_HOCKEYUNITYPLUGIN_CLASS = "net.hockeyapp.unity.HockeyUnityPlugin";

	protected const string HOCKEYAPP_BASEURL = "https://rink.hockeyapp.net/";
	protected const string HOCKEYAPP_CRASHESPATH = "api/2/apps/[APPID]/crashes/upload";
	protected const int MAX_CHARS = 199800;
	protected const string LOG_FILE_DIR = "/logs/";
	private const string SERVER_URL_PLACEHOLDER = "your-custom-server-url";
	private static HockeyAppAndroid instance;

	public enum AuthenticatorType
	{
		Anonymous,
		HockeyAppEmail,
		HockeyAppUser,
		Validate
	}

	[Header("HockeyApp Setup")]
	public string appID = "your-hockey-app-id";
	public string packageID = "your-package-identifier";
	public string serverURL = SERVER_URL_PLACEHOLDER;

	[Header("Authentication")]
	public AuthenticatorType authenticatorType;
	public string secret = "your-hockey-app-secret";

	[Header("Crashes & Exceptions")]
	public bool autoUploadCrashes = false;
	public bool exceptionLogging = true;

	[Header("Metrics")]
	public bool userMetrics = true;

	[Header("Version Updates")]
	public bool updateAlert = true;

	void Awake ()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if (instance != null) {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		CreateLogDirectory();

		if(exceptionLogging == true  && IsConnected() == true) {
			List<string> logFileDirs = GetLogFiles();
			if(logFileDirs.Count > 0) {
				Debug.Log("Found files: " + logFileDirs.Count);
				StartCoroutine(SendLogs(logFileDirs));
			}
		}
		serverURL = GetBaseURL();
		int authType = (int)authenticatorType;
		StartCrashManager(serverURL, appID, secret, authType, updateAlert, userMetrics, autoUploadCrashes);
		#endif
	}

	void OnEnable ()
	{

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if(exceptionLogging == true) {
			System.AppDomain.CurrentDomain.UnhandledException += OnHandleUnresolvedException;
			Application.logMessageReceived += OnHandleLogCallback;
		}
		#endif
	}

	void OnDisable ()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if (exceptionLogging == true) {
			System.AppDomain.CurrentDomain.UnhandledException -= OnHandleUnresolvedException;
			Application.logMessageReceived -= OnHandleLogCallback;
		}
		#endif
	}

	void OnApplicationPause(bool pause)
	{
		if (!pause) {
			PerformAuthentication();
		}
	}

	/// <summary>
	/// Start HockeyApp for Unity.
	/// </summary>
	/// <param name="urlString">The url of the endpoint used for sending data.</param>
	/// <param name="appID">The app specific Identifier provided by HockeyApp.</param>
	/// <param name="secret">The app secret used for authenticating users.</param>
	/// <param name="authType">Auth type used for authentication: Anonymous, email, email& password, or check if user was explicitly added to use this app.</param>
	/// <param name="updateManagerEnabled">True, if user should be notified about newer versions of the app.</param>
	/// <param name="userMetricsEnabled">True, app should send user and session information.</param>
	/// <param name="autoSendEnabled">True, if crashes should be sent without asking the user for approval.</param>
	protected void StartCrashManager (string urlString, string appID, string secret, int authType, bool updateManagerEnabled, bool userMetricsEnabled, bool autoSendEnabled)
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		using (var unityPlayer = new AndroidJavaClass(JAVA_UNITYPLAYER_CLASS))
		using (var pluginClass = new AndroidJavaClass(JAVA_HOCKEYUNITYPLUGIN_CLASS))
		{
			var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			pluginClass.CallStatic("startHockeyAppManager", currentActivity, urlString, appID, secret, authType, updateManagerEnabled, userMetricsEnabled, autoSendEnabled);
		}
		instance = this;
		#endif
	}

	/// <summary>
	/// This method allows to track an event that happened in your app.
	/// Remember to choose meaningful event names to have the best experience when diagnosing your app
	/// in the web portal.
	/// </summary>
	/// <param name="eventName">The name of the event, which should be tracked.</param>
	public static void TrackEvent(string eventName)
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if (instance != null)
		{
			using (var pluginClass = new AndroidJavaClass(JAVA_HOCKEYUNITYPLUGIN_CLASS))
			{
				pluginClass.CallStatic("trackEvent", eventName);
			}
		}
		else
		{
			Debug.Log("Failed to track event. SDK has not been initialized, yet.");
		}
		#endif
	}

	/// <summary>
	/// This method allows to track an event that happened in your app.
	/// Remember to choose meaningful event names to have the best experience when diagnosing your app
	/// in the web portal.
	/// </summary>
	/// <param name="eventName">The name of the event, which should be tracked.</param>
	/// <param name="measurements">Key value pairs, which contain custom metrics.</param>
	public static void TrackEvent(string eventName, IDictionary<string, string> properties)
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if (instance != null)
		{
			using (var pluginClass = new AndroidJavaClass(JAVA_HOCKEYUNITYPLUGIN_CLASS))
			{
				pluginClass.CallStatic("trackEvent", eventName,
					DictainaryToJavaMap(properties, "java.lang.String", "java.lang.String"));
			}
		}
		else
		{
			Debug.Log("Failed to track event. SDK has not been initialized, yet.");
		}
		#endif
	}

	/// <summary>
	/// This method allows to track an event that happened in your app.
	/// Remember to choose meaningful event names to have the best experience when diagnosing your app
	/// in the web portal.
	/// </summary>
	/// <param name="eventName">The name of the event, which should be tracked.</param>
	/// <param name="properties">Key value pairs with additional info about the event.</param>
	/// <param name="measurements">Key value pairs, which contain custom metrics.</param>
	public static void TrackEvent(string eventName, IDictionary<string, string> properties, IDictionary<string, double> measurements)
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if (instance != null)
		{
			using (var pluginClass = new AndroidJavaClass(JAVA_HOCKEYUNITYPLUGIN_CLASS))
			{
				pluginClass.CallStatic("trackEvent", eventName,
					DictainaryToJavaMap(properties, "java.lang.String", "java.lang.String"),
					DictainaryToJavaMap(measurements, "java.lang.String", "java.lang.Double"));
			}
		}
		else
		{
			Debug.Log("Failed to track event. SDK has not been initialized, yet.");
		}
		#endif
	}

	/// <summary>
	/// Performs user authentication.
	/// </summary>
	public static void PerformAuthentication()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		using (var unityPlayer = new AndroidJavaClass(JAVA_UNITYPLAYER_CLASS))
		using (var pluginClass = new AndroidJavaClass(JAVA_HOCKEYUNITYPLUGIN_CLASS))
		{
			var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			pluginClass.CallStatic("performAuthentication", currentActivity);
		}
		#endif
	}

	/// <summary>
	/// Check for version update and present alert if newer version is available.
	/// </summary>
	public static void CheckForUpdate()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if (instance != null)
		{
			using (var unityPlayer = new AndroidJavaClass(JAVA_UNITYPLAYER_CLASS))
			using (var pluginClass = new AndroidJavaClass(JAVA_HOCKEYUNITYPLUGIN_CLASS))
			{
				var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				pluginClass.CallStatic("checkForUpdate", currentActivity, instance.serverURL, instance.appID);
			}
		}
		else
		{
			Debug.Log("Failed to check for update. SDK has not been initialized, yet.");
		}
		#endif
	}

	/// <summary>
	/// Display a feedback form.
	/// </summary>
	public static void ShowFeedbackForm()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if (instance != null)
		{
			using (var unityPlayer = new AndroidJavaClass(JAVA_UNITYPLAYER_CLASS))
			using (var pluginClass = new AndroidJavaClass(JAVA_HOCKEYUNITYPLUGIN_CLASS))
			{
				var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
				pluginClass.CallStatic("startFeedbackForm", currentActivity);
			}
		}
		else
		{
			Debug.Log("Failed to present feedback form. SDK has not been initialized, yet.");
		}
		#endif
	}

	/// <summary>
	/// Collect all header fields for the custom exception report.
	/// </summary>
	/// <returns>A list which contains the header fields for a log file.</returns>
	protected virtual List<string> GetLogHeaders ()
	{
		List<string> list = new List<string> ();

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		using (var pluginClass = new AndroidJavaClass(JAVA_HOCKEYUNITYPLUGIN_CLASS))
		{
			var versionCode = pluginClass.CallStatic<string>("getVersionCode");
			var versionName = pluginClass.CallStatic<string>("getVersionName");
			var manufacturer = pluginClass.CallStatic<string>("getManufacturer");
			var model = pluginClass.CallStatic<string>("getModel");
			var deviceIdentifier = pluginClass.CallStatic<string>("getDeviceIdentifier");

			list.Add("Package: " + packageID);
			list.Add("Version Code: " + versionCode);
			list.Add("Version Name: " + versionName);

			var versionComponents = SystemInfo.operatingSystem.Split('/');
			var osVersion = "Android: " + versionComponents[0].Replace("Android OS ", "");
			list.Add (osVersion);

			list.Add("Manufacturer: " + manufacturer);
			list.Add("Model: " + model);
			list.Add("CrashReporter Key: " + deviceIdentifier);
			list.Add("Date: " + DateTime.UtcNow.ToString("ddd MMM dd HH:mm:ss {}zzzz yyyy").Replace("{}", "GMT"));
		}
		#endif

		return list;
	}

	/// <summary>
	/// Create the form data for a single exception report.
	/// </summary>
	/// <param name="log">A string that contains information about the exception.</param>
	/// <returns>The form data for the current crash report.</returns>
	protected virtual WWWForm CreateForm (string log)
	{
		WWWForm form = new WWWForm ();

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		byte[] bytes = null;
		using(FileStream fs = File.OpenRead(log)){

			if (fs.Length > MAX_CHARS) {
				string resizedLog = null;

				using(StreamReader reader = new StreamReader(fs)) {

					reader.BaseStream.Seek( fs.Length - MAX_CHARS, SeekOrigin.Begin );
					resizedLog = reader.ReadToEnd();
				}

				List<string> logHeaders = GetLogHeaders();
				string logHeader = "";

				foreach (string header in logHeaders) {
					logHeader += header + "\n";
				}
				resizedLog = logHeader + "\n" + "[...]" + resizedLog;

				try {
					bytes = System.Text.Encoding.Default.GetBytes(resizedLog);
				} catch(ArgumentException ae) {
					if (Debug.isDebugBuild) {
						Debug.Log("Failed to read bytes of log file: " + ae);
					}
				}
			} else {
				try {
					bytes = File.ReadAllBytes(log);
				} catch(SystemException se) {
					if (Debug.isDebugBuild) {
						Debug.Log("Failed to read bytes of log file: " + se);
					}
				}
			}
		}

		if(bytes != null) {
			form.AddBinaryData("log", bytes, log, "text/plain");
		}

		#endif

		return form;
	}

	/// <summary>
	/// Create the log directory if needed.
	/// </summary>
	protected virtual void CreateLogDirectory ()
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		string logsDirectoryPath = Application.persistentDataPath + LOG_FILE_DIR;

		try {
			Directory.CreateDirectory (logsDirectoryPath);
		} catch (Exception e) {
			if (Debug.isDebugBuild) Debug.Log ("Failed to create log directory at " + logsDirectoryPath + ": " + e);
		}
		#endif
	}

	/// <summary>
	/// Get a list of all existing exception reports.
	/// </summary>
	/// <returns>A list which contains the filenames of the log files.</returns>
	protected virtual List<string> GetLogFiles ()
	{
		List<string> logs = new List<string> ();

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		string logsDirectoryPath = Application.persistentDataPath + LOG_FILE_DIR;

		try {
			DirectoryInfo info = new DirectoryInfo(logsDirectoryPath);
			FileInfo[] files = info.GetFiles();

			if (files.Length > 0) {
				foreach (FileInfo file in files) {
					if (file.Extension == ".log") {
						logs.Add(file.FullName);
					} else {
						File.Delete(file.FullName);
					}
				}
			}
		} catch(Exception e) {
			if (Debug.isDebugBuild) {
				Debug.Log("Failed to write exception log to file: " + e);
			}
		}
		#endif

		return logs;
	}

	/// <summary>
	/// Upload existing reports to HockeyApp and delete delete them locally.
	/// </summary>
	protected virtual IEnumerator SendLogs (List<string> logs)
	{
		string crashPath = HOCKEYAPP_CRASHESPATH;
		string url = GetBaseURL () + crashPath.Replace ("[APPID]", appID);

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		using (var pluginClass = new AndroidJavaClass(JAVA_HOCKEYUNITYPLUGIN_CLASS))
		{
			var sdkName = pluginClass.CallStatic<string>("getSdkName");
			if (sdkName != null) {
				url += "?sdk=" + WWW.EscapeURL(sdkName);
			}
		}
		#endif

		foreach (string log in logs) {
			WWWForm postForm = CreateForm (log);
			string lContent = postForm.headers ["Content-Type"].ToString ();
			lContent = lContent.Replace ("\"", "");
			Dictionary<string,string> headers = new Dictionary<string,string> ();
			headers.Add ("Content-Type", lContent);
			WWW www = new WWW (url, postForm.data, headers);
			yield return www;

			if (String.IsNullOrEmpty (www.error)) {
				try {
					File.Delete (log);
				} catch (Exception e) {
					if (Debug.isDebugBuild)
						Debug.Log ("Failed to delete exception log: " + e);
				}
			} else {
				if (Debug.isDebugBuild)
					Debug.Log ("Crash sending error: " + www.error);
			}
		}
	}

	/// <summary>
	/// Write a single exception report to disk.
	/// </summary>
	/// <param name="logString">A string that contains the reason for the exception.</param>
	/// <param name="stackTrace">The stacktrace for the exception.</param>
	protected virtual void WriteLogToDisk (string logString, string stackTrace)
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		string logSession = DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss_fff");
		string log = logString.Replace("\n", " ");
		string[]stacktraceLines = stackTrace.Split('\n');

		log = "\n" + log + "\n";
		foreach (string line in stacktraceLines) {
			if(line.Length > 0) {
				log +="  at " + line + "\n";
			}
		}

		List<string> logHeaders = GetLogHeaders();
		using (StreamWriter file = new StreamWriter(Application.persistentDataPath + LOG_FILE_DIR + "LogFile_" + logSession + ".log", true)) {
			foreach (string header in logHeaders) {
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
	protected virtual string GetBaseURL ()
	{
		string baseURL = "";

		#if (UNITY_ANDROID && !UNITY_EDITOR)

		string urlString = serverURL.Trim();
		if(urlString.Length > 0 && urlString != SERVER_URL_PLACEHOLDER) {
			baseURL = urlString;

			if(baseURL[baseURL.Length -1].Equals("/") != true) {
				baseURL += "/";
			}
		} else {
			baseURL = HOCKEYAPP_BASEURL;
		}
		#endif

		return baseURL;
	}

	/// <summary>
	/// Checks whether internet is reachable
	/// </summary>
	protected virtual bool IsConnected ()
	{
		bool connected = false;

		#if (UNITY_ANDROID && !UNITY_EDITOR)

		if  (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ||
		     (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)) {
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
	protected virtual void HandleException (string logString, string stackTrace)
	{

		#if (UNITY_ANDROID && !UNITY_EDITOR)
		try
		{
			WriteLogToDisk(logString, stackTrace);
		}
		catch (Exception e)
		{
			AndroidLog(e.ToString());
		}
		#endif
	}

	/// <summary>
	/// Callback for handling log messages.
	/// </summary>
	/// <param name="logString">A string that contains the reason for the exception.</param>
	/// <param name="stackTrace">The stacktrace for the exception.</param>
	/// <param name="type">The type of the log message.</param>
	public void OnHandleLogCallback (string logString, string stackTrace, LogType type)
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if(LogType.Assert == type || LogType.Exception == type || LogType.Error == type) {
			HandleException(logString, stackTrace);
		}
		#endif
	}

	/// <summary>
	/// Callback for handling unresolved exceptions.
	/// </summary>
	public void OnHandleUnresolvedException (object sender, System.UnhandledExceptionEventArgs args)
	{
		#if (UNITY_ANDROID && !UNITY_EDITOR)
		if(args == null || args.ExceptionObject == null) {
			return;
		}

		if(args.ExceptionObject.GetType() == typeof(System.Exception)) {
			System.Exception e	= (System.Exception)args.ExceptionObject;
			HandleException(e.Source, e.StackTrace);
		}
		#endif
	}

	#region Android Binding Helpers
	#if (UNITY_ANDROID && !UNITY_EDITOR)

	private static AndroidJavaObject DictainaryToJavaMap<TKey, TValue>(IDictionary<TKey, TValue> parameters, string javaKeyClass, string javaValueClass)
	{
		if (parameters == null)
		{
			return null;
		}
		var javaMap = new AndroidJavaObject("java.util.HashMap");
		var putMethod = AndroidJNIHelper.GetMethodID(javaMap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
		foreach (var kvp in parameters)
		{
			AndroidJNI.CallObjectMethod(javaMap.GetRawObject(), putMethod, AndroidJNIHelper.CreateJNIArgArray(new object[]
				{
					new AndroidJavaObject(javaKeyClass, kvp.Key),
					new AndroidJavaObject(javaValueClass, kvp.Value)
				}));
		}
		return javaMap;
	}

	private static void AndroidLog(string message)
	{
		var logClass = new AndroidJavaObject("android.util.Log");
		logClass.CallStatic<int>("d", "HockeyApp", message);
	}

	#endif
	#endregion
}
