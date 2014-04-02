package net.hockeyapp.unity;

import net.hockeyapp.android.Constants;
import net.hockeyapp.android.CrashManager;
import net.hockeyapp.android.FeedbackManager;
import net.hockeyapp.android.UpdateManager;
import android.annotation.TargetApi;
import android.app.Activity;
import android.os.Build;

public class HockeyUnityPlugin {

  @TargetApi(Build.VERSION_CODES.GINGERBREAD)
  public static void startHockeyAppManager(final String appID, final Activity currentActivity) {

    currentActivity.runOnUiThread(new Runnable()
    {
      @Override
      public void run()
      {
        UpdateManager.register(currentActivity, appID);
        CrashManager.register(currentActivity, appID);
      }
    });
  }
  
  @TargetApi(Build.VERSION_CODES.GINGERBREAD)
  public static void startFeedbackForm(final String appID, final Activity currentActivity) {

    currentActivity.runOnUiThread(new Runnable()
    {
      @Override
      public void run()
      {
        FeedbackManager.register(currentActivity, appID, null);
        FeedbackManager.showFeedbackActivity(currentActivity);
      }
    });
  }
  
  public static String getAppVersion() {

    return Constants.APP_VERSION;
  }

}
