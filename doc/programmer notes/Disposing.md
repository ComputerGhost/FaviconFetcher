# Guidelines for Disposing

 Type				| Needs Disposed?	| Notes
--------------------|-------------------|-------
 HttpWebRequest		| No				| Hangs around for 2 minutes.
 HttpWebResponse	| Yes				| Disposes the response stream.
 Image				| Yes				|
 MemoryStream		| No				|
 StreamReader		| Yes				| Closes underlying stream.
 
#### Weird Cases:

 - HttpWebResponse.GetResponseStream()
   - The stream is disposed when HttpWebResponse is disposed.

 

