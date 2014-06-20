package net.hockeyapp.unity;

import net.hockeyapp.android.Constants;
import net.hockeyapp.android.CrashManager;
import net.hockeyapp.android.CrashManagerListener;
import net.hockeyapp.android.FeedbackManager;
import net.hockeyapp.android.UpdateManager;
import android.annotation.TargetApi;
import android.app.Activity;
import android.os.Build;

public class HockeyUnityPlugin {

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startHockeyAppManager(final String appID,
			final Activity currentActivity, final boolean updateManagerEnabled) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				if (updateManagerEnabled) {
					UpdateManager.register(currentActivity, appID);
				}
				CrashManager.register(currentActivity, appID);
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startHockeyAppManager(final String appID,
			final Activity currentActivity, final boolean updateManagerEnabled,
			final boolean autoSendEnabled) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				if (updateManagerEnabled) {
					UpdateManager.register(currentActivity, appID);
				}
				CrashManager.register(currentActivity, appID,
						new CrashManagerListener() {
							public boolean shouldAutoUploadCrashes() {
								return autoSendEnabled;
							}
						});
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startHockeyAppManager(final Activity currentActivity,
			final String serverURL, final String appID,
			final boolean updateManagerEnabled, final boolean autoSendEnabled) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				if (updateManagerEnabled) {
					UpdateManager.register(currentActivity, serverURL, appID, null);
				}
				CrashManager.register(currentActivity, serverURL, appID,
						new CrashManagerListener() {
							public boolean shouldAutoUploadCrashes() {
								return autoSendEnabled;
							}
						});
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startFeedbackForm(final String appID,
			final Activity currentActivity) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				FeedbackManager.register(currentActivity, appID);
				FeedbackManager.showFeedbackActivity(currentActivity);
			}
		});
	}

	@TargetApi(Build.VERSION_CODES.GINGERBREAD)
	public static void startFeedbackForm(final Activity currentActivity,
			final String serverURL, final String appID) {

		currentActivity.runOnUiThread(new Runnable() {
			@Override
			public void run() {
				FeedbackManager.register(currentActivity, serverURL, appID, null);
				FeedbackManager.showFeedbackActivity(currentActivity);
			}
		});
	}

	public static String getAppVersion() {

		return Constants.APP_VERSION;
	}

}
