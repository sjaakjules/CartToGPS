# CartToGPS
Console app to translate a list of XYZ points into GPS given a (lat,Lon) location and heading for X-Axis

Help to the Git, going to change significantly.


Main directory is c:/Waypoints,
If this does not exist the program creates one automatically.

Files expected to be in the directory:
- BaseLocation.txt
	- Has the latitude ,longitude and heading of the base station.
	- If not in the directory program asks for information.
- xyzPos.csv
	- Has the X,Y,Z position relative to the BaseLocation.
	-	If not in the directory then program is useless as no points are needing to be changed.

Will assume the X-Axis points in the heading defined by the baseLocation. ATM its left hand rule, but will fix this to right hand rule with Z away from ground, X in the direction of heading and Y to the left when facing in the direction of heading.

Files created from my program and can amend existing files (when prompted type n)
- GPSPos.csv
	- GPS locations and relative height (z axis value)
- WayPoints.txt
	- waypoint commands ready to load in MissionPlanner or APM 2.0

