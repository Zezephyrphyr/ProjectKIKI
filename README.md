# ProjectKIKI
Google Calendar Based Meeting Assistant
Installation:
Install RecentFilesView v1.33 
Build the project from visual studio. Readers can download the latest version of the sourcecode from the product version branch of the project: https://github.com/Zezephyrphyr/ProjectKIKI.git
Find the folder where KIKI.exe is in.
Move RecentFilesView.exe to the folder where KIKI.exe is in. 
Run KIKI.exe
To reset the program to original state, delete Settings.xml, Recors.xml, Files.xml, Meetings.xml in the build folder. The account information may stay.
To reset account information, go to visual studio and clean the build, leave the RecentFilesView.eve the only item in the folder. Then rebuild the program.
Or one can start with the original zip file provided.

User Manual:
The software main page contains serveral functions:
1. View today’s events
	A list of today’s upcoming events is listed in “Today’s event” module. The list is synchronized with Google Calendar every 5 minutes. Adds/deletes an event on the google calendar, the list will be refreshed in the next synchronization. If the event has finished, the event will also be removed from the list in the next synchronization.
2.   View the previous meeting records
	A list of previously occured meeting will be shown with its time, title, attendees and a file view button. Meetings will be in a time order. When the user clicks the file view button, a new window will pop up. The window will show a list of files that is used during the meeting. If the user clicks one of them, a new window will pop up showing during what meetings the file was modified.  The user can also open the file by clicking the file name.
3.    View the recent files 
	Switches to the “Recent File” tab, a list of file and their related meetings are shown. One can click the file name to open the file directly. If one wants to track what files are used in the meeting list, just click on the meeting title. A file list will pop up.
4.    Search files and meetings
	On the upper-left corner there is a search button. The button will lead to a new window. One can choose to search files or search meetings with keywords. If there’s no result match, a message box will pop up to show no result. Otherwise, a similar meeting list or a file list as in the main window will be shown. One can take the same action as in the main window.
      5. 	Login/Log out
	If one uses the application for the first time, the application will ask  to login with google account. The data is saved and next time one doesn’t need to type in the account and password again. If one wants to switch to another account, just click the login button at the right corner of the window. The whole window will be emptied. If one clicks the button again, he will be asked to login again.
      6.	Minimization
	The application can be minimized to background. An icon will appear in the system tray. If one double clicks the icon, the window will pop up.
      7. 	Synchronization
      The data source will be fetched and refreshed every 5 minutes, also, the UI will be changed at the same time. No matter whether  opens the application during a meeting, next time when he opens the application, the meeting information and modified file information can always be tracked.  For example, the calendar shows that one has a meeting on 2017/2/5 14:00:00, but he didn’t open the application at that time. The application can automatically get the data and show which files are modified during this meeting when one opens the application at anytime.
