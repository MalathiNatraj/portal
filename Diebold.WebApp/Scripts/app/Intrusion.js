(function (Diebold, $, undefined) {
    var _deviceId, _devices;
    Diebold.Intrusion = {
        setDevices: function (devices) {
            // console.log("setting devices");
            // console.log(devices);
            _devices = devices;
        },
        getSelectedDevice: function () {
            // console.log(JSON.stringify(_devices));
            intrusionDeviceId = $("#IntrusionCategories").val();
            return _.find(_devices, function (device) {
                return device.Id == (parseInt(intrusionDeviceId) || 0); ;
            });
        },
        addUserCode: function (device, deviceType) {
            var deviceID = $("#IntrusionCategories").val();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {

                centerViewPortDialog("#dvAddUserCode");

                $("#txtUserName").val('');
                $("#txtUserCode").val('');

                var parameters = { "deviceId": deviceID, action: "add" };
                BlockElement("dvAddUserCodeContent", 'Connecting...');
                $.ajax({
                    url: "/intrusion/GetUserCodeView",
                    data: parameters,
                    dataType: "html",
                    type: 'POST',
                    success: function (data) {
                        $("#dvAddUserCode").html(data);
                        UnBlockElement("dvAddUserCodeContent");

                    }
                });
            }

        },
        modifyUserCode: function (device, deviceType) {
            var deviceID = $("#IntrusionCategories").val();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {
                $('#txtEditUserName').val('');
                $('#txtEditUserCode').val('');
                $("#dvErrUserCodeEdit ul").empty();
                var parameters = { "deviceId": deviceID, action: "modify" };
                BlockElement("dvStep1ModifyUserCode", 'Connecting...');
                $.ajax({
                    url: "/intrusion/GetUserCodeView",
                    data: parameters,
                    dataType: "html",
                    type: 'POST',
                    success: function (data) {
                        $("#dvStep1ModifyUserCode").html(data);
                        centerViewPortDialog("#dvStep1ModifyUserCode");
                    }
                });
                centerViewPortDialog("#dvStep1ModifyUserCode");
            }

        },
        deleteUserCode: function (device, deviceType) {
            var deviceID = $("#IntrusionCategories").val();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {
                $('#txtDeleteUserName').val('');
                $('#txtDeleteUserCode').val('');
                $("#dvErrUserCodeDelete ul").empty();

                var parameters = { "deviceId": deviceID, action: "delete" };
                BlockElement("dvStep1DeleteUserCode", 'Connecting...');
                $.ajax({
                    url: "/intrusion/GetUserCodeView",
                    data: parameters,
                    dataType: "html",
                    type: 'POST',
                    success: function (data) {
                        $("#dvStep1DeleteUserCode").html(data);
                        centerViewPortDialog("#dvStep1DeleteUserCode");

                    }
                });
                centerViewPortDialog("#dvStep1DeleteUserCode");
            }

        },
        fnCloseModifyStep1OpenModifyStep2: function (url) {

            $("#dvErrUserCodeEdit ul").empty();
            var regModify1Code = /^[0-9]+$/;
            var deviceID = $("#IntrusionCategories").val();
            var isErrModifyUC = "false";

            if ($('#txtEditUserName').val() == '' && $('#txtEditUserCode').val() == '') {
                $('#dvErrUserCodeEdit ul').append('<li class="validation-summary-intrusionpopup">Either Name or User Code is Required</li>');
                isErrModifyUC = "true";
            }
            if (isErrModifyUC == "false") {
                var dataItem = { "name": $('#txtEditUserName').val(), "userCode": $('#txtEditUserCode').val(), "deviceId": deviceID };

                BlockElement("dvStep1ModifyUCContent", 'Connecting...');
                $.post(url, dataItem, Diebold.Intrusion.getUserCodeEditFirstStepCallback, "json");
            }
        },

        fnCloseDeleteStep1OpenModifyStep2: function (url) {

            $("#dvErrUserCodeEdit ul").empty();
            var regModify1Code = /^[0-9]+$/;
            var deviceID = $("#IntrusionCategories").val();
            var isErrModifyUC = "false";

            if ($('#txtEditUserName', '.UserCodeDeleteStep1').val() == '' && $('#txtEditUserCode', '.UserCodeDeleteStep1').val() == '') {
                $('#dvErrUserCodeEdit ul').append('<li class="validation-summary-intrusionpopup">Either Name or User Code is Required</li>');
                isErrModifyUC = "true";
            }
            if (isErrModifyUC == "false") {
                var dataItem = { "name": $('#txtEditUserName', '.UserCodeDeleteStep1').val(), "userCode": $('#txtEditUserCode', '.UserCodeDeleteStep1').val(), "deviceId": deviceID };

                BlockElement("dvStep1DeleteUCContent", 'Connecting...');
                $.post(url, dataItem, Diebold.Intrusion.getUserCodeDeleteFirstStepCallback, "json");
            }
        },

        getUserCodeEditFirstStepCallback: function (ResultSet) {
            UnBlockElement("dvStep1ModifyUCContent");
            if (ResultSet != null && ResultSet != undefined) {
                if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
                    fnShowAlertInfo(ResultSet.Message);
                }
                else {
                    if (ResultSet[0] != null) {
                        var data = ResultSet[0];
                        $('#name', "#modify2").val(data.UserName);
                        $('#passcode', "#modify2").val(data.UserCode);
                        var i = 1;
                        for (var i = 1; i <= 32; i++) {
                            var str = "" + i;
                            var pad = "00";
                            pad = pad.substring(0, pad.length - str.length) + str
                            // console.log(pad);
                            $('#cbAccess_' + i, '#modify2').val(data.AccessLevels["area" + pad]);
                        }
                        $('#cboProfileNumberModify').data("kendoComboBox").value(data.ProfileNumber);
                        $('#userNumber').val(data.UserNumber);
                        $('.UserCodeModifyStep1').hide();
                        $('.UserCodeModifyStep2').show();
                    }
                }
            }
        },
        getUserCodeDeleteFirstStepCallback: function (ResultSet) {
            UnBlockElement("dvStep1DeleteUCContent");
            if (ResultSet != null && ResultSet != undefined) {
                if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
                    fnShowAlertInfo(ResultSet.Message);
                }
                else {
                    if (ResultSet[0] != null) {
                        var data = ResultSet[0];
                        $('#name', "#delete2").val(data.UserName);
                        $('#passcode', "#delete2").val(data.UserCode);
                        var i = 1;
                        for (var i = 1; i <= 32; i++) {
                            var str = "" + i;
                            var pad = "00";
                            pad = pad.substring(0, pad.length - str.length) + str
                            // console.log(pad);
                            $('#cbAccess_' + i, "#delete2").val(data.AccessLevels["area" + pad]);
                        }
                        $('#cboProfileNumberDelete').data("kendoComboBox").value(data.ProfileNumber);
                        $('#userNumber', "#delete2").val(data.UserNumber);
                        $('.UserCodeDeleteStep1').hide();
                        $('.UserCodeDeleteStep2').show();
                    }
                }
            }
        },
        getMedia: function (type, device, zoneNumber) {
            var deviceID = $("#IntrusionCategories").val();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {
                $("#txtUserName").val('');
                $("#txtUserCode").val('');

                var parameters = { "deviceId": deviceID, "zoneNumber": zoneNumber, "mediaType": type };
                BlockElement("Intrusion-content", 'Retrieving...(<span id="countdown"></span>)');
                timeIt(5, "countdown");

                var modal = $('.pemodal').data('modal');
                var content = $('.modalContent');


                $.ajax({
                    url: "/Intrusion/CaptureMedia",
                    data: parameters,
                    dataType: "html",
                    type: 'GET',
                    success: function (data) {
                        UnBlockElement("Intrusion-content");
                        // console.log(data);
                        obj = JSON.parse(data);
                        var filename = obj.FileName;
                        var status = obj.Status;
                        if (status.toUpperCase() == "COMPLETED") {
                            if (obj.MediaType.toUpperCase() == "IMAGE") {
                                content.html("<img src=\"" + filename + "\" />" +
                                "<div class=\"videoInfo\">Click <a href=\"" + filename + "\" target=\"_blank\">here</a> to view your " + obj.MediaType.toLowerCase() + ". You may also download the image by right-clicking.</div>");
                            } else {
                                content.html(
                        "<div class=\"videoInfo\">Click <a href=\"" + filename + "\" target=\"_blank\">here</a> to view your " + obj.MediaType.toLowerCase() + ". You may also download the image by right-clicking.</div>");
                            }

                        } else {
                            alert("Could not get media at this time");
                        }
                        modal.open();


                    }
                });
            }

        },
        getSelectedDevice: function () {
            intrusionDeviceId = $("#IntrusionCategories").val();
            return _.find(_devices, function (device) {
                return device.Id == (parseInt(intrusionDeviceId) || 0); ;
            });
        },
        newWindow: function (content, error) {

            if (error) {
                var x = window.open();
                x.document.open();
                x.document.write(html);
                x.document.close();
                x.focus();
            } else {
                var x = window.open(content, "mediaWindow");
                x.focus();
            }
        },
        submitNewUserCode: function () {
            $("#dvErrUserCodeAdd ul").empty();
            var ProfileValue = $("#cboProfileNumberAdd").val();
            var deviceID = $("#IntrusionCategories").val();
            var regUserCode = /^[0-9]+$/;
            var isValid = true;

            if ($('#name').val() == '') {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Name is Required</li>');
                isValid = false;
            }

            if ($('#passcode').val() == '') {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Pass Code is Required</li>');
                isValid = false;
            } else if (!regUserCode.test($('#passcode').val())) {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Please enter a valid User Code</li>');
                isValid = false;
            }

            if (ProfileValue == '' || ProfileValue == "-- Select --") {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Profile Number is Required</li>');
                isValid = false;
            }

            if (isValid) {
                var formData = new Object();
                $.each($('#addUserCodeForm :input').serializeArray(), function (i, field) {
                    formData[field.name] = field.value;
                });
                var dataItem = { "ProfileNumber": ProfileValue, "deviceId": deviceID };
                //console.log(formData);
                //console.log(dataItem);
                formData = _.extend(formData, dataItem);
                // console.log("test");
                // console.log(formData);

                var url = '/Intrusion/UserCodeAdd';
                BlockElement("dvAddUserCodeContent", 'Connecting...');
                $.post(url, formData, Diebold.Intrusion.callbacks.getUserCodeAddCallback, "json");
            }

        },
        submitModifyUserCode: function () {
            $("#dvErrUserCodeAdd ul").empty();
            var ProfileValue = $("#cboProfileNumberModify", '#modify2').val();
            var deviceID = $("#IntrusionCategories").val();
            var regUserCode = /^[0-9]+$/;
            var isValid = true;

            if ($('#name', '#modify2').val() == '') {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Name is Required</li>');
                isValid = false;
            }

            if ($('#passcode', '#modify2').val() == '') {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">User Code is Required</li>');
                isValid = false;
            } else if (!regUserCode.test($('#passcode', '#modify2').val())) {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Please enter a valid User Code</li>');
                isValid = false;
            }

            if (ProfileValue == '' || ProfileValue == "-- Select --") {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Profile Number is Required</li>');
                isValid = false;
            }

            if (isValid) {
                var formData = new Object();                
                $.each($('#modify2 :input').serializeArray(), function (i, field) {                
                    formData[field.name] = field.value;
                });
                var dataItem = { "ProfileNumber": ProfileValue, "deviceId": deviceID };
                // console.log(formData);
                // console.log(dataItem);
                formData = _.extend(formData, dataItem);
                // console.log("test");
                // console.log(formData);

                var url = '/Intrusion/UserCodeModify';
                BlockElement("dvAddUserCodeContent", 'Connecting...');
                $.post(url, formData, Diebold.Intrusion.callbacks.getUserCodeModifyCallback, "json");
            }

        },
        submitDeleteUserCode: function () {
            $("#dvErrUserCodeAdd ul").empty();
            var ProfileValue = $("#cboProfileNumberModify", '#delete2').val();
            var deviceID = $("#IntrusionCategories").val();
            var regUserCode = /^[0-9]+$/;
            var isValid = true;

            if ($('#name', '#delete2').val() == '') {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Name is Required</li>');
                isValid = false;
            }

            if ($('#passcode', '#delete2').val() == '') {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">User Code is Required</li>');
                isValid = false;
            } else if (!regUserCode.test($('#passcode', '#delete2').val())) {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Please enter a valid User Code</li>');
                isValid = false;
            }

            if (ProfileValue == '' || ProfileValue == "-- Select --") {
                $('#dvErrUserCodeAdd ul').append('<li class="validation-summary-intrusionpopup">Profile Number is Required</li>');
                isValid = false;
            }

            if (isValid) {
                var formData = new Object();
                $.each($('#delete2 :input').serializeArray(), function (i, field) {                
                    formData[field.name] = field.value;
                });
                var dataItem = { "ProfileNumber": ProfileValue, "deviceId": deviceID };
                // console.log(formData);
                // console.log(dataItem);
                formData = _.extend(formData, dataItem);
                // console.log("test");
                // console.log(formData);

                var url = '/Intrusion/UserCodeDelete';
                BlockElement("dvDeleteUserCodeContent", 'Connecting...');
                $.post(url, formData, Diebold.Intrusion.callbacks.getUserCodeDeleteCallback, "json");
            }

        },
        getUserInformation: function (e) {

        },
        callbacks: {
            getUserCodeAddCallback: function (ResultSet) {
                UnBlockElement("dvAddUserCodeContent");
                if (ResultSet != null && ResultSet != undefined) {
                    if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
                        fnShowAlertInfo(ResultSet.Message);
                    }
                    else {
                        if (ResultSet == "OK") {
                            fnShowAlertInfo('User code added successfully.');
                            document.getElementById('my_overlay').style.visibility = 'hidden';
                            document.getElementById('dvAddUserCode').style.visibility = 'hidden';
                        }
                    }
                }
            },
            getUserCodeModifyCallback: function (ResultSet) {
                UnBlockElement("dvAddUserCodeContent");
                if (ResultSet != null && ResultSet != undefined) {
                    if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
                        fnShowAlertInfo(ResultSet.Message);
                    }
                    else {
                        if (ResultSet == "OK") {
                            fnShowAlertInfo('User code modified successfully.');
                            document.getElementById('my_overlay').style.visibility = 'hidden';
                            document.getElementById('dvModifyUserCode').style.visibility = 'hidden';
                        }
                    }
                }
            },
            getUserCodeDeleteCallback: function (ResultSet) {
                UnBlockElement("dvDeleteUserCodeContent");
                if (ResultSet != null && ResultSet != undefined) {
                    if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
                        fnShowAlertInfo(ResultSet.Message);
                    }
                    else {
                        if (ResultSet == "OK") {
                            fnShowAlertInfo('User code deleted successfully.');
                            document.getElementById('my_overlay').style.visibility = 'hidden';
                            document.getElementById('dvDeleteUserCode').style.visibility = 'hidden';
                        }
                    }
                }
            }

        }
    };
} (window.Diebold = window.Diebold || {}, jQuery));