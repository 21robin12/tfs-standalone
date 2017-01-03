function BranchComparison(settings) {
    var self = this;

    self.fromBranch = settings.fromBranch;
    self.toBranch = settings.toBranch;
    self.projectId = settings.projectId;

    self.data = ko.observableArray();
    self.isLoading = ko.observable(false);
    self.autoLoadTimestamp = null;
    self.autoLoadEnabled = false;

    self.title = ko.observable(self.fromBranch + " → " + self.toBranch);

    self.loadUnmergedChanges = function () {
        if (self.isLoading() === false) {
            self.isLoading(true);

            self.autoLoadTimestamp = null;

            var data = [self.fromBranch, self.toBranch, self.projectId];
            ajax.execute("UnmergedChangesetsController", "GetUnmergedChanges", data, function (res) {
                var parsed = JSON.parse(res);
                self.data(parsed);

                if (self.autoLoadEnabled === true) {
                    var timestamp = Date.now();
                    self.autoLoadTimestamp = timestamp;
                    setTimeout(function () { self.autoLoad(timestamp); }, 300000);
                }
            
                self.isLoading(false);
            });
        }
    }

    self.merge = function (changesetId) {
        var data = [changesetId, self.projectId, self.fromBranch, self.toBranch];
        ajax.execute("UnmergedChangesetsController", "Merge", data);
    }

    self.autoLoad = function (timestamp) {
        if (self.autoLoadTimestamp === timestamp) {
            self.loadUnmergedChanges();
        }
    }

    self.loadUnmergedChanges();
}

function UnmergedChangesetsViewModel() {
    var self = this;

    self.branchComparisons = ko.observableArray();
    self.isLoading = ko.observable(false);

    self.load = function () {
        self.isLoading(true);
        ajax.execute("UnmergedChangesetsController", "Load", null, function (res) {
            var parsed = JSON.parse(res);

            var bcs = ko.utils.arrayMap(parsed.branchComparisons, function(branchComparison) {
                return new BranchComparison({
                    fromBranch: branchComparison.from,
                    toBranch: branchComparison.to,
                    projectId: parsed.id
                });
            });

            self.branchComparisons(bcs);
            self.isLoading(false);
        });
    }

    self.load();
}