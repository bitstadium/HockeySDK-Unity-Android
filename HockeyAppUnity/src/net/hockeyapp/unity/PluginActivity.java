package net.hockeyapp.unity;

import net.hockeyapp.android.Constants;
import net.hockeyapp.android.CrashManager;
import net.hockeyapp.android.UpdateManager;
import android.annotation.TargetApi;
import android.os.Build;
import android.os.Bundle;

import com.unity3d.player.UnityPlayerActivity;

public class PluginActivity extends UnityPlayerActivity {
  
  @Override
  public void onCreate(Bundle savedInstanceState) {
    
    super.onCreate(savedInstanceState);
  }
  
  @TargetApi(Build.VERSION_CODES.GINGERBREAD)
  public void startHockeyAppManager(final String appID){
    
    PluginActivity.this.runOnUiThread(new Runnable() 
    {
      @Override
      public void run() 
      {
            UpdateManager.register(PluginActivity.this, appID);
            CrashManager.register(PluginActivity.this, appID);
      }
    });
  }
  
  public String getAppVersion(){
    
    return Constants.APP_VERSION;
  }
  
}
