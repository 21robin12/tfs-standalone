function ShelvesetFile(settings) {
    var self = this;

    self.filename = ko.observable(settings.filename);
    self.selected = ko.observable(false);
    self.viewModel = settings.viewModel;

    self.viewDiff = function () {
        self.viewModel.unselectAllFiles();
        self.selected(true);
        self.viewModel.loadDiff(self.filename());
    }
}

function ShelvesetDiffViewModel() {
    var self = this;

    self.shelvesets = ko.observableArray([]);
    self.isLoading = ko.observable(false);
    self.files = ko.observableArray([]);
    self.diffLines = ko.observableArray([]);

    self.selectedShelveset = null;

    self.loadShelvesets = function () {
        self.shelvesets([]);
        self.files([]);
        self.diffLines([]);
        self.selectedShelveset = null;
        self.isLoading(true);
        ajax.execute("ShelvesetDiffController", "GetShelvesets", null, function (res) {
            var parsed = JSON.parse(res);
            self.shelvesets(parsed);
            self.isLoading(false);
        });
    }

    self.shelvesetChanged = function (obj, event) {
        if (self.shelvesets.indexOf(event.target.value) > -1) {
            self.diffLines([]);
            self.selectedShelveset = event.target.value;
            self.loadShelvesetFilenames(self.selectedShelveset);
        }
    }

    self.loadShelvesetFilenames = function (shelvesetName) {
        self.files([]);
        self.isLoading(true);
        ajax.execute("ShelvesetDiffController", "GetShelvesetFilenames", [shelvesetName], function (res) {
            var parsed = JSON.parse(res);
            var mapped = ko.utils.arrayMap(parsed, function(filename) {
                return new ShelvesetFile({
                    filename: filename,
                    viewModel: self
                });
            });

            self.files(mapped);
            self.isLoading(false);
        });
    }

    self.unselectAllFiles = function() {
        ko.utils.arrayForEach(self.files(), function(file) {
            file.selected(false);
        });
    }

    self.loadDiff = function (filename) {
        self.diffLines([]);
        // TODO include ALL files from both local changes and shelveset.
        self.isLoading(true);

        self.diffTypes = {
            COPY: 0,
            INSERT: 1,
            DELETE: 2
        }

        function areAtLeastNCopiesBehind(parsed, n, i) {
            if (i < n) {
                return false;
            }

            for (var counter = 0; counter < n; counter++) {
                if (parsed[i - (counter + 1)].diffType !== self.diffTypes.COPY) {
                    return false;
                }
            }

            return true;
        }

        function areAtLeastNCopiesAhead(parsed, n, i) {
            if (parsed.length - (i + 1) < n) {
                return false;
            }

            for (var counter = 0; counter < n; counter++) {
                if (parsed[i + (counter + 1)].diffType !== self.diffTypes.COPY) {
                    return false;
                }
            }

            return true;
        }

        ajax.execute("ShelvesetDiffController", "GetDiff", [self.selectedShelveset, filename], function (res) {
            var parsed = JSON.parse(res);
            var lineNumber = 0;
            var formatted = [];
            var inCopyBlock = false;
            for (var i = 0; i < parsed.length; i++) {
                var line = parsed[i];
                
                if (line.diffType !== self.diffTypes.DELETE) {
                    lineNumber++;
                }

                if (line.diffType === self.diffTypes.COPY && areAtLeastNCopiesBehind(parsed, 3, i) && areAtLeastNCopiesAhead(parsed, 5, i)) {
                    inCopyBlock = true;

                    if (formatted[formatted.length - 1].isCopyEllipsis) {
                        // do nothing
                    } else {
                        // output ...
                        formatted.push({
                            lineNumber: "",
                            text: "...",
                            diffType: self.diffTypes.COPY,
                            isCopyEllipsis: true
                        });
                        formatted.push({
                            lineNumber: "",
                            text: "...",
                            diffType: self.diffTypes.COPY,
                            isCopyEllipsis: true
                        });
                        formatted.push({
                            lineNumber: "",
                            text: "...",
                            diffType: self.diffTypes.COPY,
                            isCopyEllipsis: true
                        });
                    }
                } else {
                    if (inCopyBlock) {
                        if (areAtLeastNCopiesAhead(parsed, 3, i)) {
                            // do nothing
                        } else {
                            inCopyBlock = false;

                            // output as standard
                            formatted.push({
                                lineNumber: line.diffType === self.diffTypes.DELETE ? "" : lineNumber,
                                text: line.text,
                                diffType: line.diffType,
                                isCopyEllipsis: false
                            });
                        }
                    } else {
                        // output as standard
                        formatted.push({
                            lineNumber: line.diffType === self.diffTypes.DELETE ? "" : lineNumber,
                            text: line.text,
                            diffType: line.diffType,
                            isCopyEllipsis: false
                        });
                    }
                }
            }

            self.diffLines(formatted);
            self.isLoading(false);
        });
    }

    self.loadShelvesets();
}