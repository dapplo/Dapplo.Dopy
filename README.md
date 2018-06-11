# Dopy
Dopy is a windows clipboard manager, it stands for Dapplo-Copy -> Dopy

Current state is that it's only experimental, Dopy can monitor every clipboard change (WM_CLIPBOARDUPDATE) and store the folling information in a LiteDB:
* current user session
* Windows Sequence ID (unique id, in the current user session)
* Date & time
* The process to which the window belongs
* The title of the window name
* Icon of the window / applications
* owner window handle
* the type of formats which were available
* the content of some formats


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
* Sharing with services (call TinyPNG on files)
* Network synchronization (exchange contents with other running Dopy instances)
* Open with (send contents to applications which are not running, or don't work directly with the clipboard)
* Survive reboots
* Addons
* API for other applications, to make it easier to have application interaction. e.g. Skype can tell use what formats it supports? And we can store "skype.exe" and formats XYZ

Some other clipboard tools already available, these might be looked at to see some ideas:
* ClipX is no longer developed: http://bluemars.org/clipx/
* Clipmate is no open source: http://www.clipmate.com/
* Others: http://www.makeuseof.com/tag/4-useful-clipboard-replacement-utilities-for-windows/


Questions:
* Do we need to store all formats? What about size, and unknown formats? Which formats for which application?
* Take CF_UNICODETEXT in favor of CF_TEXT
* Take PNG in favor of CF_TIFF, which is in favor of CF_DIBV5 -> CF_DIB -> CF_BITMAP
* HDrop -> See http://www.pinvoke.net/default.aspx/shell32.dragqueryfile
* What to do with URI`s?
* What with unknown formats? Popup a question if this should be stored?
* What with CF_SYLK and other shell formats?
