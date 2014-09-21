=======================

Artemis Mod Loader

by Russ Judge

=======================

This tool is for managing Mods to Artemis: The Spaceship Bridge Simulator.

All installations of Mods should be defined by XML files.  Use the file
"MOD_IntoTheBreach.aml" or "MOD_HelmUI.aml" for an example of how to define the
file.

This tool tracks the install of mods, so that they can be easily uninstalled.
Administrator privilege is NOT required (and is in fact discouraged).

You should start with a plain vanilla install of Artemis, then use the
ArtemisModLoader to install all Mods.  ArtemisModLoader does not detect the
presence of Mods already installed.

***********************************
NOTE TO MOD DEVELOPERS: See "Mod Development" section below!!!
***********************************

-----------------------------------
Installing Artemis Mod Loader
-----------------------------------
Run AML.msi and follow the directions.

The Artemis install path will be automatically detected. If it does not detect
the Artemis Install folder, you will be prompted for it.


------------------------------------
A Brief Explanation
------------------------------------
I was impressed with the Mods that were available for Artemis Spaceship Bridge
Simulator, but found the mechanism for installing them and switching between
them very cumbersome, and even dangerous (the possibility of blowing away the
base stock install was high, thus requiring a full re-install to start over).
You were required to run under Administrator priviledge to set up the mods,
which installed themselves using command line control.

From what I could gather, the install process was build by non-programmers.
Although I have to give them credit for thinking far enough to offer some
mechanism for simplifying the process, it really wasn't a good way to manage
more than one or two mods for the game.

I wanted something that could easily switch in and out mods that may even be
fully incompatible with each other.  I didn't want to mess with Windows Explorer
and copying files into position.  I simply wanted to do a few mouse clicks to
select my mod and go.  I also did not want to have to worry about running with
Administrator privilege.

So I came up with this Artemis Mod Loader to manage my mods.  It drives off a
series of XML files (I'm using ".aml" as their extension) that have a simple
configuration to instruct the Loader how to install and activate the various
mods. In the simplest form, a Mod designer only need to create a zip package
that has the same directory structure as the Aretmis game, but the xml file
can add information to allow for any structure.  I created the definition files
for all the Mods I could find, and am willing to create more for anyone that
wants them, since they are so simple.

------------------------------------
How it works
------------------------------------
This Loader works by copying the entire Artemis install to a folder that does
not require Administrator access, therefore ArtemisModLoader can run with
non-privileged access.  All mods get applied to the copy, and Artemis is run
from the copy.  The original install remains untouched.

For this reason, your hard drive needs to have sufficient extra available space
to manage two copies of Artemis, plus any Mods.

-------------------------------------
Stacking Mods
-------------------------------------
Mods can be combined, and you can define the order they are applied (this could
be important since one mod could overwrite the changes made by another mod).
However, please note that some Mods might be incompatible with each other (for
example, the Battlestar Galactica Mod might not be compatible with the Star
Trek Mods, but the Helm UI Mod is compatible with all, and if used, should be
activated first).

This feature allows for one Mod to be dependent on another Mod being active.

-------------------------------
Updates to Artemis Spaceship Bridge Simulator
-------------------------------
With each run, the Artemis Mod Loader compares the Artemis executable in the
copy with the install folder, and if the original install folder was updated,
then the Artemis Mod Loader will install the old version of Artemis as a Mod,
then then re-copy the install folder to the user folder (which does not require
Administrator access).  The user will be warned that this activity needs
to occur.

The version information cannot be checked since the Artemis executable does not
have version information on it.  Therefore, in order to determine if Artemis was
updated, file dates and sizes are compared.  If the available versions of Artemis
was known at the time of the release of this version of Artemis Mod Loader,
then the Mod title for the old version of Artemis

----------------------------------
Special Notes: for mods with their own installer (Battlestar Galactica)
----------------------------------
An ArtemisModLoader file can still be made to include the mod in the
ArtemisModLoader management, but requires that you install the mod from within
ArtemisModLoader so that it can identify which files are part of the mod and tag
those for the mod and not for the stock vanilla install.

If the mod is intended to permanently replace stock files, then the only way
this can be done is to completely uninstall ArtemisModLoader, install the mod,
then re-install ArtemisModLoader.

Note that the mod will not be installed if you install the mod outside
ArtemisModLoader and try to run Artemis through the ArtemisModLoader.

Although this mechanism does work, I'd like to request that the Mod developers
that do this offer a zip package instead, so that the Mod definition file
(".aml"), can be written to use this.

----------------------------------------------
Mod Development
----------------------------------------------
With version 2.0 of Artemis Mod Loader, developing your own Mods is made easy.

There are some things I have noted with how current Mods are built and what
is included in them, and based on my findings I am making the below
recommendations:

1. DO NOT INCLUDE a backup copy of the stock Artemis files in your Mod.
This is completely unnecessary if the Artemis Mod Loader is used, PLUS
if an updated version of the stock Artemis gets installed and it updates
any of the stock files included in your backup, your back will now be
invalid and could corrupt the stock Artemis.  Adding this backup also
makes your Mod package a significantly larger download (usually at least
twice as big as needed). If you wish to build a batch file that allows
for enabling of your Mod without Artemis Mod Loader, then add scripting
of backing up the Stock Artemis at runtime instead, leaving the choice
to back up at any time, with warnings not to back up if a Mod is
currently installed. Finally: WHATEVER YOU DO, DO NOT INCLUDE THE FILE
"ARTEMIS.EXE" in your backup.  This would definitely violate your
"End-User License Agreement" (EULA) as it would be considered distributing
a pirated copy of Artemis.


2. If your Mod depends on files from other Mods, you do have the choice
of including that other mod in the package for your Mod, but with Artemis
Mod Loader, this is not necessary.  With Artemis Mod Loader, you can build
your Mod and add information that your Mod is dependent on any number of
other Mods.  The order that you list your Mods should match the order that
the dependent Mods must be stacked.


Other Notes: 
If you build your Mod with Artemis Mod Loader, the Mod 
Definition File (.aml) will automatically get added to the root of your 
Zip file package.  I would recommend adding a basic readme.txt file to the
root that would instruct users where to find Artemis Mod Loader so that
they can use the Mod Loader for installing your Mod.  However, I leave
to you what you want to do.

One option is that instead of writing a batch file to install your
mod, you could offer links to download and install Artemis
Mod Loader in a ReadMe.txt file, which would simplify your work.
You could also include the *.msi file of the Mod Loader, if you desired,
but that could mean that the user will be installing an old version of Artemis
Mod Loader if I continue to update Artemis Mod Loader.

If you wanted to just include the executable for Artemis Mod Loader, so
that the user did not have to install Artemis Mod Loader, you will
also need all the .dll and the .config files.  The Mod Definitions
folder is not required, but highly recommended, but as you can see,
there are a lot of components--it would be a lot easier to just use
the .msi to install Artemis Mod Loader.



Finally, if you are masochistic, you can edit the .config file and
change the logging level to DEBUG, the review the application.log
file in the data path for Artemis Mod Loader.  DEBUG is verbose
level logging, and it'll create a 40MB log file in about three
seconds.  Suggested normal level will be WARN, which will only
log application errors, including those that it ignores.  See below for 
other levels.  I would request you set to DEBUG only if there
was a problem and I needed more information from you.


----------------------------------
Troubleshooting
----------------------------------

Symptom: 
When I activate a Mod, Artemis does not run (errors prevent start up).

Solution:
Make sure you installed the latest version of the Mod, the one compatible with
your version of Artemis.  Older versions of Mods may still be floating around
and can cause problems if installed on the wrong version of Artemis.  For
example, "Into The Breach" will only work if version 1.6 is installed.
version 1.5 of "Into The Breach" is incompatible with version 1.661 of Artemis.

--------------------------------------
Creating Your Own Mod
--------------------------------------
With Version 2 of the Artemis Mod Loader, Mod Development Management was added. for
The design plan of how things work are as follows:

1. You would either import your work-in-progress mod as a new install, or have the developer create a new install.
2. You would develop your art and other things as normal, then use the developer to import the files into your Mod's install.
3. You would use the vesselData editor to create your final ship definitions.
4. The artemis.INI file would be edited in either NotePad, the text editor of your choice, or within a text editor inside the Mod Developer (it would be user's choice).
5. A mapping editor would allow you to name and store the artemis.ini and vesselData.xml files any way you wanted, but then allow them to be named correctly and in the correct place once the Mod was activated.
6. Testing would be nothing more than deactivating your Mod (if activated), then reactivating to pick up any changes you made, then running Artemis.
7. Deployment would be to create a zip file and deposit all files from your installed Mod into that zip file, plus the *.aml file that defines the Mod for the Mod Loader.  You could then upload that zip file anywhere on your own.


--------------------------------------
Change History
--------------------------------------
version	Date		Description
0.1a	12/19/2012	Initial alpha release.  Not "Prettified".  Needs nice
			icons and graphics to make things look nice. Released
			to alpha tester(s) only.

0.2a    12/20/2012	Bug fix to disable "Uninstall" if mod is currently active.

			Added ability to install package from a definition file
			("*.aml") that is within the package.

			Added some tool-tips to various locations.

			Added drag-drop functionality on filenames.  If a file
			is dragged-and-dropped onto this application, it will
			process as if the user clicked "Install Mod" (or "install
			mission, depending on context) and selected file.

			Added nice gradient background that I ripped off from
			somewhere else.

			Added support for running and monitoring multiple Artemis
			sessions (for multiple clients and/or server).

			Added "Author" and "SupportsInvasionMode" on
			ModConfigurations.  SupportsInvasionMode defaults to
			"True".

			Added process to optionally clean up all user files upon
			uninstall.

			Added support for Mission management.

			Added support to track the IP addresses used for the
			Artemis Server and use a drop-down for the user	to select
			from.  This feature is experimental as it may require
			Administrator privilege to use.

0.3b	12/26/2012	Released to beta.

			Added "Author" and "Supports Invasion Mode" to Mod
			Definition File Creation window (oops--forgot it).

			Played around a little more with the background color.  I
			can use annoying colors if I so desire!

			Tweaked how Artemis is started to ensure the Working
			Directory gets set.

			Fixed the setup program where content files (the *.aml
			files) where not getting included.
					
			Added shortcut to Programs folder.

			Added check to verify Artemis install path.

			Added support for applying Mods to the Android client.

			Fixed bug for activating Mod with wild-carded source for
			BaseFiles.

			Fixed bug whereby "Activate" and "Deactivate" would not
			be correctly enabled after first activation/deactivation.

			Added enabling of the first variation (submod) when
			variations are offered with a mod (instead of no
			variations enabled upon activation of the mod).

			Added validation of vesselData.xml--checked the version
			number to ensure it is compatible with the current
			version of Artemis.  Current version of vesselData.xml
			should be 1.66, but I found 1.56 to be compatible.
			1.3 is not compatible.  Since the xml file could have a
			different name (such as Into The Breach has), all xml
			files are checked to see if they are a vesselData.xml
			file.

			"Prettified"--added icons everywhere.

			Fully tested install process and corrected numerous
			issues.

0.4b	12/26/2012	Updated links to "Into The Breach".  Was using old links.

0.5b	12/26/2012	Changed default logging from "INFO" level to "WARN" level.
			"INFO" level adds a LOT of logging.
			Logging levels: ERROR, WARN, INFO, DEBUG.  
			ERROR will be few and far between--basically only log when
			the application crashes.
			WARN logs errors that can be (and often are) ignored.
			INFO details neary every activity.
			DEBUG also adds entry and exit of all routines.
			Log file is stored at the same location as the data files
			("application.log").  Logging level and configuration is
			changed in the ArtemisModLoader.exe.config file (utilizes
			log4net).

			Fixed bug at startup--my update to the "Into the Breach"
			link was invalid for XML files.

0.6b	12/26/2012	Fixed bug where if the mod does not have a direct link, it
			would crash if you tried to install from web.

0.7b	12/26/2012	Added code to hide the "Browse to webpage" button if the
			web page is blank.

0.8b	12/26/2012	Added Button to About to check for updates.

			Added EULA to installer.

			Changed installer to automatically remove previous
			versions.

			Added ability to reset to the original Artemis vanilla
			stock install with one click (under "About").

			Fixed bug with the Mod Deactivation process whereby files
			overlaid by latest Mod would not get restored (such as
			"vesselData.xml").

0.9b	12/27/2012	Changed "Reset" so that all it did was deactivate
			everything instead of wiping everything out.

			Also changed reset so that the vanilla stock was refreshed
			from the original Artemis installation.

			Fixed reset so that everything deleted that should
			delete--was getting errors.

			Added thread locking to improve initial setup behavior.


1.0	12/28/2012	Added information about what the path to the Artemis copy
			is under "About".

			Fixed issue with installing missions from compressed
			files.  If the Mission file was in a sub-directory under
			the root, it would not get installed to the correct 
			location.

			Removed Command-line arguments as this functionality
			didn't really make sense.

			Removed unused code.

			Implemented code to check if Artemis itself had been
			updated and prompt to update the copy that the Mod Loader
			runs.

			Implemented centralized file processing so that the
			processing was sensitive to the file and the contents of
			the file as opposed to where the file was dropped.

			Fixed reported bug where the Loader would crash if it
			could not find a file when deactivating a mod. In the
			reported crash, the file that could not be found was in
			an installed mod.  It was missing for an unknown reason
			(perhaps manually deleted?), but in no case should this
			case the Loader to crash.  The problem would be reported
			instead, and processing would continue.

			Fixed bug whereby "Deactivate" was always enabled.  The
			truth is that only the last activated Mod could be
			deactivated (otherwise deactivation was not handled
			properly).

			Fixed bug whereby activating a Mod did not properly track
			the source file from the install, thus not correctly
			locating it when stacked mods on top of a mod were
			deactivated.

			Release from beta.  Seems stable enough for general use.

1.1	01/02/2013	Created a resource library for preparation for supporting
			multiple languages.  Basically, this application can now
			support any language, so long as someone is willing to do
			the translation.  Mods and missions defined by XML files
			will still remain in the native	language, but any text
			that is displayed that is internal to the Artemis Mod
			Loader can be translated into any language.  Not doing
			translations at this time--still have a lot of development
			to do.

			Changed references to "Artemis Mod Loader" text to use
			fixed Assembly Title.

			Added mission filename to the information displayed for
			missions.

			Fixed bug whereby missions added by a Mod was not getting
			added to the mission list until	Artemis Mod Loader was
			restarted.

			Added allowing to run the Artemis Extender Tool instead of
			Artemis directly, which in turn runs Artemis, but
			scripts various shortcuts into Artemis. This feature
			MAY be a little unstable as I have not performed
			significant testing.  Please report any problems.
			WARNING: Using the Artemis Extender Tool may limit the
			number of client sessions you can run to one session
			(don't try server and client). There is currently nothing
			to stop you from doing more than one session, but strange
			things may happen if you do.  Code will probably be added
			later to limit you to one session, but allow a second
			session to run ONLY if it is designated as the server. 

			Added Tre Chipman's Epsilon Sector Mod into predefined
			list of mods.

			Added "DependsOn" attribute for a Mod.

			Fixed bug on first run where the check for an update to
			Artemis would crash since the Artemis copy was not yet
			made.

2.0	01/16/2013	Centralized language resources between all projects.
			Will expose for language translators once Mod Development
			Management developement is complete. 

			Added fancier progress indicator.

			Refactored some of the code for more global use within
			application.

			Added Tre Chipman's Generic Mod.  If your Mod references
			elements from this Generic Mod, add the "DependsOn"
			attribute, set to "{653B3B9E-428B-4346-B6C3-2DAE47E900A5}".

			Added code to "sanitize" Xml from any source before
			loading it for processing.  Ran into instances where
			the vesselData.xml file and some mission files
			had invalid characters in the xml ("--" inside a comment,
			for example, or ">" in a quote), and because I was
			loading the Xml with the .Net XmlDocument, the
			processing would croak on those characters.  This
			sanitizing removes comments prior to load and changes
			">" to "&gt;", ">" to "&lt;", and "&" to "&amp;".  Will
			need to figure out what to do when I add support for
			"DependsOn" for missions--probably will search for the
			string prior to any processing.

			Improved format of Mod Definition panel.

			Fixed process that imported an update to Artemis (was
			a bit quirky).  Process now moves the old Stock
			Artemis into a "Mod" installation, then imports the
			updated version of Artemis, ending with requiring
			Artemis Mod Loader Restart.  If user wanted to run
			an old version of Artemis, now they only need to
			Activate that Mod.  This allows for easy rollback
			of a version of Artemis in the event problems
			arise (for example, when 1.7 of Artemis  was
			released, the Android Client was not compatible
			with it.  Now, all that is needed is to activate
			the version 1.661 Mod of Artemis, and you are
			good to go).

			Changed the URL in some of the pre-defined Mods
			so that it used the direct download link (which
			is not easily determined).  Updated the TMP and Epsilon Sector
			download links to the correct URL.

			Dressed up Settings.  Added button here to edit controls.ini
			in Notepad.

			Added Star Trek TNG Mod by Mark Bell as a pre-defined Mod.
			Tweaked a couple of the settings in the AML file from Mark
			Bell.

			Added slick-looking splash screen.  Needed something to occupy
			user while he waited for the Mod Loader to start up.

			Added button on Missions panel to edit in the Artemis Mission Editor
			tool.  Configuring to allow this is in Settings. 
			Requires v. 13.01.11.1 and above of the mission editor tool.

			Added Mod Development.  Enhancements include:
			- Editor for VesselData.xml
			- Editor for artemis.ini
			- Improved Mod definition: includes the ability to add
			Mod variations (could only be done through the Xml before)
			and custom mapping of files (such as relocating or renaming
			the artemis.ini and vesselData.xml files).
			- Merging of vesselData.xml files.  This is a great way
			to make sure you aren't missing any important ships
			or races.

			Old versions of vesselData.xml (prior to version 1.56, which includes "Adam's 
			Star Trek Mod" will load into this editor, but not correctly. Upon
			examining the vesselData file for "Adam's Star Trek Mod", I decided
			it was not worth my while to build a converter to update the file to
			the current version.  It is easier to simply fix the data in the
			editor manually.

2.0.1 	01/16/2013	Discovered "drone_port" was something that was part of vesselData.xml and
			added it in.

2.0.2 	01/16/2013  	Found "carrier" missing as well and added.  Fixed some issue with adding
			"drone_port".

2.0.3 	01/16/2013  	Bug fix whereby binding for the Mod between the ModManagement development
			was broken from the artemisini control.
			Changed logging from "DEBUG" to "WARN".  Don't need 100MB log files created everytime
			the application is run.

2.1.0 	01/16/2013	Changed the vessel window so that the tabs now are neatly scrolled down left side.

2.1.1 	01/16/2013 	When adding a new vessel, the broadtype dropdown disappeared.  Fixed by automatically
			selecting first broadtype in list.

			Carrier compliment would not port in.

2.1.2 	01/16/2013 	Fixed bug introduced with 2.1.1 on vessels.
			Added code so that an added vessel was selected for edit.
			Added code to generate a unique ID on a new vessel that is truly unique.

2.2.0 	01/17/2013	Added button to open vesselData.xml in Notepad
			Added button on ArtemisINI panel to play sounds.
			Added filtering of vessels by side on the VesselData editor.  Not real happy about
			the end result, but it works.  I'll probably come back to this later.

			Fixed bug where if certain data was not entered when saving, the application
			would crash trying to validate that data.  made change so that if
			there is a bug that does not check for a null object, it will instead
			log a warning and not crash, then continue processing as normal.

			Cleaned up a bit how the vesselData file was loaded in memory.  Process is now
			significantly faster.

2.2.1 	01/17/2013 	Minor change in the way it detects the Artemis installation.  Instead of doing it
			at startup, it now checks for it when it needs it, and prompts the user if it can't
			find it at that time.

2.3.0 	01/18/2013	Adding sorting on Vessels: by UniqueID and by Side.

			Adjusted the math on the ratio calculation for rendering the beam port lines.  Original
			calculation was unreliable and did not allow consistency between all beam ports on a
			vessel (beam ports of different ranges might render the same size). This change also leads
			the way to stacking all beam ports together to show an amalgamation.
			
			Added beam port stack to show amalgamation of beam ports on a vessel.

2.4.0	01/23/2013	Made the beam port stack introduced in 2.3.0 invisible when there were no beam ports on
			the vessel.
			
			Added FileSystemWatcher on vesselData.xml and artemis.ini so that if the underlying
			file gets changed while loaded, the user will be prompted to reload the file.
			
			Added "*" indicator to Mod Developer Manager windows to indicate to the user that 
			changes have been made that need saved.
			
			Changed the opening of artemis.ini and vesselData.xml into Notepad to use the default
			for xml and ini, whatever that might be.
			
			Modified the process for loading vesselData.xml so that comments and unknown attributes
			and nodes will no longer be dropped (though the order in the file cannot be assured).
			This enhancement allows for continued support even if future versions add new attributes.
			
			Bug fixed whereby the artemis.ini editor would on occasion save multiple entries for
			the same parameter.
			
			Fixed bug on selecting meshfile.  The filter for ".dxs" files was incorrect and so
			would not list them.
			
			Fixed bug where changing a file in the vesseldata control would not take, nor would
			it correctly identify that a set file did not exist.
			
			Added selector on the Beam Port congomerate to make it easier to visual each beam port
			with the whole.
			
			Updated the BSG Mod download link for BSG version 12.8.9
			
			Added File explorer for Mod in Mod Developer.
			
			Rearranged things for user efficiency.
			
			Added simple access to change the Network Port.
			
			Changed data type for how network port was stored from double to int.
			
			Updated the Artemis logo icon in a few places to a much slicker one, from the Artemis website.
			
			On the Vessel Data control, moved the "Edit in Notepad" button to the front so that it
			will be the selected button on startup.  Previously, the "New" button was the selected
			button on startup, which could cause a new vesseldata file to be created by accident
			simply if the user hit return.
			
			Since I figured that not everyone would be pleased with my color selection (some of the colors
			do kind of clash) I added the ability customize most of the colors on each panel. Look
			for a little drop-down, kind of hidden on each panel to adjust the background color.
			
2.4.1	01/24/2013	(released without fanfare)
			Added donation info on About window.  I've put a lot of effort into this--it would be nice
			to see some fruit.
			
			Bug fixed where classname on vessels could not be changed.
			
			Refactored the routines for drawing the arc on the beam ports.  Was drawing the arc
			about nine times per change.  Now it only draws it once or twice.  Not
			noticeable to the user, but was driving me nuts trying to debug an issue.
			
			Fixed filter for .snt files--bug was due to a typo.
			
			Added logging to identify the version of the application being run.

			Added code to prevent crash as reported by Tim Reiff.  Could not reproduce
			issue, but additional code should prevent it from occurring in first place.
			
2.4.2	01/24/2013	Discovered (to my horror) that vesselData.xml vessels were valid to have multiple "art"
			definitions.  Added the code to handle this in the vessel editor.
			
2.4.3	01/24/2013	"Damage" needed to be changed from an int type to a double type.

			Fixed bug where if the Xml data was a number, stored with a decimal point, and it was
			loading into an integer field, the integer field would not load the data and stay at zero.
			
			Fixed bug on new art control where initial data would not load until selecting next art and
			returning.
			
			Fixed bug where the "next/previous" control would allow you to go beyond the number of items
			in the list.
			
2.4.4	01/25/2013	Added one of the skybox images from Artemis as a background in a few places to make it look
			cool.
			
			Drone ports were not bound correctly, so the user would only see zeroes for the values for
			drone ports, even though the data was loaded.
			
2.5.0	01/29/2013	Corrected links to some of the actively updating Mods (TNG, TMP, BSG), and removed the ability
			for downloading from Web directly since it is not easy to keep the links updated.
			
			Added code to remove the "Download" button when the download link is unavailable.
			
			Added an Xml editor for the DMX commands and for vesselData.xml.  The editor is a bit in the
			experimental stages at this point, but it is a stepping stone for future development.
			
2.6.0	01/31/2013	Corrected indention on Xml Editor when the file has an xml declaration.

			Removed unused native Windows API calls.
			
			General enhancements made to the Xml Editor.
			
			Took a tangent and developed an Engineering Presets manager, modeled after the Engineering
			Presets manager developed by Hissatsu.  Can be found under Settings.
			
			Tweaked the predefined TMP Mod definition file.
			
2.6.1	02/01/2013	Corrected bug in Xml Editor end-tag code completion where tag had an underscore (it
			did not realize that underscore was valid in tags).
			
			Corrected bug in Xml Editor that auto-inserted tabs when typing in a closing tag.  The number
			of tabs inserted was excessive.
			
			General dress-up of Xml Editor, adding menu.
			
			Removed a few more ununsed native Windows API calls.
			
3.0.0	02/08/2013	Added mood music.

			Further refinement of Xml Editor
			
			Some refactoring of the Setup window.
			
			Fixed bug for Mod Developer when mapping files--if file being mapped to did not
			exist, you could not specify it.
			
			Added the "Into The Breach Look and Feel" mod by Dean Mallory as a Pre-defined mod.
			
			Added Mission Studio.
			
			Modified file selection in VesselData editor and file mapping so that backslashes are now forward slashes,
			per Xml standard.
		
3.0.1	02/08/2013	Added credits for the various components used for building Artemis Mod Loader.

			Had a setting wrong on "Into the Breach Look and Feel Mod" so that it did not get included in the installer.
			
			Discovered that the TNG mod was missing from installer as well and added it in.
			
3.0.2	02/11/2013	Tweaked the download source for "Into the Breach Look and Feel Mod" so that it downloads correctly.

			Bug Fix: VesselData editor: Turn Rate, Back Shields, and top speed would not get updated with changes. 
			
			Bug Fix: Vesseldata editor: When the arc width was "1", then the arc would not render.  Corrected by "faking" it.  Internally, the
			arc is rendered as just shy of 1 (0.999999) to fix the issue.
			
			Bug Fix: VesselData Editor: Previously, if a vessel was assigned an invalid side (hull race), the user would only be told
			that vessel(s) had invalid data.  Now, the user gets better feedback.
			
			Added code to ensure user settings gets kept whenever the application gets updated.
			
3.0.3	02/28/2013	Added Code completion on Mission Script for named objects.

			Added a Launch Artemis Mod Loader checkbox to the installer.
			
			Added code to self-update Artemis Mod Loader as updates become available.  I just have to remember to keep the update file on my DropBox account
			updated, or else the self-update won't self-update when an update is done because it won't detect an update.
			
			Found historical information as to release dates of older versions of Artemis, so added that in to the Artemis Install update-detection process for
			identifying Artemis versions.
			
			Corrected bug with prompt for Artemis Path when it could not find Artemis install path.  Prompts displayed funny due to being
			initiated in a background thread.
			
			Changed code for rendering backgrounds with the Artemis skyboxes so that the file would not be locked.  If the file is locked,
			there is the potential for crashing when applying a mod, or if the skybox must be updated for some reason.
			
3.0.4	03/03/2013	Added in the release date for Artemis version 1.702.

			Fixed bug where Artemis Mod Loader would crash on startup if it could not find the *.ogg files (which can happen when initially installed).
			
3.0.5	03/04/2013	Still reports of strange blow-up on startup.  Determined must be related to how things start up.  Code changed so that initialization
			occurs after main window is displayed, and found that the prompt for the Artemis Install location will tend to fly through without
			stopping (some kind of strange quirk with Microsoft's WPF).  A little googling gave the answer, so this quirk is resolved.
			Because things happen later, it should resolve the issue, but I'm uncertain due to difficulty in reproducing issue.
			
			Set the "Stand by" window to always be on top.
			
			Fixed issue with auto-update process.  Version 3.0.3 and 3.0.4 would not auto-update properly unles user had admin access to the C:
			drive.	Manual update was required.
			
3.0.6 	03/26/2013	If the Mod Definition file (*.aml) defined a folder structure that did not exist in the Mod pack (*.zip, *.rar), then Artemis Mod
			Loader would crash This fixes that issue by simply providing a warning message instead of crashing.  The Mod might not get applied--but at least we don't crash.
			
			Added version 2 of Mark Bell's TNG mod to the predefined list.  PLEASE NOTE: if you installed version 2 of TNG using the predefined
			TNG mod prior to this version, you will need to completely uninstall the mod.
			
			Added code so that the Standby window would only temporarily be the top-most window.  When it was always topmost, information messages would display behind it instead of being visible on top.
			
3.0.7	03/31/2013	Downloaded all mods to my Dropbox and set installation of Mods to pull from there.  This way I have better control over the mods, though I won't
			be able to keep up with updates.  I have had the problem in the past of folder structures changing, which wreaks all kinds of havoc on Artemis Mod Loader.  This way I will be able to note changes in the structure and fix things myself before anyone gets ahold of it.
			
			Forgot to include the TNG version 2 in the installer--should now be included.

3.0.8	05/10/2013	Adding a race to the vesselData file in the Mod Developer crashed the application, trying to split the parse out the type keys ("Friend",
			"Enemy", etc.) when they hadn't been defined, yet.

			Clicking "New" for a new vesselData file in the Mod Developer crashed the program.

			Added code to make sure Artemis Spaceship Bridge Simulator executable is found before trying to start it.  Had an occasion of people telling me that Artemis
			Mod Loader was blowing up when trying to play Artemis, when the log file said it couldn't find the Artemis executable, which is why it crashed--the only
			reason it won't find the Artemis executable is that Artemis isn't installed.

			Added code to check that the Artemis Spaceship Bridge Simulator is no longer installed and offer the user to remove the copy used by Artemis Mod Loader--so that they can remain within License restrictions.
			
3.0.9	05/14/2013	Adding a new vessel to the vesselData file in the Mod Development editor failed to add an Art definition, so the Art definition list
			was empty. Since the Art definition was not blocked, changes would not get saved, until an Art Definiton was added.  Also added code to
			disable entry if there is no available Art Defintion, since even one Art definition can be removed.
			
			Bug fix with the self-update process.  If an update was detected, the process would crash once the update was downloaded.  This is fixed, so that when the update is downloaded, the installer is run to install the update.  However, version 3.0.9 and prior will require manual update.
			
3.0.10	05/20/2013	Update to Star Trek: Into the Breach mod.  Verified all links to all other Mods

3.0.11	06/05/2013	Added "Flight of the Artemis" fan music mod.

			Fixed bug--if Mod Loader was playing music from the Artemis install folder, it would not be able to replace that music.  Solved problem by stopping all music when activating mods or updating the Stock version.  Also prevented crashing if a file copy was unsuccessful.
			
			Added code to prevent crashing if for some reason it is unable to play a music file (possibly due to file corruption?)  There may be a
			bit of quirkiness with the music playing at this time--but at least everything else should function fine.
			
			Fixed an issue with Star Trek: The Motion Picture Mod.  Activating it would cause Artemis to crash after starting the server.  This 
			is now fixed.
			
			Updated Into The Breach to indicate that it does now support Invasion Mode.
			
			NOTE: to see these changes to the MOD definitions, you MUST completely uninstall the MOD, close and restart Artemis Mod Loader, then
			re-install.  Updating an already installed Mod is on the To-Do list.
			
3.0.12	06/07/2013	Fixed the Mod definition for Star Trek: Into the Breach

3.0.13  06/20/2013	Added code to check for updates to installed Mod Definitions and automatically apply them.  If the Mod is Activated, simply deactivate
			and re-activate to apply the changes (no longer need to uninstall the mod).  This check will only occur after the first run of an update to Artemis Mod Loader.
			
			Changed References to "Android Client" to "Unofficial Android Client".  The Official Android Client cannot be modded, but Thom gave his blessing to the author of the Unofficial Android Client to continue his work--and the Unofficial Android Client can be modded.  The term "Unofficial" is added to differentiate the two versions.  Code to mod the official Android client will be added once it is possible to be modded.
			
			Added the TSN Overhaul Mod.
			
3.0.14	06/27/2013	Updated the link to the TSN Overhaul Mod.  To pick up the updates that the author of this mod did, you will need to uninstall and
			re-install the mod.  The update from 3.0.13 should allow for the automatic update of the download link of the mod definition if you
			have installed this mod, but this will be the first time it is in action, so any bugs in this function might crop up.
			
			Updated the definition file for TNG V2, provided by Mark Bell.  Again, this uses the new definition update feature, but the TNG V2 package was also updated, so it should be unintalled and re-installed to pick up the updates.
			
3.0.15	06/27/2013	Improved performance of activating a mod by only copying files into the Artemis copy that are not specifically defined in the baseFiles
			mapping.  This will prevent duplicate copies of files and copying files twice, so that the process should run faster on certain mods,
			plus the amount of disk space used is reduced.
			
			Strange and unexplainable error received on the file listing under Mod Development that crashed the application.  Added code to prevent
			crashing, but due to nature of error, was unable to fix root cause.
			
3.0.16	06/27/2013	Updated the version stored in modified vesselData.xml files to 1.702, from 1.66.

			One last tweak to TNG_V2 Mod.
			
3.0.17	07/24/2013	(not released) Removed version 1 of the TNG mod.

			Updated the link for the TSN Overhaul Mod
			
			Updated the vesselData definition for version 2.0 of Artemis to include taunts under Hullrace.
			
			Added mechanism to copy the copy of the Artemis install in its current state (with all applied mods), for easy transference to other computers.
			
3.0.18	07/26/2013	(not released) Updated the vesselData definition for version 2.0 of Artemis to include "Efficiency" under Performance.

3.0.19	08/14/2013	Updated the vesselData definition for version 2.0 of Artemis to include the new list of broadTypes under vessels and the "biomech"
			key under hullRace.

3.0.20	08/22/2013	Added additional updates for Artemis version 2.0, specifically the changes to the artemis.ini file.

			Added the two new ships to the list of ships for the artemis.ini file.
			
			Fixed the Battlestar Galactica Mod, both for v. 2.0 and for Artemis Mod Loader.  Artemis Mod Loader had a problem with the
			installer.  Since BSG needed updated anyhow, I extracted the files into a standard RAR file, which Artemis Mod Loader does a
			much better job of installing.
			
			Fixed the ability of adding "anomolyeater" and "astroideater" to the broadtype of vessels in the vesseldata file.
			
3.0.21	08/23/2013	Added "warship" as an additional ability to broadtype in the vesselData file.

3.0.22	08/30/2013	Refactored logging mechanism (internal issue only).

			General programming tweaks.
			
			Added DMX Editor (DMX Commander).
			
			Fixed TNG and TMP mod activation issues.
			
			Added better message information when a MOD definition is defined wrong.
			
			(See DMX_Readme.txt for change history on the DMX editor).
			
3.0.23	08/30/2013	Updated the web page link for all Mods pointing to the current Artemis Forum--apparently the forum altered all links recently.

3.0.24	01/28/2014	Updated AML for Star Trek: Into the Breach Mod.
				Updated a few general URL links.
				Updated DMX API.
				
3.1.25	02/xx/2014	Updated for Artemis Version 2.1.