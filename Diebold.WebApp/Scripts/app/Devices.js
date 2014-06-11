(function (Diebold, $, undefined) {
    var _gateways;

    Diebold.Devices = {
        
        selectHealthCheck: function (version) {
            // console.log("selectHealthCheck called");
                var dataItem = { "healthCheckVersion": version };
                var url = '/Device/AsyncDeviceTypes';
                $.post(url, dataItem, this.getHealthbyCompanyIdCallback, "json");

                return;
            },

            updateVersion: function (data) {
                if (!_gateways)
                    _gateways = data;
                // console.log(JSON.stringify(data));
                // console.log(data);
                var selectedGateway = _.find(_gateways, function (g) { return g.id == $('#GatewayId').val(); });
                // console.log(selectedGateway);
                var versionBox = $('#HealthCheckVersion').data("kendoComboBox");
                // console.log(versionBox);
                var version = 'Version' + selectedGateway.version;
                versionBox.value(version);
                $('#hHealthCheckVersion').val(version);
                this.selectHealthCheck(version);

            },
            getHealthbyCompanyIdCallback: function(ResultSet) {
                if (ResultSet != null && ResultSet != undefined) {
                    // console.log(ResultSet)
                    if (ResultSet.Message != null && ResultSet.Message != undefined && ResultSet.Message != "") {
                        fnShowAlertInfo(ResultSet.Message);
                    }
                    else {
                        var DTcombobox = $("#DeviceType").data("kendoComboBox");
                        DTcombobox.dataSource.data(ResultSet);
                    }
                }
            }
       
    };
}(window.Diebold = window.Diebold || {}, jQuery));
