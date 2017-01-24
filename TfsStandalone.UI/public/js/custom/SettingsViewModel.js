function SettingsViewModel(data) {
    var self = this;

    var project = data.projects[0];

    var workspacePath = project ? project.workspacePath : "";
    var developerCmdPath = project ? project.developerCmdPath : "";
    var changesetsToIgnore = project ? project.ignoredChangesets.join(",") : "";
    var branchComparisons = project ? project.branchComparisons.map(function (bc) {
        return bc.from + ">" + bc.to;
    }).join(",") : "";

    self.projectCollectionUrl = ko.observable(data.url);
    self.username = ko.observable(data.username);
    self.workspacePath = ko.observable(workspacePath);
    self.developerCmdPath = ko.observable(developerCmdPath);
    self.branchComparisons = ko.observable(branchComparisons);
    self.changesetsToIgnore = ko.observable(changesetsToIgnore);

    self.save = function () {
        var data = [self.projectCollectionUrl(), self.username(), self.workspacePath(), self.developerCmdPath(), self.branchComparisons(), self.changesetsToIgnore()];
        ajax.execute("SettingsController", "SaveSettings", data);
    }
}