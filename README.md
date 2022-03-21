# Fantasy Grounds Module Exporter

Command line interface that exports Fantasy Grounds campaigns to modules completely outside of Fantasy Grounds.

[Download the latest version](https://github.com/Gtaray/FG-Exporter/releases/latest/download/fge.exe)

## But Why?

There are two primary reasons to want to create modules outside of Fantasy Grounds.

1. If you keep your modules in any kind of source control then this tool allows you to build modules automatically as part of your CI/CD process. It could happen automatically when you push a new version, and you wouldn't have to ever worry about exporting the module through Fantasy Grounds again.

2. If you develop modules that require exporting multiple versions of (a player and GM module for example), you could define two configuration files, one for each build, and run the exporter for each. You wouldn't have to worry about exporting data in the wrong module again

## How to Use

### Command Line Interface

```
Usage: fge [--version] [-i|--input CampaignFolder] [-o|--output OutputFolder] [-d|--data DataFolder] [-t|--thumbnail Thumbnail.png] [-c|--config ConfigFile]
Command line interface to export Fantasy Grounds modules from a campaign

Available options:
    -i, --input     Required. Path to the campaign folder that includes the db.xml to export.
    -o  --output    Required. Path to where the converted module is placed. Defaults to location of fge.exe
    -d  --data      Path to the Fantasy Grounds data folder.
    -t  --thumbnail Path to the thumbnail to use. Default is no thumbnail used
    -c, --config    Required. Path to json configuration file that defines the export.
    --help          Display this help screen.
    --version       Display version information.

Example: fge -i C:/FGU/campaigns/myCampaign -c C:/FGU/campaigns/myCampaign/export.json
```

### Configuration File

The configuration file is a json file that contains all of the parameters for how to export the module. Below is an example file with comments on what each property does. Unless a property is specified as optional, it is required.

The example file could be used for the 5e ruleset, and shows all possible record types that are exported. The "background" record type lists all of the possible options for record types, even though many of them aren't needed. The rest of the record types only list the properties they would need to work.

<details>
<summary>Click to expand example configuration file</summary>

```javascript
{
    // Relative or absolute path to the folder where the mod file will be put
    // Relative paths are to where the executable is located
    "outputFolder": "C:/FGU/modules",
    // Module's file name (minus the extension)
    "fileName": "moduleName",
    // Relative or absolute path to the thumbnail
    "thumbnail": "",
    // Module name
    "name": "Module_Name",
    // Module display name
    "displayName": "Module Name",
    // Module category
    "category": "Module Category",
    // Module author
    "author": "Your Name",
    // Optional. True if the module is read only. Defaults to false
    "readOnly": false,
    // Optional. True if exporting a player module. Defaults to false
    "playerModule": false,
    // Optional. True if exporting a module for any ruleset. Defaults to false
    "anyRuleset": false,
    // Ruleset label for the module.
    "ruleset": "5E",
    // Relative or absolute path to the folder where the FG data folder is located
    // This is the folder where campaigns, modules, images, tokens, etc. are located
    "fgDataFolder": "C:/FGU",
    // Optional. Integration with Ghost Writer extension. This property can be either "gm" or "player"
    // When set, the exporter will remove any content that doesn't match this property. If not set, then all content is exported
    "ghostWriter": "gm",
    // List of record types to export, configured individually
    "recordtypes": [
        {
            // Record type. This shoud match the ruleset's record identifier
            "recordType": "background",
            // The text that's used in the module's index for records of this type
            "libraryName": "Backgrounds",
            // Optional. Element name where this record type is found in the campaign's db.xml before exporting
            // Defaults to the same as recordType
            "dbPath": "background",
            // Optional. Element name where this record type is found in the module's db.xml after exporting
            // Defaults to the same as recordType
            "modulePath": "background",
            // Optional. Element name where this record type is found in a read-only module's db.xml after exporting.
            // Defaults to the same as recordType
            "referencePath": "backgrounddata",
            // Optional. Value for this record type's librarylink.class element in the module's library
            // Defaults to "reference_list"
            "librarylinkClass": "reference_list",
            // Optional. Value for this record type's librarylink.recordname element in the module's library.
            // Defaults to ".."
            "librarylinkRecordName": "..",
            // Optional. Boolean flag to include the recordtype element in the library entry for this record type
            // Defaults to true
            "includeLibraryRecordType": true,
            // Optional. List of record ids to export.
            // If not specified, all records of this type will be exported
            "records": [
                "id-00001",
                "id-00003",
                "id-00010"
            ]
        },
        {
            "recordType": "battle",
            "libraryName": "Encounters",
            "referencePath": "battles",
        },
        {
            "recordType": "battlerandom",
            "libraryName": "Encounters (Random)",
            "referencePath": "battlerandoms",
        },
        {
            // Exporting pregenerated characters these properties to work by default
            "recordType": "charsheet",
            "modulePath": "pregencharsheet",
            "libraryName": "Pregenerated Characters",
            "librarylinkClass": "pregencharselect",
            "librarylinkRecordName": "pregencharsheet",
            "includeLibraryRecordType":  false
        },
        {
            "recordType": "class",
            "libraryName": "Classes",
            "referencePath": "classdata"
        },
        {
            // NOTE: exporting effects does NOT support using the records element to specify specific effects.
            // NOTE: when exporting a read-only module, Effects do not have a referencePath property as they are not exported inside of the reference element.
            "recordType": "effects"
        },
        {
            // While this record type is called 'story', in the backend DB.xml files it is referred to as 'encounter', so we have to update the db and module path accordingly
            "recordType": "story",
            "libraryName": "Story",
            "dbPath": "encounter",
            "modulePath": "encounter",
            "referencePath": "encounters",
        },
        {
            "recordType": "feat",
            "libraryName": "Feats",
            "referencePath": "featdata",
        },
        {
            // NOTE: when exporting a read-only module, Images do not have a referencePath property as they are not exported inside of the reference element.
            "recordType": "image",
            "libraryName": "Images"
        },
        {
            "recordType": "item",
            "libraryName": "Items",
            "referencePath": "equipmentdata"
        },
        {
            "recordType": "itemtemplate",
            "libraryName": "Item Templates",
            "referencePath": "magicrefitemdata",
        },
        {
            // NOTE: exporting modifiers does NOT support using the records element to specify specific modifiers.
            // NOTE: when exporting a read-only module, modifiers do not have a referencePath property as they are not exported inside of the reference element.
            "recordType": "modifiers"
        },
        {
            "recordType": "npc",
            "libraryName": "NPCs",
            "referencePath": "npcdata",
        },
        {
            "recordType": "quest",
            "libraryName": "Quests",
            "referencePath": "questdata",
        },
        {
            "recordType": "race",
            "libraryName": "Races",
            "referencePath": "racedata",
        },
        {
            "recordType": "skill",
            "libraryName": "Skills",
            "referencePath": "skilldata",
        },
        {
            "recordType": "spell",
            "libraryName": "Spells",
            "referencePath": "spelldata",
        },
        {
            // When exporting a read-only module, story templates are located in 'storytemplates' (plural), while in  non-read-only modules they're located in 'storytemplate' (singular).
            "recordType": "storytemplate",
            "libraryName": "Story Templates",
            "referencePath": "storytemplates",
        },
        {
            // This record type is called 'table' (singular), but in the campaign and module DB.xml it is located in the 'tables' (plural) element.
            "recordType": "table",
            "libraryName": "Tables",
            "dbPath": "tables",
            "modulePath": "tables",
            "referencePath": "tables",
        },
        {
            // This record type is called 'treasureparcel' (singular), but in the campaign and module DB.xml it is located in the 'treasureparcels' (plural) element.
            "recordType": "treasureparcel",
            "libraryName": "Parcels",
            "dbPath": "treasureparcels",
            "modulePath": "treasureparcels",
            "referencePath": "treasureparcels",
        }
    ],
    // List of paths to any tokens to be exported. These are separate from any tokens exported as part of other records (like npcs).
    // The paths for these tokens should be relative, either from the 'campaign/' folder if the token is located in the campaign, or from the 'tokens/' folder if the token is located in Fantasy Grounds' global token folder.
    "tokens": [
        "campaign/tokens/token.png", // This token is located in the campaign's token folder
        "campaign/tokens/npcs/npc_token.png", // This token is located in the 'npcs' folder, which is located in the campaign's token folder
        "tokens/Monsters/monster_token.png", // This token is located in the 'Monsters' folder, which is located in the global token folder
        "tokens/PCs/pc_token.png"   // This token is located in the 'PCs' folder, which is located in the global token folder
    ]
}
```

</details>

### Creating your own Configuration File

When creating your own configuration file you can start with the above example. Delete entries for record types you don't want to export at all. If you want to export only specific records, then update the record types 'records' property with the ids to export (see the backgrounds record type for an example of how that looks).

## Submitting Bugs

This exporter has only been tested with the **CoreRPG** and **D&D 5e** rulesets. It _should_ be flexible enough to support any ruleset, but there are likely some rulesets that operate a little differently than normal. In these cases, feel free to submit an Issue on this github, leave a comment on the [Fantasy Grounds forum](https://www.fantasygrounds.com/forums/showthread.php?73056-I-Created-a-Command-Line-Module-Exporter), or message me through Discord (@Saagael#5728).

### [Ghost Writer](https://github.com/MeAndUnique/GhostWriter) Support

The [Ghost Writer](https://github.com/MeAndUnique/GhostWriter) extension allows you to define which content is bundled with which module release in a campaign (GM or Player). Normally you could do this by exporting only the records each module needs to have, but there's still one big limitation with that: the reference manual. A campaign can only have one reference manual, which means you would either need to keep two campaigns, possibly duplicating work, or edit the reference manual by hand after exporting

[Ghost Writer](https://github.com/MeAndUnique/GhostWriter) allows a user to specify whether pages in a reference manual should be exported for Players, GMs, or both, and injects itself into the Fantasy Grounds exporter to make those changes. This exporter does the same thing; by specifying the 'ghostWriter' property in the configuration file you can tell the exporter to only export player or GM content. This allows module creators to have a single campaign in which to create their content, while still allowing the flexibility of creating distinct player and GM modules.

## Limitations

This exporter does not use Fantasy Grounds' code in any way. It was created by comparing how data was transformed from the campaign's db.xml to the module's db.xml, and mimicking that behavior as best as could be done. This means that there are some edge cases that Fantasy Grounds handles behind the secene that this program doesn't, and can't, know about.

One such limitation is when exporting reference manuals, the exporter will not automatically create a list of keywords for each page. To the best of my knowledge, keywords are saved in the campaign's db.xml so a workaround is to close and re-open the campaign before exporting. This should populate the keywords for each page.

Another limitation is that the exporter knows absolutely nothing about the ruleset it's doing the conversion for. It doesn't know what data structures Fantasy Grounds uses, or how those data structures relate. This is why the configuration file must be as verbose as it is; it must be provided all of the data that Fantasy Grounds gets by default.

## Future Updates

### Github Action Support

This would allow you to have modules built as part of a github workflow. You could trigger that workflow on a commit to main, at which point the workflow would export your campaign to a module, create a release, and attach the .mod file to the release.

## Contributing

Fantasy Grounds supports dozens of rulesets, each with their own quirks and oddities, and I can't support all of them equally. If you are familiar with a specific ruleset that's not supported you are welcome to contribute and submit pull requests with updates.

Any contributions should be made in their own branch (not the main branch). You can then submit a pull request to merge your changes into main. For a pull request to be approved all unit tests must pass, and any new functionality must include new unit tests proving that it works. See the FGE.Tests project for examples how how those are set up.
