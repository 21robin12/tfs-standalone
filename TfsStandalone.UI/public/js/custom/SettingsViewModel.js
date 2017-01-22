function SettingsViewModel(data) {
    var self = this;

    self.projectCollectionUrl = ko.observable(data.projectCollectionUrl);
    self.username = ko.observable(data.username);
    self.workspacePath = ko.observable(data.workspacePath);
    self.developerCmdPath = ko.observable(data.developerCmdPath);
    self.branchComparisons = ko.observable(data.branchComparisons.replace(/&gt;/g, ">"));
    self.changesetsToIgnore = ko.observable(data.changesetsToIgnore);

    self.save = function () {
        var data = [self.projectCollectionUrl(), self.username(), self.workspacePath(), self.developerCmdPath(), self.branchComparisons(), self.changesetsToIgnore()];
        ajax.execute("SettingsController", "SaveSettings", data);
    }
}