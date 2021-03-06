﻿window.App = angular.module("Index", ['ui.router', 'Scope.safeApply']);

App.config([
    '$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {

        var default_url = '/Open';
        switch (parseInt($.cookie("authority"))) {
            case 0:
                default_url = '/New';
                break;
            case 1:
                default_url = '/Open';
                break;
        }
        $urlRouterProvider.when('', default_url);
        $urlRouterProvider.otherwise(default_url);

        $stateProvider.state('Head', {
            url: '',
            templateUrl: 'Index/GetTemmplate?url=Public.Head',
            controller: 'HeadCtrl',
            abstract: true
        });
        $stateProvider.state('Head.Menu', {
            url: '',
            templateUrl: 'Index/GetTemmplate?url=Index.Menu',
            controller: 'MenuCtrl',
            abstract: true
        });
        $stateProvider.state('Head.Menu.New', {
            url: '/New',
            templateUrl: 'Index/GetTemmplate?url=Index.New',
            controller: 'IndexNewCtrl'
        });
        $stateProvider.state('Head.Menu.Open', {
            url: '/Open',
            templateUrl: 'Index/GetTemmplate?url=Index.Open',
            controller: 'IndexOpenCtrl'
        });
        $stateProvider.state('Head.Menu.Receive', {
            url: '/Receive',
            templateUrl: 'Index/GetTemmplate?url=Index.Receive',
            controller: 'IndexReceiveCtrl'
        });
        $stateProvider.state('Head.Menu.RecycleBin', {
            url: '/RecycleBin',
            templateUrl: 'Index/GetTemmplate?url=Index.RecycleBin',
            controller: 'IndexRecycleBinCtrl'
        });
        $stateProvider.state('Head.Menu.UrgeReport', {
            url: '/UrgeReport',
            templateUrl: 'Index/GetTemmplate?url=Index.UrgeReport',
            controller: 'IndexUrgeReportCtrl'
        });
    }
]);