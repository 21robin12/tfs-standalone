function SettingsViewModel() {
    var self = this;

    self.projectCollectionUrl = ko.observable("url here");
    self.username = ko.observable("username");
    self.workspacePath = ko.observable("workspace path");
    self.developerCmdPath = ko.observable("dev cmd");
    self.branchComparisons = ko.observable("branch compare");
    self.changesetsToIgnore = ko.observable("ignored changesets");
}