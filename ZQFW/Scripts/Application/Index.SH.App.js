window.App = angular.module("Index", ['ui.router', 'index.directive']);/*.run(function ($http, $cacheFactory) {
    $http.defaults.cache = $cacheFactory('cache');
});*/

App.config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {

    /*if (Number($.cookie("limit")) < 4) {
        $urlRouterProvider.when('', '/Open');
        $urlRouterProvider.otherwise('/Open');
    } else {
        $urlRouterProvider.when('', '/New');
        $urlRouterProvider.otherwise('/New');
    }*/

    $urlRouterProvider.when('', '/New');
    $urlRouterProvider.otherwise('/New');

    $stateProvider.state('Head', {
        url: '',
        templateUrl: '/Index/GetTemmplate?url=Public.SH.Head',
        controller: 'IndexCtrl',
        abstract: true
    });
    $stateProvider.state('Head.Menu', {
        url: '',
        templateUrl: '/Index/GetTemmplate?url=Public.SH.Menu',
        abstract: true
    });
    $stateProvider.state('Head.Menu.New', {
        url: '/New',
        templateUrl: '/Index/GetTemmplate?url=Public.SH.New'
    });
    $stateProvider.state('Head.Menu.Open', {
        url: '/Open',
        templateUrl: '/Index/GetTemmplate?url=Public.SH.Open'
    });
    $stateProvider.state('Head.Menu.Receive', {
        url: '/Receive',
        templateUrl: '/Index/GetTemmplate?url=Public.SH.Receive'
    });
}]);