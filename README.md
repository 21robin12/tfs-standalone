# TFS Standalone

A Windows app that aims to solve some of the annoying problems that come with using Team Foundation Server source control, such as:

 1. Merging multiple changes across several branches
 2. Comparing local changes to a shelveset
 
![merging changes](screenshot-merging.png)
![diffing against a shelveset](screenshot-shelveset-diff.png)
 
## Installation

 - Download and extract `Release.zip` 
 - Replace the values in `config.json` with info about your TFS project<sup>1</sup>
 - Run `TfsStandalone.UI.exe`
 
<sup>1</sup>`TfsStandalone.UI.exe` must be run under a Windows account that also has access to your TFS project. Otherwise, `altCredentials` must be provided - although storing your credentials in a text file is clearly a bad idea! In future releases, I intend to implement better authentication methods. All other config values are required.
 