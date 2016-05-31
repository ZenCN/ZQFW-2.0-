window.App = angular.module("BaseData", ['ui.router']);

App.config(['$stateProvider', '$urlRouterProvider', function($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.when('', '/Index');
    $urlRouterProvider.otherwise('/Index');

    $stateProvider.state('Head', {
        url: '',
        templateUrl: 'Index/GetTemmplate?url=Public.Head',
        controller: 'HeadCtrl',
        abstract: true
    });
    $stateProvider.state('Head.Main', {
        url: '/Index',
        templateUrl: 'Index/GetTemmplate?url=BaseData.Main',
        controller: 'MainCtrl'
    });
}]);