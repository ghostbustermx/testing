(function () {
    'use strict';
    // Get main app module
    var app = angular.module(appName)
    app.controller('UserController', ['$scope', '$window', '$state', 'UserService', 'AccessService', 'ModalConfirmService', 'ngToast', '$http', 'EmployeeInfoService', 'ProjectService', function ($scope, $window, $state, UserService, AccessService, ModalConfirmService, ngToast, $http, EmployeeInfoService, ProjectService) {
        $scope.currentPage = 1;
        $scope.pageSize = 10;
        HasAccess();
        $scope.employees = [];
        $scope.users = [];
        $scope.projects = [];
        $scope.projectsAssigned = [];
        $scope.userSelected = [];
        $scope.action = "";
        $scope.addEditAction = "";
        $scope.UItoEdit = false;
        $scope.UIProject;
        $scope.userToUpdate = "";
        $scope.errorMessage = "";
        $scope.userAssigned = { selectedItems: null };
        $scope.projectAssigned = { selectedItems: null };
        $scope.roleAssigned = { selectedItems: null };
        $scope.projectsToAdd = [];


        $scope.Options = [];

        function GetPaginationElements() {
            AccessService.GetPagination(function (data) {
                $scope.Options = data;
                $scope.RowsSelected = $scope.Options[0];
            }, function (error) {
            });
        }

        $scope.changeValue = function (value) {
            $scope.pageSize = value;
        }

        $scope.setVariables = function (action, user) {
            $scope.AddEditAction = action;
            $scope.errorMessage = "";

            if (action == 'Add') {
                $scope.UItoEdit = false;
                $scope.addEditAction = "Add"
                $scope.projectsAssigned = [];
                $scope.userSelected = [];
                $scope.userAssigned = { selectedItems: null };
                $scope.projectAssigned = { selectedItems: null };
                $scope.roleAssigned = { selectedItems: null };
                $scope.projectsToAdd = [];
                $scope.projectsAssigned = [];
                $scope.userSelected = [];
            } else {
                $scope.addEditAction = "Edit"
                $scope.UItoEdit = true;
                GetInformation(user)
                $scope.userToUpdate = user;

            }
        }

        $scope.Action = function (action) {
            if (action == 'Add') {
                AddUser();
            } else {
                UpdateUser();
            }
        }

        function GetInformation(User) {
            $scope.employees.Items.forEach(function (u) {
                if (u.UserName == User.UserName) {
                    $scope.userAssigned.selectedItems = u;
                }
            });
            var role = UnconvertRole(User.Role);
            if (role == 'Tester By Project' || role == 'Business Analyst' || role == 'Viewer By Project') {
                $scope.UIProject = false;
            } else {
                $scope.UIProject = true;
            }

            $scope.projectAssigned = {};
            $scope.projectsAssigned = [];
            $scope.roleAssigned = { selectedItems: UnconvertRole(User.Role) };
            UserService.GetUsersProjects(User.Id, function (data) {
                data.forEach(function (p) {
                    var proj = {
                        Id: p.ProjectId,
                        Name: p.ProjectName,
                    }
                    $scope.projectsAssigned.push(proj);
                });
            }, function (error) {
            });
        }



        $scope.rolesOptions = ['Tester By Project', 'Viewer By Project', 'Business Analyst', 'Tester', 'Tester Full Access'];
        function HasAccess() {
            AccessService.HasAccessUserSection(function (data) {
                if (!data.HasPermission) {
                    $state.go('Projects').then(function () {
                        ngToast.danger({
                            content: 'You do not have access to the requested page.'
                        });
                    });
                } else {
                    GetPaginationElements();
                    GetEmployees();
                    GetUsers();
                    GetProjects();
                }
            }, function (error) {
            });
        };

        function GetUsers() {
            UserService.GetAll(function (data) {
                $scope.users = data;
            }, function (error) {
            });
        };


        $scope.AddProject = function (Project) {
            var exists = false;

            $scope.projectsAssigned.forEach(function (element) {
                if (element.Name === Project.Name) {
                    exists = true;
                }
            });
            if (!exists) {
                $scope.projectsAssigned.push(Project);
            }
        }


        function GetEmployees() {
            EmployeeInfoService.GetEmployees(function (data) {
                $scope.employees = data;
            }, function (error) {

            });
        }



        function GetProjects() {
            ProjectService.GetActives(function (data) {
                $scope.projects = data;
            }, function (error) {

            });
        }


        $scope.SelectValidation = function (role) {
            var r = ConvertRole(role);
            if (r == 'TBP' || r == 'BA' || r == 'VBP') {
                $scope.UIProject = false;
            } else {
                $scope.UIProject = true;
                $scope.projectAssigned = {};
                $scope.projectsAssigned = [];
            }
        }


        function AddUser() {
            if ($scope.userAssigned.selectedItems != null && $scope.roleAssigned.selectedItems != null) {
                var userProject = {
                    User: {
                        Id: 0,
                        UserName: $scope.userAssigned.selectedItems.UserName,
                        Role: ConvertRole($scope.roleAssigned.selectedItems),
                        FirstName: $scope.userAssigned.selectedItems.FirstName,
                        LastName: $scope.userAssigned.selectedItems.LastName,
                        Email: $scope.userAssigned.selectedItems.Email,
                        PhotoUrl: $scope.userAssigned.selectedItems.PhotoUrl,
                        Division: $scope.userAssigned.selectedItems.Division,
                        JobTitle: $scope.userAssigned.selectedItems.JobTitle,
                        Gender: $scope.userAssigned.selectedItems.Gender,
                        Department: $scope.userAssigned.selectedItems.Department,
                        Alias: $scope.userAssigned.selectedItems.Alias,
                        HireDate: $scope.userAssigned.selectedItems.HireDate,
                        IsActive: true
                    },
                    Projects:
                        [

                        ]
                }

                var exists = false;
                GetUsers();
                $scope.users.forEach(function (u) {
                    if (u.UserName == userProject.User.UserName) {
                        exists = true;
                    }
                });

                if (exists) {
                    ngToast.danger({
                        content: 'The user already exists.'
                    });
                } else {
                    var isValid = false;

                    if ((userProject.User.Role == 'TBP' || userProject.User.Role == 'BA' || userProject.User.Role == 'VBP') && $scope.projectsAssigned.length > 0) {
                        getProjectsAssigned();
                        userProject.Projects = $scope.projectsToAdd;
                        isValid = true;
                    }
                    if (userProject.User.Role != 'TBP' && userProject.User.Role != 'BA' && userProject.User.Role != 'VBP') {
                        userProject.Projects = null;
                        isValid = true;
                    }

                    if (isValid) {
                        UserService.Save(userProject, function (data) {
                            GetUsers();
                            $('#modalAddUserForm').modal('hide');
                            ngToast.success({
                                content: 'The user has been added.'
                            });
                        }, function (error) {

                        });
                    } else {
                        ngToast.danger({
                            content: 'Please fill all the fields.'
                        });
                    }
                }
            } else {
                ngToast.danger({
                    content: 'Please fill all the fields.'
                });
            }
        }




        function UpdateUser() {
            if ($scope.userAssigned.selectedItems != null && $scope.roleAssigned.selectedItems != null) {
                var userProject = {
                    User: {
                        Id: $scope.userToUpdate.Id,
                        UserName: $scope.userAssigned.selectedItems.UserName,
                        Role: ConvertRole($scope.roleAssigned.selectedItems),
                        FirstName: $scope.userAssigned.selectedItems.FirstName,
                        LastName: $scope.userAssigned.selectedItems.LastName,
                        Email: $scope.userAssigned.selectedItems.Email,
                        PhotoUrl: $scope.userAssigned.selectedItems.PhotoUrl,
                        Division: $scope.userAssigned.selectedItems.Division,
                        JobTitle: $scope.userAssigned.selectedItems.JobTitle,
                        Gender: $scope.userAssigned.selectedItems.Gender,
                        Department: $scope.userAssigned.selectedItems.Department,
                        Alias: $scope.userAssigned.selectedItems.Alias,
                        HireDate: $scope.userAssigned.selectedItems.HireDate,
                        IsActive: $scope.userToUpdate.IsActive
                    },
                    Projects:
                        [

                        ]
                }

                var isValid = false;

                if ((userProject.User.Role == 'TBP' || userProject.User.Role == 'BA' || userProject.User.Role == 'VBP') && $scope.projectsAssigned.length > 0) {
                    getProjectsAssigned();
                    userProject.Projects = $scope.projectsToAdd;
                    isValid = true;
                }
                if (userProject.User.Role != 'TBP' && userProject.User.Role != 'BA' && userProject.User.Role != 'VBP') {
                    userProject.Projects = null;
                    isValid = true;
                }

                if (isValid) {
                    UserService.Update(userProject, function (data) {
                        GetUsers();
                        $('#modalAddUserForm').modal('hide');
                        ngToast.success({
                            content: 'The user has been edited.'
                        });
                    }, function (error) {
                    });
                } else {
                    ngToast.danger({
                        content: 'Please fill all the fields.'
                    });
                }
            } else {
                ngToast.danger({
                    content: 'Please fill all the fields.'
                });
            }
        }



        $scope.removeProject = function (item) {
            var index = $scope.projectsAssigned.indexOf(item);
            $scope.projectsAssigned.splice(index, 1);

            if ($scope.projectAssigned.selectedItems == item) {
                //Remove element from the project dropdown
                $scope.projectAssigned = { selectedItems: null };
            }


        }

        $scope.selectUser = function (user) {
            $scope.userSelected = user;
            if (user.IsActive) {
                $scope.action = "Disable";
            } else {
                $scope.action = "Enable";
            }
        }


        $scope.DisableUser = function (user) {
            if (user.IsActive) {
                user.IsActive = false;
            } else {
                user.IsActive = true;
            }
            UserService.Disable(user, function (data) {
                GetUsers();
            }, function (error) {
            });
        }

        function ConvertRole(role) {
            switch (role) {
                case 'Tester By Project':
                    return "TBP";
                case 'Viewer By Project':
                    return "VBP";
                case 'Business Analyst':
                    return "BA";
                case 'Tester':
                    return "TESTER";
                case 'Tester Full Access':
                    return "TFA";
            }
        }

        function UnconvertRole(role) {
            switch (role) {
                case 'TBP':
                    return 'Tester By Project';
                case 'VBP':
                    return 'Viewer By Project';
                case 'BA':
                    return 'Business Analyst';
                case 'TESTER':
                    return 'Tester';
                case 'TFA':
                    return "Tester Full Access";
            }
        }


        function getProjectsAssigned() {
            $scope.projectsToAdd = [];
            $scope.projectsAssigned.forEach(function (u) {
                var p = {
                    Id: 0,
                    ProjectId: u.Id,
                    ProjectName: u.Name,
                    UserId: 0,
                }
                $scope.projectsToAdd.push(p);
            });
        }
    }])
})();