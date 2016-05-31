window.App = angular.module("Index", ['ui.router', 'Scope.safeApply']);

App.config([
    '$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
        $urlRouterProvider.when('', '/Open');
        $urlRouterProvider.otherwise('/Open');

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
    }
]);