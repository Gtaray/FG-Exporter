# Fantasy Grounds Module Exporter

Command line interface that exports Fantasy Grounds campaigns to modules completely outside of Fantasy Grounds.

[Download the latest version](https://github.com/Gtaray/FG-Exporter/releases/latest/download/fge.exe)

[See the wiki for more documentation](https://github.com/Gtaray/FG-Exporter/wiki)

## Submitting Bugs

This exporter has only been tested with the **CoreRPG** and **D&D 5e** rulesets. It _should_ be flexible enough to support any ruleset, but there are likely some rulesets that operate a little differently than normal. In these cases, feel free to submit an Issue on this github, leave a comment on the [Fantasy Grounds forum](https://www.fantasygrounds.com/forums/showthread.php?73056-I-Created-a-Command-Line-Module-Exporter), or message me through Discord (@Saagael#5728).

## Contributing

Fantasy Grounds supports dozens of rulesets, each with their own quirks and oddities, and I can't support all of them equally. If you are familiar with a specific ruleset that's not supported you are welcome to contribute and submit pull requests with updates.

Any contributions should be made in their own branch (not the main branch). You can then submit a pull request to merge your changes into main. For a pull request to be approved all unit tests must pass, and any new functionality must include new unit tests proving that it works. See the FGE.Tests project for examples how how those are set up.
