versioncontrol_ONI
------------

This console program automatizes compiling of ONI projects. Since it's only made for me, there might be bugs or issues depending on the project structure.

-log PATH

Path to game log file. Defaults to %appdata%\..\LocalLow\Klei\Oxygen Not Included\Player.log when omitted. File must exist.

-md PATH

Optional. Path to source file changelog.md. Will read the first mod version line for later use. Will print the game version into changelog.

-info PATH

Optional. Path to output file mod_info.yaml. Will update values.

-asbly PATH

Optional. Path to source file AssemblyInfo.cs. Will update mod version.

-state PATH

WARNING, this will overwrite a source file!
Optional. Path to source file _CustomizeXState.cs. Will make mod ready for localization. Attributes will get localization string keys and LoadStrings() will be replaced with those values. Note these requirements:
* custom code style rules will probably cause issues
* you need 'namespace'
* you need the method 'public static void LoadStrings()', it will be overwritten
* properties and attributes must be in a single line
* put your options into named regions e.g. '#region Settings'
* you can prefix a region with $ to keep it category-less
* number of decimal places is copied from default value
* if variable ends with 'Percent' uses format 'P'

Unused arguments:
-projectname
-stateoverwrite

