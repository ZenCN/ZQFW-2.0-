window.App = angular.module("HistoryDisaster", ['ui.router']);

App.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when('', '/View');
    $urlRouterProvider.otherwise('/View');

    $stateProvider.state('Head', {
        url: '',
        templateUrl: 'Index/GetTemmplate?url=Public.Head',
        controller: 'HeadCtrl',
        abstract: true
    });
    $stateProvider.state('Head.Menu', {
        url: '',
        templateUrl: 'Index/GetTemmplate?url=HistoryDisaster.Menu',
        controller: 'MenuCtrl',
        abstract: true
    });
    $stateProvider.state('Head.Menu.View', {
        url: '/View',
        templateUrl: 'Index/GetTemmplate?url=HistoryDisaster.Main',
        controller: 'HistoryDisasterViewCtrl'
    });
}]);