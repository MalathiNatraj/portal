(function (Diebold, $, undefined) {
    var _deviceId, _devices;
    Diebold.Access = {
        setDevices: function (devices) {
            // console.log("setting devices");
            // console.log(devices);
            _devices = devices;
        },
        getSelectedDevice: function () {
            // console.log(JSON.stringify(_devices));
            intrusionDeviceId = $("#AccessViewDeviceList").val();
            return _.find(_devices, function (device) {
                return device.Id == (parseInt(intrusionDeviceId) || 0); ;
            });
        },
        addAccessGroup: function (device, deviceType) {
            var deviceID = $("#AccessViewDeviceList").val();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {
                centerViewPortDialog("#dvAddAccessTimePeriods");
                var parameters = { "deviceId": deviceID, action: "addAccessGroup" };
                BlockElement("dvAddAGContent", 'Connecting...');
                $.ajax({
                    url: "/MyAccessPoints/GetCardholderView",
                    data: parameters,
                    dataType: "html",
                    type: 'POST',
                    success: function (data) {
                        if (data.indexOf('"Status":"Error"') != -1) {
                            UnBlockElement("dvAddAGContent");
                            var strErr = data.substring(29, data.length - 2);
                            fnShowAlertInfo(strErr);
                        }
                        else {
                            UnBlockElement("dvAddAGContent");
                            $("#dvAddAccessTimePeriods").html(data);
                            // centerViewPortDialog("#dvAddAccessTimePeriods");
                        }
                    }
                });
               // UnBlockElement("dvAddAGContent");
            }
        },
        updateAccessGroup: function (device, deviceType) {
            var deviceID = $("#AccessViewDeviceList").val();
            var lbldvType = $("#lbldvType").text();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {
                if (lbldvType == 'dmpXR100Access' || lbldvType == 'dmpXR500Access') {
                    centerViewPortDialog("#dvDMPXRModifyAccessGroups");
                    var parameters = { "deviceId": deviceID, action: "updateAccessGroup" };
                    BlockElement("dvDMPXRCardholderModifyContent", 'Connecting...');
                    $.ajax({
                        url: "/MyAccessPoints/GetCardholderView",
                        data: parameters,
                        dataType: "html",
                        type: 'POST',
                        success: function (data) {
                            if (data.indexOf('"Status":"Error"') != -1) {
                                UnBlockElement("dvDMPXRCardholderModifyContent");
                                var strErr = data.substring(29, data.length - 2);
                                fnShowAlertInfo(strErr);
                            }
                            else {
                                UnBlockElement("dvDMPXRCardholderModifyContent");
                                $("#dvDMPXRModifyAccessGroups").html(data);
                            }
                        }
                    });
                }
                else {
                    centerViewPortDialog("#dvModifyAccessGroups");
                    var parameters = { "deviceId": deviceID, action: "updateAccessGroup" };
                    BlockElement("dvCardholderModifyContent", 'Connecting...');
                    $.ajax({
                        url: "/MyAccessPoints/GetCardholderView",
                        data: parameters,
                        dataType: "html",
                        type: 'POST',
                        success: function (data) {
                            if (data.indexOf('"Status":"Error"') != -1) {
                                UnBlockElement("dvCardholderModifyContent");
                                var strErr = data.substring(29, data.length - 2);
                                fnShowAlertInfo(strErr);
                            }
                            else {
                                UnBlockElement("dvCardholderModifyContent");
                                $("#dvModifyAccessGroups").html(data);
                            }
                        }
                    });
                }
            }
        },
        deleteAccessGroup: function (device, deviceType) {
            var deviceID = $("#AccessViewDeviceList").val();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {

                centerViewPortDialog("#dvDeleteAccessGroups");

                var parameters = { "deviceId": deviceID, action: "deleteAccessGroup" };
                BlockElement("dvDeleteAGContent", 'Connecting...');
                $.ajax({
                    url: "/MyAccessPoints/GetCardholderView",
                    data: parameters,
                    dataType: "html",
                    type: 'POST',
                    success: function (data) {
                        if (data.indexOf('"Status":"Error"') != -1) {
                            UnBlockElement("dvDeleteAGContent");
                            var strErr = data.substring(29, data.length - 2);
                            fnShowAlertInfo(strErr);
                        }
                        else {
                            UnBlockElement("dvDeleteAGContent");
                            $("#dvDeleteAccessGroups").html(data);
                        }
                    }
                });
            }
        },
        modifyCardholder: function (device, deviceType) {
            var deviceID = $("#AccessViewDeviceList").val();
            var lbldvType = $("#lbldvType").text();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {            
                if (lbldvType == 'dmpXR100Access' || lbldvType == 'dmpXR500Access') {
                    centerViewPortDialog("#dvDMPXRModifyCardHolder");
                    var parameters = { "deviceId": deviceID, action: "modify" };
                    BlockElement("dvDMPXRCardholderModify2Contentss", 'Connecting...');
                    $.ajax({
                        url: "/MyAccessPoints/GetCardholderView",
                        data: parameters,
                        dataType: "html",
                        type: 'POST',
                        success: function (data) {
                            UnBlockElement("dvDMPXRCardholderModify2Contentss");
                            $("#dvDMPXRModifyCardHolder").html(data);
                        }
                    });
                }
                else {

                    centerViewPortDialog("#dvModifyCardHolder");
                    var parameters = { "deviceId": deviceID, action: "modify" };
                    BlockElement("dvCardholderModify2Contentss", 'Connecting...');
                    $.ajax({
                        url: "/MyAccessPoints/GetCardholderView",
                        data: parameters,
                        dataType: "html",
                        type: 'POST',
                        success: function (data) {
                            UnBlockElement("dvCardholderModify2Contentss");
                            $("#dvModifyCardHolder").html(data);
                        }
                    });
                }
            }

        },
        deleteCardHolder: function (device, deviceType) {
            var deviceID = $("#AccessViewDeviceList").val();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {
                centerViewPortDialog("#dvDeleteCrdHldr");
                var parameters = { "deviceId": deviceID, action: "delete" };
                BlockElement("dvCardDeleteContent", 'Connecting...');
                $.ajax({
                    url: "/MyAccessPoints/GetCardholderView",
                    data: parameters,
                    dataType: "html",
                    type: 'POST',
                    success: function (data) {
                        UnBlockElement("dvCardDeleteContent");
                        $("#dvDeleteCrdHldr").html(data);
                    }
                });
                //centerViewPortDialog("#dvDeleteCrdHldr");
            }

        },
        addAccessCardholder: function (device, deviceType) {
            var deviceID = $("#AccessViewDeviceList").val();
            if (deviceID == '') {
                fnShowAlertInfo("Please select a device");
            } else {
                centerViewPortDialog("#dvAddCardHolder");

                // $('#cboCHAddAccessGroup').val('');
                // $("#txtUserName").val('');
                //$("#txtUserCode").val('');

                var parameters = { "deviceId": deviceID, action: "add" };
                BlockElement("dvCardholderAddContent", 'Connecting...');
                $.ajax({
                    url: "/MyAccessPoints/GetCardholderView",
                    type: 'POST',

                    dataType: "html",
                    data: parameters,
                    success: function (data) {
                        if (data.indexOf('"Status":"Error"') != -1) {
                            UnBlockElement("dvCardholderAddContent");
                            var strErr = data.substring(29, data.length - 2);
                            fnShowAlertInfo(strErr);
                        }
                        else {
                            $("#dvAddCardHolder").html(data);
                            UnBlockElement("dvCardholderAddContent");
                        }
                    }
                });
                //UnBlockElement("dvCardholderAddContent");
            }

        }
    };
} (window.Diebold = window.Diebold || {}, jQuery));