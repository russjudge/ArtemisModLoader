=================================

DMX Commander

by Russ Judge

=================================


This tool is for building and testing defintions for DMX Device usage.

Originally designed for use with Artemis: The Spaceship Bridge Simulator, but can be used for other purposes, as well.

This tool requires the FTD2XX drivers be installed, available from http://www.ftdichip.com/Drivers/D2XX.htm, and your
DMX device MUST be compatible with the Open DMX USB box from EntTec (http://www.enttec.com/index.php?main_menu=Products&pn=70303&show=description).

An alternate and cheaper source for such a device can be found here: http://www.holidaycoro.com/RGB-Sample-Pack-3-p/35.htm.



-------------------------------
Change History
-------------------------------
version	Date		Description
0.1b	08/28/2013	Initial beta release
0.2b	08/30/2013	Added logging

					General bug fixes
					
					Added alternating colors to the various lists

					Added better error handling on connecting to the DMX device

					More correct handling of multiple identically named event types.

					Fixed bug that prevented entering negative values for "change".

					Added code to automatically open the configuration window if configuration has not been set.

0.3b	08/30/2013	Added code to allow for running without a DMX device.

0.4b	09/14/2013	Added scripting engine.

					Added grouping in channel selection drop-downs.

0.5b	09/18/2013	Bug #4: Reported NullReferenceException when selecting event types in the event drop-down should be fixed.

0.6b	11/30/2013	Bug #6: Modified Channel number in Configuration to match how it is configured with the DMX Tool.  Channel
					label that is displayed will use channel number that Artemis uses, which is one less than defined by DMX tool.

0.7b	09/21/2014	Bug #7: A fatal error was reported when the list of channels were being refreshed.  Unable to reproduce,
					but added code to prevent the error.  Exception received: System.ArgumentNullException.

