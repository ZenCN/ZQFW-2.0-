window.App = angular.module("Town", ['ui.router']);

App.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when('', '/New');
    $urlRouterProvider.otherwise('/New');

    $stateProvider.state('Head', {
        url: '',
        templateUrl: 'Index/GetTemmplate?url=Public.Head',
        controller: 'HeadCtrl',
        abstract: true
    });
    $stateProvider.state('Head.Menu', {
        url: '',
        templateUrl: 'Index/GetTemmplate?url=Town.Menu',
        controller: 'MenuCtrl',
        abstract: true
    });
    $stateProvider.state('Head.Menu.New', {
        url: '/New',
        templateUrl: 'Index/GetTemmplate?url=Town.New',
        controller: 'TownNewCtrl'
    });
    $stateProvider.state('Head.Menu.Secretary', {
        url: '/Secretary',
        templateUrl: 'Index/GetTemmplate?url=Public.UnderConstruction',
        controller: 'TownSecretaryCtrl'
    });
}]);

