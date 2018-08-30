
AppService.controller('loginController', function ($scope, $cookies, $http, requestService) {

    $scope.enterEvent = function (e) {
        var keycode = window.event ? e.keyCode : e.which;
        if (keycode == 13) {
            $scope.login();
        }
    }
    //点击登陆按钮
    $scope.login = function () {
        var userName = angular.element("#userName").val(), passWord = angular.element("#passWord").val();
        if (userName.length > 0 && passWord.length>0) {
            var parameter = { "userName": userName, "passWord": passWord };

            layui.use([], function () {
                var layer = layui.layer;
                var index = layer.load(1, { shade: 0.2 });
                requestService.login(parameter, '../webapi/user/login').then(function (response) {
                    layer.close(index);
                    var res = JSON.parse(response.data);
                    if (res.status == 0) {
                        //缓存用户信息
                        //$cookies.remove("username");
                        //$cookies.put("username", userName);
                        //$cookies.remove("userid");
                        //$cookies.put("userid", res.message);
                        document.cookie = "username="+userName+"; path=/";
                        document.cookie = "userid="+res.message+"; path=/";
                        document.cookie = "IFSignIn=LogIn; path=/";
                        window.location = "admin";
                    } else {
                        layui.use('layer', function () {
                            var layer = layui.layer;
                            layer.msg(res.message);
                        });
                    }
                }, function errorCallback(response) {
                    layer.close(index);
                });
            });
        } else {
            layer.msg("用户名或密码不能为空！");
        }
        
    };
});
