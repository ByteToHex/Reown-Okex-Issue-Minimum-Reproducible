package com.turbolabo.persistentunity;

import androidx.appcompat.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;
import android.content.pm.PackageManager; // Required for onRequestPermissionsResult

// Import your persistentnotification class
import com.turbolabo.persistentnotification.PersistentNotification;

public class MainActivity extends AppCompatActivity {

    private PersistentNotification persistentNotification;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main); // Set the layout for this activity

        // Initialize your PersistentNotification instance
        persistentNotification = new PersistentNotification();

        // Pass the current Activity context to the plugin
        // This is crucial for the plugin to function correctly.
        PersistentNotification.receiveUnityActivity(this);

        // Find buttons by their IDs
        Button showNotificationButton = findViewById(R.id.showNotificationButton);
        Button hideNotificationButton = findViewById(R.id.hideNotificationButton);

        // Set click listener for the "Show Notification" button
        showNotificationButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Call the showPersistentNotification method
                persistentNotification.showPersistentNotification(
                        "Boxx Game Running",
                        "Your game is active in the background."
                );
                Toast.makeText(MainActivity.this, "Attempting to show notification...", Toast.LENGTH_SHORT).show();
            }
        });

        // Set click listener for the "Hide Notification" button
        hideNotificationButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                // Call the hidePersistentNotification method
                persistentNotification.hidePersistentNotification();
                Toast.makeText(MainActivity.this, "Notification hidden.", Toast.LENGTH_SHORT).show();
            }
        });
    }

    /**
     * This method is crucial for handling the result of runtime permission requests,
     * including the POST_NOTIFICATIONS permission on Android 13+.
     * It forwards the result to your PersistentNotification plugin.
     */
    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        // Forward the permission result to the static method in your plugin
        PersistentNotification.onRequestPermissionsResult(requestCode, permissions, grantResults);

        // Optional: If permission was just granted, you might want to re-attempt showing the notification
        // This is a simple retry; in a real app, you might have more sophisticated logic.
        if (requestCode == 1001 && grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
            Toast.makeText(this, "Notification permission granted! Try showing notification again.", Toast.LENGTH_LONG).show();
        }
    }
}
