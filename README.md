# This is a simple c# api about mycobot

You can download the `.dll` at [Release](https://github.com/elephantrobotics/Mycobot.csharp/releases)

And import it to you Project.

```c#
/// arm power on
public void PowerOn()

/// arm power off
public void PowerOff()

/// Send one angle value
public void SendOneAngle(int jointNo, int angle, int speed)

/// Send all angles
public void SendAngles(int[] angles, int speed)

/// Get all angles
public int[] GetAngles()

/// Send one coord
public void SendOneCoord(int coord, int value, int speed)

/// Send all coords to arm
public void SendCoords(int[] coords, int speed, int mode)

/// Get all coord
public int[] GetCoords()
```
