Dopy ideas / rambling

After copy event
-> Initiated when the Clipboard gets new content, the event should have the information on the application that provided these

Before Paste event
-> Keyboard hotkey, with the information of the application which will probably handle the paste.
This needs a change in Dapplo.Windows.Input, so the current focussed window is also passed
Consider this: https://stackoverflow.com/questions/3940346/foreground-vs-active-window

After Paste event -> Would this be possible? Key up?

Identify applications "uniquely" so we can create rules for them.
* Application name
* Process name
* location (s)?
* Icon?
* Description?
* URL?
* Version?

Services
* History
** Some information needs to be encrypted
** Some information should not be allowed to be stored
** Formats to include / exclude (name, regex)
** Expire?
* Delete from Clipboard, for instance after paste
* Converters
* Matchers
* Clipboard "slots" (temporary content, only accessible from Dopy / API? -> not on the clipboard or in the history)

Rules:
* We need a way to define & store rules
** Format Json?
** Via the UI
* A place to store rules online
** a process to update these via pull-requests
** way to check for updates and download them

An example rule could be:
For 1pass do not store in the history, and delete after paste
Definition could be something like this:
On("AfterCopy").Where(Clipboard.Owner== "1pass" - version2).Ignore();
On("AfterPaste").Where(Clipboard.Owner=="1pass").Delay(1).Action(() => Clipboard.clear());
On("AfterCopy").Where(Clipboard.Content.Contains("Exception"))....

