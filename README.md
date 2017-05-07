# DopyDaste
A Dapplo clipboard manager (DopyDaste)

Currently this is a placeholder, for an experimental Windows clipboard management application.

The "Clipboard" in Windows is actually a horrible technology.
Here are some blogs about this:
* https://blog.codinghorror.com/reinventing-the-clipboard/

I would like to try to offer the following functionality:
* Clipboard viewer
* Clipboard history, where one can search and select information
* Backup, restore
* Import, export
* Correct wrong formats, reformat etc
* Smart pasting: depending on the active application the clipboard can have different content. This should allow applications which currently don't integrate to work with each other.
* Filters / Tools
* Sharing with services
* Network synchronization
* Open with
* Survice reboots
* Addons
* API for other applications, to make it easier to have application interaction. e.g. Skype can tell use what formats it supports? And we can store "skype.exe" and formats XYZ

Some other clipboard tools already available, these might be looked at to see some ideas:
* ClipX is no longer developed: http://bluemars.org/clipx/
* Clipmate is no open source: http://www.clipmate.com/
* Others: http://www.makeuseof.com/tag/4-useful-clipboard-replacement-utilities-for-windows/


Implementation details:
Every clipboard change (WM_CLIPBOARDUPDATE) will be handled, so all the current information will be stored.
On the event, the following information should be stored:
* owner window handle
* Date & time
* Sequence
* The process to which the window belongs
* The title of the window name

Do we need to store all formats? What about size, and unknown formats?

Database storage should be simple and flexible.
LiteDB would be my current favorite to use...
