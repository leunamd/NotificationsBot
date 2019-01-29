# NotificationsBot
DarkOrbit Notify Bot  
This is not a darkorbit bot.

This is a script to check the activity of your account (every 5 minutes) to make sure the bot you are using hasn't crashed!

Features:  
-Notifies you via the PushBullet API for inactivity  
-Collects daily login rewards  
-Autobuilds precision targeter tech  
-Repairs drones over X damage (you can change it in the .txt file)  
-Relogins in case you lost your DO session with the same SID specified at start   
-Supports multiple acconts at once (1 account per line)  
-Change sid of current accounts by typing sid in the console  

Settings.txt example:  
apikey(pushbullet);server;sid;username;password;damagePercentagetoRepairdrones(0-99);buildTech(true/false)


Framework needed:
.Net Core 2.2 Runtime  
Download at: https://dotnet.microsoft.com/download

Troubleshooting:  
If bot crashes at start you either did not install .Net core 2.2 runtime or your .txt file is incorrect.  Make sure there are no additional blank lines in the settings.txt file.
