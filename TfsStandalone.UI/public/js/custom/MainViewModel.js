﻿function NavItemViewModel(settings) {
    var self = this;

    self.icon = settings.icon;
    self.title = settings.title;
    self.isActive = ko.observable(settings.isActive);

    self.css = ko.computed(function () {
        return self.isActive() ? "primary-nav__item--active" : "primary-nav__item";
    });

    self.iconCss = ko.computed(function () {
        return "fa fa-2x " + self.icon;
    });
}

function MainViewModel(data) {
    var self = this;

    self.navItems = ko.observableArray();
    self.lowerNavItems = ko.observableArray();

    self.unmergedChangesetsViewModel = null;
    self.unmergedChangesetsNavItem = ko.observable(null);

    self.shelvesetDiffViewModel = null;
    self.shelvesetDiffNavItem = ko.observable(null);

    self.settingsViewModel = null;
    self.settingsNavItem = ko.observable(null);

    self.init = function () {
        self.unmergedChangesetsViewModel = new UnmergedChangesetsViewModel();
        self.unmergedChangesetsNavItem(new NavItemViewModel({
            icon: "fa-compress",
            title: "Unmerged Changesets",
            isActive: true
        }));

        self.shelvesetDiffViewModel = new ShelvesetDiffViewModel();
        self.shelvesetDiffNavItem(new NavItemViewModel({
            icon: "fa-files-o",
            title: "Shelveset Diff",
            isActive: false
        }));

        self.navItems([
            self.unmergedChangesetsNavItem,
            self.shelvesetDiffNavItem
        ]);

        self.settingsViewModel = new SettingsViewModel(data.settings);
        self.settingsNavItem(new NavItemViewModel({
            icon: "fa-cog",
            title: "Settings",
            isActive: false
        }));

        self.lowerNavItems([
            self.settingsNavItem
        ]);
    };

    self.selectNavItem = function (navItem) {
        var activeItem = ko.utils.arrayFirst(self.navItems().concat(self.lowerNavItems()), function (item) {
            return item().isActive() === true;
        });

        activeItem().isActive(false);
        navItem.isActive(true);
    }

    self.init();
}