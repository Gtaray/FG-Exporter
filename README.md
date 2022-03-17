# Fantasy Grounds Module Exporter

Command line interface that exports Fantasy Grounds campaigns to modules completely outside of Fantasy Grounds.

## But Why?

There are two primary reasons to want to create modules outside of Fantasy Grounds.

1. If you keep your modules in any kind of source control then this tool allows you to build modules automatically as part of your CI/CD process. It could happen automatically when you push a new version, and you wouldn't have to ever worry about exporting the module through Fantasy Grounds again.

2. If you develop modules that require exporting multiple versions of (a player and GM module for example), you could define two configuration files, one for each build, and run the exporter for each. You wouldn't have to worry about exporting data in the wrong module again

## How to Use

### Command Line Interface

```
Usage: fge [--version] [-i|--input CampaignFolder] [-c|--config ConfigFile]
Command line interface to export Fantasy Grounds modules from a campaign

Available options:
    -i, --input     Required. Path to the campaign folder that includes the db.xml to export.
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
    ]
}
```

</details>

### Creating your own Configuration File

When creating your own configuration file you can start with the above example. Delete entries for record types you don't want to export at all. If you want to export only specific records, then update the record types 'records' property with the ids to export (see the backgrounds record type for an example of how that looks).

## Submitting Bugs

This exporter has only been tested with the **CoreRPG** and **D&D 5e** rulesets. It _should_ be flexible enough to support any ruleset, but there are likely some rulesets that operate a little differently than normal. In these cases, feel free to submit an Issue on this github, leave a comment on the Fantasy Grounds forum, or message me through Discord (@Saagael#5728).

## Future Updates

### Export Tokens

Tokens contained in any records will be exported, but currently the exporter doesn't handle exporting specific tokens. This will be added in the next update.

### [Ghost Writer ](https://github.com/MeAndUnique/GhostWriter) Support

The extension allows you to define which content is bundled with which module release in a campaign (GM or Player). Using this exporter would allow you to change a single configuration property to export two different modules. Notably this would include being able to have different reference manuals for each module, while still only keeping a single campaign to develop in.

### Github Action Support

This would allow you to have modules built as part of a github workflow. You could trigger that workflow on a commit to main, at which point the workflow would export your campaign to a module, create a release, and attach the .mod file to the release.

## Contributing

Fantasy Grounds supports dozens of rulesets, each with their own quirks and oddities, and I can't support all of them equally. If you are familiar with a specific ruleset that's not supported you are welcome to contribute and submit pull requests with updates.

All contributions for ruleset specific changes must include unit tests proving the converter works. See the FGE.Tests project for examples how how those are set up.
