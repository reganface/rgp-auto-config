# RGP Auto Config
This tool will allow staff of a multi-location climbing gym to update their RGP location to the local database. For example, someone with a laptop has their RGP client configured to connect to Location A. They then travel to Location B, but if they open Rock Gym Pro, it will still connect to Location A. Running this program will automatically update the client configuration to connect to Location B - the local database. The next time RGP is opened, it will connect to Location B!

*This is for locally hosted RGP installations only!  There is no need for a tool like this if you are using RGP Cloud.*

## Download
Head over to the [releases tab](https://github.com/reganface/rgp-auto-config/releases/) to download this program.

## How it Works
This program will piggy back off of your existing RGP configuration to connect to your database, get all possible remote database connections, and automatically choose the correct connection based off of the client's IP address.  It assumes that the local database will exist within the same subnet as the client.

## Logo
You can change the logo that is displayed within the program by including a `logo.png` file within the same directory as `RGP Auto Config.exe`.  I forget the exact maximum dimensions, but it's somewhere around 300x100.

## How to Use
- RGP should not be open on the the workstation you are trying to update.  If you get an error saying RGP is running, it's possible there is a background task running.  Wait for it to end (or force quit if you are really impatient) and then try again.  If there are multiple Windows users logged in, it should be closed there, as well.
- Run `RGP Auto Config.exe` and enter your staff PIN.  All valid PINs will work regardless of RGP access level.
- If you have multiple servers set up on the same subnet, the auto config will not know which server to connect to.  In this case, you will get to select from all locations on the current subnet.
- A success message will pop up.  Hit OK to close the program.
- At this point you can now open RGP, and it will be connected to your local database.

# Build From Source
C# is not what I usually use.  In fact, this was my first and only time using C# and Visual Studio sometime back in 2016.  As a result, I *think* I've included everything needed, but I'm not 100% sure.  You should be able to open the .sln file in Visual Studio and compile it, but let me know if that's not working.  I may be able to look into it further.  Also, don't mind the mess!

All of the logic for the program is in `Form1.cs`.  If you want to replace the logo with your own, overwrite the `Resources/mesarim_logo.png` file with your logo, then compile.
